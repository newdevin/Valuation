using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Valuation.Service;

namespace Valuation.Console
{
    public class Worker : IHostedService
    {
        private readonly IEndOfDayPriceDownloadService endOfDayPriceService;
        private readonly IEndOfDayLogService endOfDayLogService;
        private readonly ICurrencyRatesDownloadService currencyRatesDownloadService;
        private readonly ICurrencyRatesLogService currencyRatesLogService;
        private readonly IValuationService valuationService;
        private readonly IValuationLogService valuationLogService;
        private readonly IPriceAlertService priceAlertService;
        private readonly IConfiguration configuration;
        private readonly INotificationService notificationService;
        private readonly ILogger<Worker> logger;
        private bool monitorPrices = false;
        private bool monitorPricesIsStopped = false;

        private readonly int quoteDelayInMinutes = 15;

        private DateTime? endOfDayPriceDownloadedDateTime;
        private DateTime? currencyRatesDownloadedDateTime;
        private DateTime? valuationRunDateTime;



        public Worker(ILogger<Worker> logger,
            IEndOfDayPriceDownloadService endOfDayPriceService,
            IEndOfDayLogService endOfDayLogService,
            ICurrencyRatesDownloadService currencyRatesDownloadService,
            ICurrencyRatesLogService currencyRatesLogService,
            IValuationService valuationService,
            IValuationLogService valuationLogService,
            IPriceAlertService priceAlertService,
            IConfiguration configuration,
            INotificationService notificationService)
        {
            this.logger = logger;
            this.endOfDayPriceService = endOfDayPriceService;
            this.endOfDayLogService = endOfDayLogService;
            this.currencyRatesDownloadService = currencyRatesDownloadService;
            this.currencyRatesLogService = currencyRatesLogService;
            this.valuationService = valuationService;
            this.valuationLogService = valuationLogService;
            this.priceAlertService = priceAlertService;
            this.configuration = configuration;
            this.notificationService = notificationService;

            if (!int.TryParse(configuration["QuoteDelayInMinutes"], out quoteDelayInMinutes))
                quoteDelayInMinutes = 15;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            monitorPrices = true;
            MonitorPrices(cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoWork(cancellationToken);
                    if (AllDownloadSucceeded() && !await ValuationRunToday())
                    {
                        await RunValuation();
                        await SendValuationNotification();
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private async Task SendValuationNotification()
        {
            var day = DateTime.Now.AddDays(-1);
            var portfolioValuation = await valuationService.GetPortfolioValuation(day);
            var subject = $"Portfolio Valuation for {day:dd MMM yyyy}";
            string message = ConsturctMessage(portfolioValuation);
            await notificationService.Send(subject, message);
            logger.LogInformation($"Portfolio valuation notification sent");
        }

        private static string ConsturctMessage(PortfolioValuation val)
        {
            return $"Total Value of portfolio : {(int)val.ValuationSummary.ValuationInGbp:n0} GBP. Total changed since day before : {(int)val.TotalValuationChange:n0} GBP.";
        }

        private void MonitorPrices(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            return;
                        if (monitorPrices == false)
                        {
                            monitorPricesIsStopped = true;
                            logger.LogInformation("Monitor prices is now PAUSED");
                            await Task.Delay(TimeSpan.FromMinutes(1));
                            continue;
                        }
                        monitorPricesIsStopped = false;
                        logger.LogInformation("Monitor prices is now ACTIVE");
                        await priceAlertService.CheckAndSendAlert();
                        monitorPricesIsStopped = true;
                        await AwaitAsync(quoteDelayInMinutes);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.ToString());
                        await AwaitAsync(quoteDelayInMinutes);
                    }
                }
            });
        }

        private async Task AwaitAsync(int minutes)
        {
            for (int i = 0; i < minutes; i++)
            {
                if (monitorPrices == false)
                    break;
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private async Task RunValuation()
        {
            logger.LogInformation("Starting daily valuation");
            var id = await valuationLogService.ValuationServiceStarted();
            await valuationService.ValuePortfolio();
            await valuationLogService.ValuationServiceCompleted(id);
            valuationRunDateTime = DateTime.Now.Date;
            logger.LogInformation("Finished daily valuation");
        }

        private async Task<bool> ValuationRunToday()
        {
            if (valuationRunDateTime.HasValue && valuationRunDateTime.Value == DateTime.Now.Date)
                return true;
            if (await valuationLogService.HasValuationServiceRunOn(DateTime.Now.Date))
            {
                valuationRunDateTime = DateTime.Now.Date;
                return true;
            }
            return false;
        }

        private bool AllDownloadSucceeded()
        {
            var today = DateTime.Now.Date;
            if (endOfDayPriceDownloadedDateTime.HasValue && endOfDayPriceDownloadedDateTime.Value.Date == today
                && currencyRatesDownloadedDateTime.HasValue && currencyRatesDownloadedDateTime.Value == today)
                return true;
            else
                return false;
        }

        private async Task DoWork(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var startTime = TimeSpan.Parse(configuration["DownloadStartTime"]);
            DateTime today = DateTime.Now;
            if (today.TimeOfDay >= startTime)
            {
                if (!await CurrencyRatesDownloadedToday())
                {
                    monitorPrices = false;
                    while (!monitorPricesIsStopped)
                    {
                        logger.LogInformation("Waiting for price monitoring to pause");
                        await Task.Delay(TimeSpan.FromSeconds(15));
                    }
                    await DownloadCurrencyRates(today);
                }

                if (!await EndOfDayPricesDownloadedToday())
                {
                    monitorPrices = false;
                    while (!monitorPricesIsStopped)
                    {
                        logger.LogInformation("Waiting for price monitoring to pause");
                        await Task.Delay(TimeSpan.FromSeconds(15));
                    }
                    await DownloadEndOfPrices(today);
                }

                monitorPrices = true;

            }
            await Task.CompletedTask;
        }

        private async Task DownloadEndOfPrices(DateTime now)
        {
            var id = await endOfDayLogService.EndOfDayPriceDownloadStarted();
            try
            {
                endOfDayPriceDownloadedDateTime = now.Date;
                logger.LogInformation("Started end of day price download");
                await endOfDayPriceService.DownloadEndOfDayPrices();
                await endOfDayLogService.EndOfDayPriceDownloadCompleted(id);
                logger.LogInformation("Finished end of day price download");
            }
            catch (Exception e)
            {
                await endOfDayLogService.SetEndOfDayDownloadToErrored(id);
                endOfDayPriceDownloadedDateTime = null;
                logger.LogError(e, "An Error has occurred while downloading end of day prices {@e}", e);
            }
        }
        private async Task DownloadCurrencyRates(DateTime now)
        {
            var id = await currencyRatesLogService.CurrencyRatesDownloadStarted();
            try
            {
                currencyRatesDownloadedDateTime = now.Date;
                logger.LogInformation("Started currency rates download");
                await currencyRatesDownloadService.DownloadCurrenctRates();
                await currencyRatesLogService.CurrencyRatesDownloadCompleted(id);
                logger.LogInformation("Finished currency rates download");
            }
            catch (Exception e)
            {
                await currencyRatesLogService.SetCurrencyRatesDownloadToErrored(id);
                currencyRatesDownloadedDateTime = null;
                logger.LogError(e, "An Error has occurred while downloading currency rates {@e}", e);
            }
        }
        private async Task<bool> EndOfDayPricesDownloadedToday()
        {
            if (endOfDayPriceDownloadedDateTime.HasValue && endOfDayPriceDownloadedDateTime.Value.Date == DateTime.Now.Date)
                return true;
            else
            {
                var hasRun = await endOfDayLogService.EndOfDayPriceDownloadHasRunOn(DateTime.Now.Date);
                if (hasRun)
                {
                    logger.LogInformation($"End Of Day Prices have already been downloaded today: {DateTime.Now.Date:yyyy-MM-dd}");
                    endOfDayPriceDownloadedDateTime = DateTime.Now.Date;
                }
                return hasRun;
            }

        }
        private async Task<bool> CurrencyRatesDownloadedToday()
        {
            if (currencyRatesDownloadedDateTime.HasValue && currencyRatesDownloadedDateTime.Value.Date == DateTime.Now.Date)
                return true;
            else
            {
                var hasRun = await currencyRatesLogService.CurrencyRatesDownloadHasRunOn(DateTime.Now.Date);
                if (hasRun)
                {
                    logger.LogInformation($"Currency Rates have already been downloaded today: {DateTime.Now.Date:yyyy-MM-dd}");
                    currencyRatesDownloadedDateTime = DateTime.Now.Date;
                }
                return hasRun;
            }

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {

            logger.LogInformation("stopping");
            return Task.CompletedTask;

        }
    }
}
