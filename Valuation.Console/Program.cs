

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
using Serilog;
using Serilog.Events;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Valuation.Repository.Mapper;

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

                services.AddAutoMapper(typeof(ObjectMapper));

                var conStr = hostContext.Configuration.GetConnectionString("PicassoDbConnectionString");
                var uriString = hostContext.Configuration["AlphaVantage:baseUri"];
                var uriYahooString = hostContext.Configuration["Yahoo:baseUri"];
                int.TryParse(hostContext.Configuration["AlphaVantage:delay"], out int delay);

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
                services.AddTransient<IProviderRepository, ProviderRepository>();
                services.AddTransient<ITargetPriceRepostiory, TargetPriceRepostiory>();
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
                services.AddTransient<Repository.Mapper.IObjectMapper, ObjectMapper>(s => new ObjectMapper(s.GetService<IMapper>()));
                //services.AddTransient<ITradingDataService, AlphaVantageDataService>(s =>
                //new AlphaVantageDataService(new System.Uri(uriString), s.GetService<IApiRepository>()));
                services.AddTransient<ITradingDataService, YahooFinanceDataService>(s =>
                new YahooFinanceDataService(uriYahooString));
                services.AddTransient<IListingService, ListingService>();
                services.AddTransient<IEndOfDayLogService, EndOfDayLogService>();
                services.AddTransient<ICurrencyRatesLogService, CurrencyRatesLogService>();
                services.AddTransient<ICurrencyService, CurrencyService>();
                services.AddTransient<ICurrencyRateService, CurrencyRateService>();
                services.AddTransient<IValuationService, ValuationService>();
                services.AddTransient<IEndOfDayPriceService, EndOfDayPriceService>();
                services.AddTransient<INotificationService, EmailNotificationService>(s =>
                {
                    var smtpServer = hostContext.Configuration["Notification:smtpServerName"];
                    var serviceName = hostContext.Configuration["Notification:serviceName"];
                    var fromAddress = hostContext.Configuration["Notification:fromAddress"];
                    var fromName = hostContext.Configuration["Notification:fromName"];
                    var toAddress = hostContext.Configuration["Notification:toAddress"];
                    int.TryParse(hostContext.Configuration["Notification:port"], out int port);
                    return new EmailNotificationService(s.GetService<ILogger<EmailNotificationService>>(), s.GetService<IProviderService>(),
                        serviceName, fromAddress, fromName, toAddress, smtpServer, port);
                });
                services.AddTransient<IPriceAlertService, PriceAlertSerice>();
                services.AddTransient<ITargetPriceReachedService, TargetPriceReachedService>();
                services.AddTransient<IProviderService, ProviderService>();
                services.AddTransient<IQuoteService, AlphaVantageQuoteService>(s =>
                {
                    int.TryParse(hostContext.Configuration["AlphaVantage:QuoteDelayInMinutes"], out int quoteDelayInMinutes);
                    quoteDelayInMinutes = (quoteDelayInMinutes == 0) ? 15 : quoteDelayInMinutes;
                    return new AlphaVantageQuoteService(s.GetService<ILogger<AlphaVantageQuoteService>>(), s.GetService<ITradingDataService>(),
                        s.GetService<IHttpClientFactory>(), delay);
                });
                services.AddTransient<ValuationCalculator>();
                services.AddTransient<ValuationSummaryCalculator>();


            });

    }
}
