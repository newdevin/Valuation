using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public class PriceAlertSerice : IPriceAlertService
    {
        private readonly ILogger<PriceAlertSerice> logger;
        private readonly ITargetPriceReachedService targetPriceReachedService;
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly INotificationService notificationService;
        private readonly IQuoteService quoteService;

        public PriceAlertSerice(ILogger<PriceAlertSerice> logger,
            ITargetPriceReachedService targetPriceReachedService,
            IEndOfDayPriceService endOfDayPriceService,
            INotificationService notificationService,
            IQuoteService quoteService)
        {
            this.logger = logger;
            this.targetPriceReachedService = targetPriceReachedService;
            this.endOfDayPriceService = endOfDayPriceService;
            this.notificationService = notificationService;
            this.quoteService = quoteService;
        }
        public async Task CheckAndSendAlert()
        {
            var targetPrices = await targetPriceReachedService.GetTargetPrices();
            var listings = targetPrices.Select(tp => tp.Listing).ToList();

            await CheckEodPrices(targetPrices, listings);
            await CheckQuotes(targetPrices, listings);

        }

        private async Task CheckQuotes(IEnumerable<TargetPrice> targetPrices, List<Listing> listings)
        {
            var quotes = await quoteService.GetQuotes(listings);
            quotes = quotes.Where(q => !q.IsEmpty).ToList();
            var targetReachedPrices = targetPrices
                .Select(tp => (tp, quotes.FirstOrDefault(p => tp.Listing.Id == p.Listing.Id)))
                .Where(it => it.Item2 != null && it.Item2.Price.HasValue && it.tp.IsTargetPriceReached(it.Item2.Price.Value))
                .Select(it => (it.tp, it.Item2))
                .ToList();

            if (targetReachedPrices.Any())
            {
                await SendNotification(targetReachedPrices.Select(x => (x.tp, x.Item2.Price.Value, x.Item2.Day.Value)));
                await targetPriceReachedService.SetNotified(targetReachedPrices.Select(t => t.tp.Listing.Id).Distinct());
            }
            else
                logger.LogInformation($"No target prices set or non reached the target price based on quoted price on {DateTime.Now:dd MMM yyyy HH:mm}");

        }

        private async Task CheckEodPrices(IEnumerable<TargetPrice> targetPrices, List<Listing> listings)
        {
            var listingIds = listings.Select(l => l.Id).ToList();
            var eodPrices = await endOfDayPriceService.GetLatestEndOfDayPrices(listingIds);
            var targetReachedPrices = targetPrices
                .Select(tp => (tp, eodPrices.FirstOrDefault(p => tp.Listing.Id == p.ListingId)))
                .Where(it => it.Item2 != null && it.tp.IsTargetPriceReached(it.Item2.ClosePrice.Value))
                .Select(it => (it.tp, it.Item2))
                .ToList();

            if (targetReachedPrices.Any())
            {
                await SendNotification(targetReachedPrices.Select(x=> (x.tp,x.Item2.ClosePrice.Value, x.Item2.Day)));
                await targetPriceReachedService.SetNotified(targetReachedPrices.Select(t => t.tp.Listing.Id).Distinct());
            }
            else
                logger.LogInformation("No target prices set or non reached the target price based on End of day price");
        }

        private async Task SendNotification(IEnumerable<(TargetPrice, decimal, DateTime)> targetReachedPrices)
        {
            StringBuilder message = new StringBuilder();
            var subject = "Target price reached";
            foreach (var target in targetReachedPrices)
            {
                message.AppendLine($"DateAndTime:{target.Item3:dd MMM yyyy HH:mm:ss} {target.Item1.TargetType}: {target.Item1.Listing.Symbol} has met the target price of {target.Item1.Target}. New price is {target.Item2}.");
            }
            await notificationService.Send(subject, message.ToString());
        }
    }
}
