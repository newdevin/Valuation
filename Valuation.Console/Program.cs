

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valuation.Domain;
using Valuation.Service;
using Valuation.WorldTradingData.Repository;
using Valuation.WorldTradingData.Service;
using AutoMapper;
namespace Valuation.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddAutoMapper(typeof(Worker));


                services.AddTransient<IEndOfDayRepository, EndOfDayRepository>();
                services.AddTransient<IEndOfDayPriceService, EndOfDayPriceService>();

            });
        
    }
}
