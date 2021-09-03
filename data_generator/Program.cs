
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FriendsAppDataGenerator
{
    static class Program
    {
        static void Main(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient<IDataGenerator, DataGenerator>();
            });

            using IHost host = hostBuilder.Build();

            var dg = host.Services.GetRequiredService<IDataGenerator>();
            dg.Run();

        }
    }
}
