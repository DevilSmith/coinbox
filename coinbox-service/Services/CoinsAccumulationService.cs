using coinbox_service.Web.DAL.Models;
using coinbox_service.Web.DAL.Repositories;
using SignalRApp;

namespace coinbox_service.Web.Services;


public class CoinsAccumulatorService : IHostedService
{

    private readonly Random _randomizer = new Random();

    private readonly ILogger _logger;

    private readonly CoinboxServiceHub _hub;

    private readonly ICoinsRepository _coinsRepository;


    public CoinsAccumulatorService(ICoinsRepository coinsRepository, ILogger logger, CoinboxServiceHub hub)
    {
        _coinsRepository = coinsRepository;
        _logger = logger;
        _hub = hub;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {

                await Task.Delay(_randomizer.Next(1000, 10000));

                uint coinIncreasing = (uint)_randomizer.Next(10, 50);

                var countOfCoins = await _coinsRepository.GetCurrentNumberOfCoinsAsync();

                checked
                {
                    var checkOverflow = countOfCoins + coinIncreasing;
                }

                await _coinsRepository.PutCoinsAsync(coinIncreasing);

                countOfCoins = await _coinsRepository.GetCurrentNumberOfCoinsAsync();

                string message = $"Increase coins: {coinIncreasing}. Total: {countOfCoins}";

                _logger.LogInformation(message);

                await _hub.BroadcastSend("Automatic Service Increasing", message);

            }
            catch (OverflowException)
            {
                await this.StopAsync(cancellationToken);

                string message = "Overflow coins count. Coin accumulation stops.";
                await _hub.BroadcastSend("Automatic Service Increasing", message);
                _logger.LogCritical(message);
            }

        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }




}