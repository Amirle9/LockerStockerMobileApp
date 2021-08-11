using System;
using System.Text;
using System.Net.Http;

using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Azure.Storage.Blobs;

namespace Camera2Basic
{
    class AzureApi
    {
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


        public static async System.Threading.Tasks.Task<String> sendPhotoNotofication(int id)
        {
            try
            {
                string FuncUri = "https://lockerfunctionapp.azurewebsites.net/api/send-photo-notification/" + id;
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, FuncUri))
                {
                    var json = JsonConvert.SerializeObject("");
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        var response = client.GetStringAsync(FuncUri);
                        Console.WriteLine("*************************** " + response.Result.ToString());
                    }
                }
                return "Success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "False";
            }

        }

    }
}

