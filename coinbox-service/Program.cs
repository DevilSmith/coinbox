using coinbox_service.Web.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using SignalRApp;
using WeatherArchiveApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.Configure<HostOptions>(x =>
{
    x.ServicesStartConcurrently = true;
    x.ServicesStopConcurrently = false;
});

builder.Services.AddApplicationServices();

var app = builder.Build();

app.MapHub<CoinboxServiceHub>("/hub");

app.MapGet("/", () => "Coinbox-service");

var coinsRepository = app.Services.GetService<ICoinsRepository>();
var hub = app.Services.GetService<CoinboxServiceHub>();

app.MapGet("/api/getCurrentNumberOfCoins", async () =>
{
    uint totalNumberOfCoins = 0;

    if (coinsRepository is not null) totalNumberOfCoins = await coinsRepository.GetCurrentNumberOfCoinsAsync();
    else { return Results.Problem(); }

    return Results.Ok(new { NumberOfCoins = totalNumberOfCoins });
});

app.MapGet("/api/getChangesInCoins", ([FromQuery(Name = "countOfRecords")] uint countOfRecords) =>
{
    if (coinsRepository is not null)
    {
        return Results.Ok(coinsRepository.GetNumbersOfCoins(countOfRecords));
    }
    else { return Results.Problem(); }
});

app.MapPut("/api/takeCoins", async ([FromQuery(Name = "count")] uint takedCoins) =>
{
    try
    {
        if ((coinsRepository is null) || (hub is null)) return Results.Problem();
        else
        {
            await coinsRepository.TakeCoinsAsync(takedCoins);

            var totalNumberOfCoins = await coinsRepository.GetCurrentNumberOfCoinsAsync();

            await hub.BroadcastSend("User", $"User taked coins. Count of taked coins: {takedCoins}");
            return Results.Ok(new { TakedCoins = takedCoins, TotalNumberOfCoins = totalNumberOfCoins });
        }
    }
    catch (CoinsRepositoryException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

using (ApplicationContext db = new ApplicationContext())
{
    db.Database.EnsureCreated();
    db.SaveChanges();
}

app.Run();

