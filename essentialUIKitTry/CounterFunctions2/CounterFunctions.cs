using System;
using System.Linq;
using System.Timers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
//using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CounterFunctions
{
    public static class CounterFunctions
    {
        private static readonly AzureSignalR SignalR = new AzureSignalR(Environment.GetEnvironmentVariable("AzureSignalRconnectionString"));
       // const double interval60Minutes = 60 * 60 * 1000; // milliseconds to one hour
      //  const double interval55Minutes = 55 * 60 * 1000; // milliseconds to 55 min


        [FunctionName("negotiate")]
        public static async Task<SignalRConnectionInfo> NegotiateConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage request,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Negotiating connection for user: < {request.Content.ReadAsStringAsync()}>.");
                ConnectionRequest connectionRequest = await ExtractContent<ConnectionRequest>(request);
                log.LogInformation($"Negotiating connection for user: <{connectionRequest.UserId}>.");

                string clientHubUrl = SignalR.GetClientHubUrl("CounterHub");
                string accessToken = SignalR.GenerateAccessToken(clientHubUrl, connectionRequest.UserId);
                return new SignalRConnectionInfo { AccessToken = accessToken, Url = clientHubUrl };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to negotiate connection.");
                throw;
            }
        }


        // -------------------- OCCUPY AND AVAILABLE ------------------------------
        [FunctionName("set-occupy")]
        public static async Task SetOccupy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
            [Table("LockerRoom")] CloudTable cloudTable,
            [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("SetOccupy function . . .");
            Locker counterRequest = await ExtractContent<Locker>(request);
            Locker cloudLocker = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudLocker.available = false;
            cloudLocker.locked = false;
            cloudLocker.user_key = counterRequest.user_key;
            cloudLocker.release_time = counterRequest.release_time;
            ScheduleRelease(cloudLocker, cloudTable, signalRMessages , log);
            TableOperation updateOperation = TableOperation.Replace(cloudLocker);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "Occupy",
                    Arguments = new object[] { cloudLocker }
                });
        }


        [FunctionName("set-available")]
        public static async Task SetAvailable(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
    [Table("LockerRoom")] CloudTable cloudTable,
    [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
    ILogger log)
        {
            log.LogInformation("Setting Available.");

            Locker counterRequest = await ExtractContent<Locker>(request);

            Locker cloudLocker = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudLocker.locked = true;
            cloudLocker.available = true;
            cloudLocker.release_time = DateTimeOffset.Now.AddHours(0);
            cloudLocker.user_key = "";
            cloudLocker.photo_taken = false;
            TableOperation updateOperation = TableOperation.Replace(cloudLocker);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "available",
                    Arguments = new object[] { cloudLocker }
                });
        }


        [FunctionName("set-photo-taken")]
        public static async Task SetPhotoTakene(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
    [Table("LockerRoom")] CloudTable cloudTable,
    [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
    ILogger log)
        {
            log.LogInformation("Setting Available.");

            Locker counterRequest = await ExtractContent<Locker>(request);

            Locker cloudLocker = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudLocker.photo_taken = true;
            TableOperation updateOperation = TableOperation.Replace(cloudLocker);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "photoTaken",
                    Arguments = new object[] { cloudLocker }
                });
        }


        // -------------------- SET COSTS ------------------------------
        [FunctionName("set-cost")]
        public static async Task SetCost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
            [Table("LockerRoom")] CloudTable cloudTable,
            [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            Locker counterRequest = await ExtractContent<Locker>(request);
            Locker cloudLocker = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudLocker.price_per_hour = counterRequest.price_per_hour;
            //ScheduleRelease(cloudLocker, cloudTable, signalRMessages, log);
            TableOperation updateOperation = TableOperation.Replace(cloudLocker);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "setCosts",
                    Arguments = new object[] { cloudLocker }

                });
        }



        // -------------------- LOCK AND UNLOCK ------------------------------
        [FunctionName("set-lock")]
        public static async Task SetLock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
            [Table("LockerRoom")] CloudTable cloudTable,
            [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("Setting Occupy.");

            Locker counterRequest = await ExtractContent<Locker>(request);

            Locker cloudCounter = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudCounter.locked = true;
            TableOperation updateOperation = TableOperation.Replace(cloudCounter);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "lock",
                    Arguments = new object[] { cloudCounter }
                });

        }


        [FunctionName("set-unlock")]
        public static async Task SetUnlock(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
    [Table("LockerRoom")] CloudTable cloudTable,
    [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
    ILogger log)
        {
            log.LogInformation("Setting Available.");

            Locker counterRequest = await ExtractContent<Locker>(request);

            Locker cloudCounter = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudCounter.locked = false;
            TableOperation updateOperation = TableOperation.Replace(cloudCounter);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "unlock",
                    Arguments = new object[] { cloudCounter }
                });
        }


        [FunctionName("set-user-balance")]
        public static async Task SetUserBalance(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
    [Table("usersBalance")] CloudTable cloudTable,
    [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
    ILogger log)
        {
            log.LogInformation("Setting user balance");

            UserBalance userbalanceRequest = await ExtractContent<UserBalance>(request);

            UserBalance userBalance = await GetOrCreateBalance(cloudTable, userbalanceRequest.user_key);
            userBalance.balance = userbalanceRequest.balance;
            TableOperation updateOperation = TableOperation.Replace(userBalance);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "set-balance",
                    Arguments = new object[] { userBalance }
                });
        }



        // --------------------  GET/SET USER KEY  ------------------------------
        [FunctionName("set-user-key")]
        public static async Task SetUserKey(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "set-user-key/{user_key}")] HttpRequestMessage request,
        [Table("LockerRoom")] CloudTable cloudTable,
        [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
        ILogger log,
        String user_key)
        {
            log.LogInformation("Setting user key.");
            Locker counterRequest = await ExtractContent<Locker>(request);
            Locker cloudLocker = await GetOrCreateCounter(cloudTable, counterRequest.Id);
            cloudLocker.user_key = user_key;
            TableOperation updateOperation = TableOperation.Replace(cloudLocker);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "user-key",
                    Arguments = new object[] { cloudLocker }
                });
        }


        [FunctionName("get-user-key")]
        public static async Task<string> GetUserKey(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-user-key/{id}")] HttpRequestMessage request,
            [Table("LockerRoom")] CloudTable cloudTable,
            string id,
            ILogger log)
        {
            log.LogInformation("Getting User Key.");
            Locker locker = await GetOrCreateCounter(cloudTable, int.Parse(id));
            log.LogInformation("Returning: " + locker.user_key);
            return locker.user_key;
        }





        // ------------------------------------------------------------------

        [FunctionName("get-locker")]
        public static async Task<Locker> GetLocker(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-locker/{id}")] HttpRequestMessage request,
            [Table("LockerRoom")] CloudTable cloudTable,
            string id,
            ILogger log)
        {
            log.LogInformation("Getting locker.");
            Locker locker = await GetOrCreateCounter(cloudTable, int.Parse(id));
            string locker_str = JsonConvert.SerializeObject(locker);
            log.LogInformation("Getting locker. " + locker_str);
            return locker;
            //return locker_str;
        }


        [FunctionName("get-balance")]
        public static async Task<double> GetBalance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-balance/{user_key}")] HttpRequestMessage request,
            [Table("usersBalance")] CloudTable cloudTable,
            string user_key,
            ILogger log)
        {
            log.LogInformation("Getting balance.");
            UserBalance userbalance = await GetOrCreateBalance(cloudTable, user_key);
            string userBalance_str = JsonConvert.SerializeObject(userbalance);
            log.LogInformation("Getting locker. " + userBalance_str);
            return userbalance.balance;
            //return locker_str;
        }


        [FunctionName("get-all-lockers")]
        public static async Task<List<Locker>> GetAllLockers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-all-lockers")] HttpRequestMessage request,
            [Table("LockerRoom")] CloudTable cloudTable,
            ILogger log)
        {

            TableQuery<Locker> idQuery = new TableQuery<Locker>();

            TableQuerySegment<Locker> queryResult = await cloudTable.ExecuteQuerySegmentedAsync(idQuery, null);
            return queryResult.ToList();
        }


        private static async Task<T> ExtractContent<T>(HttpRequestMessage request)
        {
            string connectionRequestJson = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(connectionRequestJson);
        }

        public static async Task<Locker> GetOrCreateCounter(CloudTable cloudTable, int counterId)
        {
            TableQuery<Locker> idQuery = new TableQuery<Locker>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, counterId.ToString()));

            TableQuerySegment<Locker> queryResult = await cloudTable.ExecuteQuerySegmentedAsync(idQuery, null);
            Locker cloudLocker = queryResult.FirstOrDefault();
            if (cloudLocker == null)
            {
                cloudLocker = new Locker { Id = counterId };
                TableOperation insertOperation = TableOperation.InsertOrReplace(cloudLocker);
                cloudLocker.PartitionKey = "locker";
                cloudLocker.RowKey = cloudLocker.Id.ToString();
                TableResult tableResult = await cloudTable.ExecuteAsync(insertOperation);
                return await GetOrCreateCounter(cloudTable, counterId);
            }

            return cloudLocker;
        }

        public static async Task<UserBalance> GetOrCreateBalance(CloudTable cloudTable, string user_key)
        {
            TableQuery<UserBalance> idQuery = new TableQuery<UserBalance>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, user_key));

            TableQuerySegment<UserBalance> queryResult = await cloudTable.ExecuteQuerySegmentedAsync(idQuery, null);
            UserBalance userbalance = queryResult.FirstOrDefault();
            if (userbalance == null)
            {
                userbalance = new UserBalance { user_key = user_key };
                TableOperation insertOperation = TableOperation.InsertOrReplace(userbalance);
                userbalance.PartitionKey = "user";
                userbalance.RowKey = userbalance.user_key;
                TableResult tableResult = await cloudTable.ExecuteAsync(insertOperation);
                return await GetOrCreateBalance(cloudTable, user_key);
            }
            return userbalance;
        }

        //public static void checkForTime_Elapsed(object sender, ElapsedEventArgs e, Locker locker)
        public static async void checkForTime_Elapsed(object sender, Locker locker,
            [Table("LockerRoom")] CloudTable cloudTable,
            [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("time elapsed now ! ");
            locker.available = true;
            locker.locked = true;
            locker.release_time = DateTimeOffset.Now.AddHours(0);
            locker.user_key = "";
            locker.photo_taken = false;
            TableOperation updateOperation = TableOperation.Replace(locker);
            await cloudTable.ExecuteAsync(updateOperation);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "available",
                    Arguments = new object[] { locker }
                });
        }

        public static void ScheduleRelease(Locker locker,
            [Table("LockerRoom")] CloudTable cloudTable,
            [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages ,
            ILogger log)
        {
            log.LogInformation("ScheduleRelease function . . . with " + locker.release_time.Millisecond);
            log.LogInformation("locker should be released in " + (locker.release_time-DateTimeOffset.Now).TotalMilliseconds);
            Timer checkForTime;
            checkForTime = new Timer((locker.release_time - DateTimeOffset.Now).TotalMilliseconds);
            checkForTime.Elapsed += (sender, args) => checkForTime_Elapsed(sender, locker, cloudTable, signalRMessages , log);
            checkForTime.AutoReset = false;
            checkForTime.Enabled = true;
        }

        [FunctionName("get-locker-photo")]
        public static async Task<string> GetLockerPhoto(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-locker-photo/{id}")] HttpRequestMessage request,
            int id,
            ILogger log,
             [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation("get Locker photo id -  " + id);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "TakePhoto",
                    Arguments = new object[] { id }
                });
            return "Success";
        }

        [FunctionName("send-photo-notification")]
        public static async Task<string> sendPhotoNotofication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "send-photo-notification/{id}")] HttpRequestMessage request,
            int id,
            ILogger log,
             [SignalR(HubName = "CounterHub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation("sendPhotoNotofication -  " + id);
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "photoReady",
                    Arguments = new object[] { id }
                });
            return "Success";
        }


    }
}
