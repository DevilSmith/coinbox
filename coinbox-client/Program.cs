using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace console_client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var httpClient = httpClientFactory?.CreateClient();
            
            try
            {
                if (httpClient != null)
                {
                    if (args.Length == 0)
                    {

                        HubConnection connection;

                        connection = new HubConnectionBuilder()
                            .WithUrl("http://localhost:5002/hub")
                            .WithAutomaticReconnect()
                            .Build();

                        connection.On<string, string>("Receive", (user, message) =>
                        {
                            Console.WriteLine($"{user}: {message}");
                        });

                        await connection.StartAsync();

                        while (true) ;

                    }
                    else if (args.Length == 1)
                    {
                        if (args[0] == "get-current-number")
                        {
                            var response = await httpClient.GetAsync($"http://localhost:5002/api/getCurrentNumberOfCoins");
                            var content = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(content);
                        }
                        else Console.WriteLine("Parameter not found.");
                    }
                    else if (args.Length == 2)
                    {
                        Regex numericRegex = new(@"^\d+$");

                        if (args[0] == "take-coins")
                        {
                            if (!numericRegex.IsMatch(args[1])) Console.WriteLine("Please, set correct count of taked coins");
                            else
                            {
                                var response = await httpClient.PutAsync($"http://localhost:5002/api/takeCoins?count={args[1]}", null);
                                var content = await response.Content.ReadAsStringAsync();
                                Console.WriteLine(content);
                            }
                        }
                        else

                        if (args[0] == "get-changes")
                        {
                            if ((args[1].Length == 0) || !numericRegex.IsMatch(args[1])) Console.WriteLine("Please, set correct count of taked coins");
                            else
                            {
                                var response = await httpClient.GetAsync($"http://localhost:5002/api/getChangesInCoins?countOfRecords={args[1]}");
                                var content = await response.Content.ReadFromJsonAsync<List<RecordOfChange>>();
                                if (content is not null) Console.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
                            }
                        }
                        else Console.WriteLine("Parameter not found.");
                    }
                    else Console.WriteLine("Check program arguments.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    internal class RecordOfChange
    {
        public uint Count { get; set; }
        public DateTime CurrentDateTime { get; set; }
    }
}

