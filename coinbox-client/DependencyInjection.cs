using coinbox_client.Services;
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddHttpClient<IProgramArgumentController<string[]>, ProgramArgumentController>();

        return services;
    }
}
