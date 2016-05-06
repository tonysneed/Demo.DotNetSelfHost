using System;
using System.Net.Http;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
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
                var postResponse = client.PostAsJsonAsync("", name).Result;
                postResponse.EnsureSuccessStatusCode();

                // Get greeting
                var getResponse = client.GetAsync("").Result;
                getResponse.EnsureSuccessStatusCode();
                var greeting = getResponse.Content.ReadAsAsync<string>().Result;
                Console.WriteLine($"Greeting: {greeting}");

                // Get name again
                Console.WriteLine();
                name = GetNameFromUser();
            }

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        private static string GetNameFromUser()
        {
            Console.WriteLine("Enter a name:");
            return Console.ReadLine();
        }
    }
}
