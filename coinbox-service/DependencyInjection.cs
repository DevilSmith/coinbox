using coinbox_service.Web.DAL.Repositories;
using coinbox_service.Web.Services;
using SignalRApp;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddSingleton<CoinboxServiceHub>()
            .AddTransient<ILogger, Logger<Program>>()
            .AddSingleton<ICoinsRepository, SqliteCoinsRepository>()
            .AddHostedService<CoinsAccumulatorService>();

        return services;
    }
}
