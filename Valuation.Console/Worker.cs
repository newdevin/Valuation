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
        private readonly ILogger<Worker> logger;

        public Worker(ILogger<Worker> logger, IEndOfDayPriceService endOfDayPriceService)
        {
            this.logger = logger;
            this.endOfDayPriceService = endOfDayPriceService;
        }

        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        private TaskCompletionSource<bool> TaskCompletionSource { get; } = new TaskCompletionSource<bool>();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("starting");
            await DoWork(cancellationToken);
        }

        private async Task DoWork(CancellationToken token)
        {
            System.Console.WriteLine("Hello World!");
            if (!token.IsCancellationRequested)
                await endOfDayPriceService.DownloadEndOfDayPrices(DateTime.Now.Date);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            CancellationTokenSource.Cancel();
            logger.LogInformation("stopping");
            return TaskCompletionSource.Task;

        }
    }
}
