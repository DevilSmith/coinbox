using coinbox_client.Services;
using Microsoft.Extensions.Logging;
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .AddHttpClient<IProgramArgumentController<string[]>, ProgramArgumentController>();

        return services;
    }
}
