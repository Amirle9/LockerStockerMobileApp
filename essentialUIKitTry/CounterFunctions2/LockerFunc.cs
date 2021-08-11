using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;





namespace CounterFunctions
{
    public static class LockerFunc
    {
        [FunctionName("LockerFunc")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "LockerFunc/{action}/{id}/{user_key}")] 
            HttpRequestMessage request, ILogger log, string action, string id, string user_key) { 
            return await call_action(log, action, id, user_key);
        }


        public static async Task<HttpResponseMessage> call_action(ILogger log, string action, string id, string user_key)
        {
            var locker = new Locker();
            locker.Id = int.Parse(id);
            locker.available = true;
            locker.locked = true;
            if (action == "set-occupy")
            {
                locker.user_key = user_key;
            }
            if (action == "set-user-key")
            {
                action = action + "/" + user_key;
            }

            if ((action == "get-locker")||(action == "get-user-key"))
            {
                action = action + "/" + id;
            }

            var BaseUri = "http://localhost:7071/api/";
            var FuncUri = BaseUri + action;

            log.LogInformation("Triggering: " + FuncUri);
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, FuncUri))
            {
                var json = JsonConvert.SerializeObject(locker);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = await client.PostAsync(FuncUri, stringContent);
                    var contents = await response.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

    }
}






