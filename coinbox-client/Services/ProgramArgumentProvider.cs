using Microsoft.AspNetCore.SignalR.Client;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using coinbox_client.Models;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace coinbox_client.Services;

public interface IProgramArgumentController<T>
{
    public Task ProvideProgramArguments(T args);
}
public class ProgramArgumentController : IProgramArgumentController<string[]>
{
    HttpClient _httpClient;
    ILogger _logger;

    public ProgramArgumentController(HttpClient httpClient, ILogger<ProgramArgumentController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task ProvideProgramArguments(string[] args)
    {
        var countOfTakedCoinsArgument = new Argument<uint>
            (
                "count",
                "Count of taked coins"
            );

        var countOfChangesArgument = new Argument<uint>
            (
                "count",
                "Count of changes"
            );

        var getCurrentNumberCommand = new Command("get-current-number", "Get current number of coins");
        var takeCoinsCommand = new Command("take-coins", "Take coins");
        var getChangesCommand = new Command("get-changes", "Get changes");
        var listenMessagesCommand = new Command("listen-messages", "Listen messages");

        takeCoinsCommand.AddArgument(countOfTakedCoinsArgument);
        getChangesCommand.AddArgument(countOfChangesArgument);

        getCurrentNumberCommand.SetHandler(async () =>
            {
                var response = await _httpClient.GetAsync($"http://localhost:5002/api/getCurrentNumberOfCoins");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(content);
            });

        takeCoinsCommand.SetHandler(async (count) =>
            {
                var response = await _httpClient.PutAsync($"http://localhost:5002/api/takeCoins?count={count}", null);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(content);
            }, countOfTakedCoinsArgument);

        getChangesCommand.SetHandler(async (count) =>
            {
                var response = await _httpClient.GetAsync($"http://localhost:5002/api/getChangesInCoins?countOfRecords={count}");
                var content = await response.Content.ReadFromJsonAsync<List<RecordOfChange>>();
                if (content is not null) Console.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
            }, countOfChangesArgument);

        listenMessagesCommand.SetHandler(async (count) =>
        {
            HubConnection connection;

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5002/hub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<string, string>("Receive", (user, message) =>
            {
                _logger.LogInformation($"{user}: {message}");
            });

            await connection.StartAsync();

            while (true) ;
        });

        var root = new RootCommand("Coinbox-client CLI application") { getCurrentNumberCommand, takeCoinsCommand, getChangesCommand, listenMessagesCommand};
        await root.InvokeAsync(args);
    }
}
