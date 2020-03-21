using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Valuation.Console
{
    public class Worker : IHostedService
    {
        public Worker(ILogger<Worker> logger)
        {
            Logger = logger;
        }

        public ILogger<Worker> Logger { get; }
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        private TaskCompletionSource<bool> TaskCompletionSource { get; } = new TaskCompletionSource<bool>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("starting");
            Task.Run(()=> DoWork(cancellationToken));
            return TaskCompletionSource.Task;
        }

        private void DoWork(CancellationToken token)
        {
            System.Console.WriteLine("Hello World!");
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            CancellationTokenSource.Cancel();
            Logger.LogInformation("stopping");
            return TaskCompletionSource.Task;
            
        }
    }
}
