using Microsoft.AspNetCore.SignalR.Client;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using coinbox_client.Models;

namespace coinbox_client.Services;

public interface IProgramArgumentController<T>
{
    public Task ProvideProgramArguments(T args);
}

public class ProgramArgumentController : IProgramArgumentController<string[]>
{
    HttpClient _httpClient;

    public ProgramArgumentController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task ProvideProgramArguments(string[] args)
    {
        try
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
                    var response = await _httpClient.GetAsync($"http://localhost:5002/api/getCurrentNumberOfCoins");
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
                        var response = await _httpClient.PutAsync($"http://localhost:5002/api/takeCoins?count={args[1]}", null);
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
                        var response = await _httpClient.GetAsync($"http://localhost:5002/api/getChangesInCoins?countOfRecords={args[1]}");
                        var content = await response.Content.ReadFromJsonAsync<List<RecordOfChange>>();
                        if (content is not null) Console.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
                    }
                }
                else Console.WriteLine("Parameter not found.");
            }
            else Console.WriteLine("Check program arguments.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}
