using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using CounterFunctions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Xamarin.Forms;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Reflection;
using System.Net.Http.Headers;
using Plugin.LocalNotifications;
using System.Threading.Tasks;

namespace essentialUIKitTry
{
    class AzureApi
    {
        private static string BaseUri = "https://lockerfunctionapp.azurewebsites.net/api/";
        private static string GetUri = BaseUri + "get-locker/";
        private static string LockerFuncUri = BaseUri + "LockerFunc";
        private static int timeNotificationBaseId = 1000;
        private static int timeNotification5minsBeforeBaseId = 100;

        public static async System.Threading.Tasks.Task<bool> IsAvailableAsync(Int32 locker_num)
        {

            string FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/get-locker/" + locker_num;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, FuncUri))
            {
                var json = JsonConvert.SerializeObject("");
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = await client.GetStringAsync(FuncUri);
                    Locker locker = JsonConvert.DeserializeObject<Locker>(response);
                    Console.WriteLine(response);

                    return locker.available;
                }
            }
        }

        public static Locker GetLocker(Int32 locker_num)
        {
            string FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/get-locker/" + locker_num;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, FuncUri))
            {
                var json = JsonConvert.SerializeObject("");
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = client.GetStringAsync(FuncUri);
                    Locker locker = JsonConvert.DeserializeObject<Locker>(response.Result.ToString());
                    Console.WriteLine(response);

                    return locker;
                }
            }
        }

        internal static void setPhotoTaken(Locker locker)
        {
            var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-photo-taken";
            setLockerInCloud(locker, FuncUri);
        }

        public static List<Locker> GetAllLockers()
        {
            string FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/get-all-lockers";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, FuncUri))
            {
                var json = JsonConvert.SerializeObject("");
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = client.GetStringAsync(FuncUri);
                    List<Locker>  lockers = JsonConvert.DeserializeObject<List<Locker>>(response.Result.ToString());
                    Console.WriteLine(response);

                    return lockers;
                }
            }
        }
        public static double GetBalance(string user_key)
        {
            string FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/get-balance/" + user_key;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, FuncUri))
            {
                var json = JsonConvert.SerializeObject("");
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = client.GetStringAsync(FuncUri);
                    double balance = JsonConvert.DeserializeObject<double>(response.Result.ToString());
                    Console.WriteLine(response);

                    return balance;
                }
            }
        }

        public static Task<string> sendSignalToTakePhoto(int id)
        {
            Task<string> response= null;
            try
            {
                var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/get-locker-photo/" + id;
                
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, FuncUri))
                {
                    var json = JsonConvert.SerializeObject("");
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        response = client.GetStringAsync(FuncUri);
                        Console.WriteLine("######################  :" +response.Result.ToString());
                        return response;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("###################### error :" + e.Message);
            }
            return response;
        }

        public static string GetRemainingTime(Locker locker)
        {
            return ("" + (locker.release_time - DateTimeOffset.Now.AddHours(0))).Split('.')[0];
        }



          
        public static void SetOccupy(Locker locker)//Int32 locker_num, string userKey, DateTimeOffset release_time)
        {
            
           var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-occupy";
            setLockerInCloud(locker, FuncUri);
            double timeToNotify = (locker.release_time - DateTimeOffset.Now).TotalMinutes - 5;
            CrossLocalNotifications.Current.Show("Locker Stocker", "less than 5 minutes left till locker " + locker.Id + " gets released!",
                AzureApi.timeNotification5minsBeforeBaseId + locker.Id, DateTime.Now.AddMinutes(timeToNotify));
            CrossLocalNotifications.Current.Show("Locker Stocker", "locker " + locker.Id + " got released!",
                AzureApi.timeNotificationBaseId + locker.Id, DateTime.Now.AddMinutes(timeToNotify+5));


        }
        public static void SetAvailable(Locker locker)
        {
            
            var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-available";
            setLockerInCloud(locker, FuncUri);
            CrossLocalNotifications.Current.Cancel(timeNotificationBaseId + locker.Id);
            CrossLocalNotifications.Current.Cancel(timeNotification5minsBeforeBaseId + locker.Id);
        }

        public static void SetUnlock(Locker locker)
        {
            
            var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-unlock";
            setLockerInCloud(locker, FuncUri);
        }

        public static void SetUserBalance(UserBalance userBalance)
        {
            
            var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-user-balance";
            setBalanceInCloud(userBalance, FuncUri);
        }

        public static void SetLock(Locker locker)
        {
            
            var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-lock";
            setLockerInCloud(locker, FuncUri);
        }
        public static async void setLockerInCloud(Locker locker, string funcUri)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, funcUri))
                {
                    var json = JsonConvert.SerializeObject(locker);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        var response = await client.PostAsync(funcUri, stringContent);
                        var contents = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static async void setBalanceInCloud(UserBalance userBalance, string funcUri)
        {

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, funcUri))
            {
                var json = JsonConvert.SerializeObject(userBalance);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = await client.PostAsync(funcUri, stringContent);
                    var contents = await response.Content.ReadAsStringAsync();
                }
            }
        }

        public static  void SetCostsGlobally(int newCost)
        {
            int numOfRows = 4;
            for (int i = 1; i <= numOfRows; i++)
                SetCostsByRow(i, newCost);
        }
        public static  void SetCostsByRow(int rowNum, int newCost)
        {
            int rowIdx = rowNum - 1;
            int lockersInRow = 5;

            for (int lockerInRowIdx = 0; lockerInRowIdx < lockersInRow; lockerInRowIdx++)
            {
                SetCostsByLockerId(rowIdx*lockersInRow + lockerInRowIdx + 1, newCost);
                //Locker tmpLocker = await AzureApi.GetLocker(rowIdx * lockersInRow + lockerInRowIdx + 1);
                //lockerRows[rowIdx].Add(tmpLocker);
            }
        }
        public static void SetCostsByLockerId(int lockerId, int newCost)
        {
            Locker locker = AzureApi.GetLocker(lockerId);
            locker.price_per_hour = newCost;
            var FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/set-cost";
            setLockerInCloud(locker, FuncUri);
        }


        public static async void TakeLockerPhoto(string id)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountdefau8140;AccountKey=yCQK9mj77GChmhG5Ghe4cyA5ftIMiWZtm/Jg/6W8jMtBUdmoIhuLDEjllq9JCIK5o6XeNWWcfL/vOHWtNX8WKw==;EndpointSuffix=core.windows.net";
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            //Create a unique name for the container
            string containerName = "lockerphotos";
            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient("insideALocker.jpeg");
            var externalStorage = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var lockersPicturesPath = Path.Combine(externalStorage, "Pictures//LockerStocker");
            var imagePath = Path.Combine(lockersPicturesPath, "LockerStocker_" + id + ".jpeg");


            bool exists = System.IO.Directory.Exists(lockersPicturesPath);
            if (!exists) 
                System.IO.Directory.CreateDirectory(lockersPicturesPath);


           // FileStream file = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            using (FileStream file = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                blobClient.DownloadTo(file);
                file.Close();
            }


            BlobClient blobClientUpload = containerClient.GetBlobClient("LockerStocker_" + id + ".jpeg");
            //FileStream upFileStream = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            FileStream upFileStream = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            //upFileStream = File.OpenRead(imagePath);
            await blobClientUpload.UploadAsync(upFileStream, true);
            upFileStream.Close();
        }
        /*camera**/
        public static async void TakeLockerCameraPhoto(string id)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountdefau8140;AccountKey=yCQK9mj77GChmhG5Ghe4cyA5ftIMiWZtm/Jg/6W8jMtBUdmoIhuLDEjllq9JCIK5o6XeNWWcfL/vOHWtNX8WKw==;EndpointSuffix=core.windows.net";
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            //Create a unique name for the container
            string containerName = "lockerphotos";
            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // Get a reference to a blob

            BlobClient blobClient = containerClient.GetBlobClient("LockerStocker_viaCamera_" + id + ".jpeg");
            var externalStorage = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var lockersPicturesPath = Path.Combine(externalStorage, "Pictures//LockerStocker");
            var imagePath = Path.Combine(lockersPicturesPath, "LockerStocker_" + id + ".jpeg");


            bool exists = System.IO.Directory.Exists(lockersPicturesPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(lockersPicturesPath);


            // FileStream file = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            using (FileStream file = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                blobClient.DownloadTo(file);
                file.Close();
            }

        }

        public static async System.Threading.Tasks.Task<String> Negotiate()
        {
            string url = "https://lockerfunctionapp.azurewebsites.net/api/negotiate?";
            String text = "";
            try
            {

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://lockerfunctionapp.azurewebsites.net/api/negotiate?");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent("{\"UserId\":\"SomeUser\"}",
                                                    Encoding.UTF8,
                                                    "application/json");//CONTENT-TYPE header

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                System.Diagnostics.Debug.Write(" after  negotiate : *********************");
                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(" ############## catched error : " + e.Message);
            }
            return text;
        }
    }
}

