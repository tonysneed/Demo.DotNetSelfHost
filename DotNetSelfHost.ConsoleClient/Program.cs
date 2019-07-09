using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetSelfHost.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create http client
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/hello/"),
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Get name from the user
            string name = GetNameFromUser();

            while (name?.Length > 0)
            {
                // Set greeting
                var content = new StringContent(JsonConvert.SerializeObject(name), Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("", content);
                postResponse.EnsureSuccessStatusCode();

                // Get greeting
                var getResponse = client.GetAsync("").Result;
                getResponse.EnsureSuccessStatusCode();
                var greeting = getResponse.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Greeting: {greeting}");

                // Get name again
                Console.WriteLine();
                name = GetNameFromUser();
            }
        }

        private static string GetNameFromUser()
        {
            Console.WriteLine("Enter a name:");
            return Console.ReadLine();
        }
    }
}
