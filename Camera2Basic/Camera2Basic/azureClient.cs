
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace Camera2Basic
{
    class azureClient
    {
        private static readonly HttpClient client = new HttpClient();
        private HubConnection connection;
        private static azureClient instance = null ;
        public static int LockerId = 0;

        private azureClient() {
            // ConfigSignalR();
        }
        public static azureClient getInstance() {
            if (instance == null) {
                instance = new azureClient(); 
            }
            return instance;
        }
        public async Task<HubConnection> getConnection() {
            return this.connection;
        }
        async void ConfigSignalR()
        {
            var results = await AzureApi.Negotiate();
            JObject json = JObject.Parse(results);
            var url = json["url"].ToString();
            var token = json["accessToken"].ToString();

            connection = new HubConnectionBuilder().WithUrl(url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
             .Build();

          


            try
            {
                await connection.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}