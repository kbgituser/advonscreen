using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Starter
{
    public class WSClient
    {
        HttpClient client;
        public WSClient()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) => { return true; };
            client = new HttpClient(httpClientHandler);
        }

        public T Get<T>(string url)
        {
            //Task<T> task = Task.Run<T>(() => GetAsycn<T>(url));

            Task<string> task = Task.Run<string>(() => GetAsycn(url));
            string json = task.Result;
            //string json = GetAsycn(url).GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(json))
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            return default(T);
        }

        public async Task<string> GetAsycn(string url)
        {
            string json = "";
            //client.Timeout = TimeSpan.FromMilliseconds(10000); //adjust based on your network

            //var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            // Get the CancellationToken
            //var cancellationToken = cancellationTokenSource.Token;

            // Now we will start two tasks almost at the same time :
            // - A first task which runs 3seconds before it cancels the token
            // - A second tasks calling GetAsync with the cancellation token.
            //Task.Run(async () =>
            //{
            //    await Task.Delay(1000);
            //    cancellationTokenSource.Cancel();
            //});

            try
            {
                //var response = await client.GetAsync(url);
                //await client.GetStringAsync(url);

                    //HttpResponseMessage response = await client.GetAsync(url,cancellationToken);
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Соединение с сетью недоступно. Проверьте соединение.");
                    }
                    json = await response.Content.ReadAsStringAsync();

                //if (!string.IsNullOrEmpty(json))
                //    {
                //        var result = JsonConvert.DeserializeObject<T>(json);                        
                //        return result;
                //    }                    
                return json;
                //return default(T);
            }            

            catch (TaskCanceledException cancelException)
            {
                // Do something here ?
                Console.WriteLine("cancelException");
                return json;
            }

            catch (TimeoutException e)
            {
                // handle somehow                
                Console.WriteLine("TimeoutException");
                return json;
                //return default(T);
            }
            catch (HttpRequestException e)
            {
                //throw new HttpRequestException("Соединение с сетью недоступно. Проверьте соединение.");
                Console.WriteLine("HttpRequestException");
                return json;
                //return default(T);

            }
            catch (Exception e)
            {
                Console.WriteLine("General exc");
                return json;
                //return default(T);
            }
        }
    }
}
