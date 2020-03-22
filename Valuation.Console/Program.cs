

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valuation.Domain;
using Valuation.Service;
using Valuation.WorldTradingData.Repository;
using Valuation.WorldTradingData.Service;
using AutoMapper;
using Valuation.Service.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Valuation.Infrastructure;
using IObjectMapper = Valuation.Infrastructure.IObjectMapper;

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
                var conStr = hostContext.Configuration.GetConnectionString("PicassoDbConnectionString");
                services.AddDbContext<PicassoDbContext>(options => options.UseSqlServer(conStr));
                
                services.AddTransient<IEndOfDayRepository, EndOfDayRepository>( s => 
                    new EndOfDayRepository(s.GetService<PicassoDbContext>(), s.GetService<IObjectMapper>()));
                services.AddTransient<IListingRepository, ListingRepository>(s => 
                    new ListingRepository(s.GetService<PicassoDbContext>(), s.GetService<IObjectMapper>()));

                services.AddHttpClient();
                services.AddTransient<IEndOfDayPriceService, EndOfDayPriceService>();
                services.AddTransient<IObjectMapper, ObjectMapper>(s => new ObjectMapper(s.GetService<IMapper>()));
                services.AddTransient<IWorldTradingDataService, WorldTradingDataService>();
                services.AddTransient<IListingService, ListingService>();

            });
        
    }
}
