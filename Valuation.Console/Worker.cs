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
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly IEndOfDayLogService endOfDayLogService;
        private readonly IConfiguration configuration;
        private readonly ILogger<Worker> logger;

        private DateTime? lastRunDateTime;

        public Worker(ILogger<Worker> logger,
            IEndOfDayPriceService endOfDayPriceService,
            IEndOfDayLogService endOfDayLogService,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.endOfDayPriceService = endOfDayPriceService;
            this.endOfDayLogService = endOfDayLogService;
            this.configuration = configuration;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);
                await Task.Delay(60_000);
            }

        }

        private async Task DoWork(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var startTime = TimeSpan.Parse(configuration["DownloadStartTime"]);
            DateTime now = DateTime.Now;
            if (now.TimeOfDay >= startTime && !await EndOfDayPricesDownloadedToday())
            {
                var id = await endOfDayLogService.Start();
                try
                {
                    lastRunDateTime = now.Date;
                    logger.LogInformation("Started end of day price download");
                    await endOfDayPriceService.DownloadEndOfDayPrices();
                    await endOfDayLogService.Complete(id);
                    logger.LogInformation("Finished end of day price download");
                }
                catch (Exception e)
                {
                    await endOfDayLogService.SetErrored(id);
                    logger.LogError(e, "An Error has occurred while downloading end of day prices");
                }
            }
            await Task.CompletedTask;

        }

        private async Task<bool> EndOfDayPricesDownloadedToday()
        {
            if (lastRunDateTime.HasValue && lastRunDateTime.Value.Date == DateTime.Now.Date)
                return true;
            else
            {
                var hasRun = await endOfDayLogService.HasRunOn(DateTime.Now.Date);
                if (hasRun)
                {
                    lastRunDateTime = DateTime.Now.Date;
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
