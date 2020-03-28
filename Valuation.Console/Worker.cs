using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly IConfiguration configuration;
        private readonly ILogger<Worker> logger;

        private DateTime? endOfDayPriceDownloadedDateTime;
        private DateTime? currencyRatesDownloadedDateTime;

        public Worker(ILogger<Worker> logger,
            IEndOfDayPriceDownloadService endOfDayPriceService,
            IEndOfDayLogService endOfDayLogService,
            ICurrencyRatesDownloadService currencyRatesDownloadService,
            ICurrencyRatesLogService currencyRatesLogService,
            IValuationService valuationService,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.endOfDayPriceService = endOfDayPriceService;
            this.endOfDayLogService = endOfDayLogService;
            this.currencyRatesDownloadService = currencyRatesDownloadService;
            this.currencyRatesLogService = currencyRatesLogService;
            this.valuationService = valuationService;
            this.configuration = configuration;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);
                if (AllDownloadSucceeded())
                {
                    await valuationService.ValuePortfolio(DateTime.Now.AddDays(-1));
                    await Task.Delay(TimeSpan.FromHours(24));
                }
                else
                    await Task.Delay(60_000);
            }
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
                var downloadEndOfDayPricesTask = Task.Run(async () =>
                {
                    if (!await EndOfDayPricesDownloadedToday().ConfigureAwait(false))
                        await DownloadEndOfPrices(today).ConfigureAwait(false);
                });
                var currencyRatesDownloadTask = Task.Run(async () =>
                {
                    if (!await CurrencyRatesDownloadedToday().ConfigureAwait(false))
                        await DownloadCurrencyRates(today).ConfigureAwait(false);
                });

                var tasks = new[] { downloadEndOfDayPricesTask, currencyRatesDownloadTask };
                await Task.WhenAll(tasks);
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
