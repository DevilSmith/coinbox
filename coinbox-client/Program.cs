using Microsoft.Extensions.DependencyInjection;
using coinbox_client.Services;

namespace console_client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddApplicationServices();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var argumentController = serviceProvider.GetService<IProgramArgumentController<string[]>>();

            if (argumentController is not null)
                await argumentController.ProvideProgramArguments(args);

        }
    }

    internal class RecordOfChange
    {
        public uint Count { get; set; }
        public DateTime CurrentDateTime { get; set; }
    }
}

