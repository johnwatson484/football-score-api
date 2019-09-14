using FootballScoreAPI.Data;
using FootballScoreAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FootballScoreAPI.Refresh
{
    class Program
    {
        private static IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            RegisterServices();

            var service = serviceProvider.GetService<IRefreshService>();

            if (args.Contains("-full"))
            {
                Console.WriteLine("Refreshing All");
                service.Refresh();
            }
            else if (args.Contains("-day"))
            {
                Console.WriteLine("Refreshing day");
                service.RefreshDay();
            }

            DisposeServices();
        }

        private static void RegisterServices()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddDbContext<FootballScoreContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FootballScoreContext")));

            services.AddScoped<IRefreshService, RefreshService>();
            services.AddScoped<IScrapingService, BBCScrapingService>();
            serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (serviceProvider == null)
            {
                return;
            }
            if (serviceProvider is IDisposable)
            {
                ((IDisposable)serviceProvider).Dispose();
            }
        }
    }
}
