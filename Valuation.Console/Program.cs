

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valuation.Domain;
using Valuation.Service;
using Valuation.Repository;
using Valuation.WorldTradingData.Service;
using AutoMapper;
using Valuation.Service.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Valuation.Infrastructure;
using IObjectMapper = Valuation.Infrastructure.IObjectMapper;
using Serilog;
using Serilog.Events;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Valuation.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureLogging(builder =>
            {
                var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var logFile = Path.Combine(path, "Valuation.Log");
                var logConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logFile, LogEventLevel.Information, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5_000_000, retainedFileCountLimit: 5)
                .CreateLogger();

                builder.AddSerilog(logConfiguration, true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();

                services.AddAutoMapper(typeof(Worker));

                var conStr = hostContext.Configuration.GetConnectionString("PicassoDbConnectionString");
                var uriString = hostContext.Configuration["AlphaVantage"];
                int.TryParse(hostContext.Configuration["delay"], out int delay);

                services.AddDbContext<PicassoDbContext>(options => options.UseSqlServer(conStr), ServiceLifetime.Transient);

                services.AddTransient<IEndOfDayPriceRepository, EndOfDayPriceRepository>();
                services.AddTransient<IValuationLogRepository, ValuationLogRepository>();
                services.AddTransient<IListingRepository, ListingRepository>();
                services.AddTransient<IApiRepository, ApiRepository>();
                services.AddTransient<IEndOfDayLogRepository, EndOfDayLogRepository>();
                services.AddTransient<ICurrencyRatesLogRepository, CurrencyRatesLogRepository>();
                services.AddTransient<ICurrencyRateRepository, CurrencyRateRepository>();
                services.AddTransient<IValuationRepository, ValuationRepository>();
                services.AddTransient<IBuyTradeRepository, BuyTradeRepository>();
                services.AddTransient<ISellTradeRepository, SellTradeRepository>();
                services.AddTransient<IValuationLogService, ValuatinLogService>();
                services.AddTransient<IEndOfDayPriceDownloadService, EndOfDayPriceDownloadService>(s =>
                {
                    return new EndOfDayPriceDownloadService(s.GetService<ILogger<EndOfDayPriceDownloadService>>(), s.GetService<ITradingDataService>(),
                        s.GetService<IEndOfDayPriceRepository>(), s.GetService<IListingService>(), s.GetService<IHttpClientFactory>(), delay);
                });
                services.AddHttpClient();
                services.AddTransient<ICurrencyRatesDownloadService, CurrencyRatesDownloadService>(s =>
                {
                    return new CurrencyRatesDownloadService(s.GetService<ITradingDataService>(), s.GetService<ICurrencyRateService>(),
                        s.GetService<IHttpClientFactory>(), delay);
                });
                services.AddTransient<IObjectMapper, ObjectMapper>(s => new ObjectMapper(s.GetService<IMapper>()));
                services.AddTransient<ITradingDataService, AlphaVantageDataService>(s =>
                new AlphaVantageDataService(new System.Uri(uriString), s.GetService<IApiRepository>()));
                services.AddTransient<IListingService, ListingService>();
                services.AddTransient<IEndOfDayLogService, EndOfDayLogService>();
                services.AddTransient<ICurrencyRatesLogService, CurrencyRatesLogService>();
                services.AddTransient<ICurrencyService, CurrencyService>();
                services.AddTransient<ICurrencyRateService, CurrencyRateService>();
                services.AddTransient<IValuationService, ValuationService>();
                services.AddTransient<IEndOfDayPriceService, EndOfDayPriceService>();
                services.AddTransient<ValuationCalculator>();
                services.AddTransient<ValuationSummaryCalculator>();


            });

    }
}
