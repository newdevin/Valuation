using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuation.Domain;

namespace Valuation.Service
{
    public class PriceAlertSerice : IPriceAlertService
    {
        private readonly ILogger<PriceAlertSerice> logger;
        private readonly ITargetSellPriceReachedService targetSellPriceReachedService;
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly INotificationService notificationService;
        private readonly IQuoteService quoteService;

        public PriceAlertSerice(ILogger<PriceAlertSerice> logger,
            ITargetSellPriceReachedService targetSellPriceReachedService,
            IEndOfDayPriceService endOfDayPriceService,
            INotificationService notificationService,
            IQuoteService quoteService)
        {
            this.logger = logger;
            this.targetSellPriceReachedService = targetSellPriceReachedService;
            this.endOfDayPriceService = endOfDayPriceService;
            this.notificationService = notificationService;
            this.quoteService = quoteService;
        }
        public async Task CheckAndSendAlert()
        {
            var targetSellPrices = await targetSellPriceReachedService.GetTargetSellPrices();
            var listings = targetSellPrices.Select(tp => tp.Listing).ToList();

            await CheckEodPrices(targetSellPrices, listings);
            await CheckQuotes(targetSellPrices, listings);

        }

        private async Task CheckQuotes(IEnumerable<TargetSellPrice> targetSellPrices, List<Listing> listings)
        {
            var quotes = await quoteService.GetQuotes(listings);
            quotes = quotes.Where(q => !q.IsEmpty).ToList();
            var targetReachedPrices = targetSellPrices
                .Select(tp => (tp, quotes.FirstOrDefault(p => tp.Listing.Id == p.Listing.Id)))
                .Where(it => it.Item2 != null && it.Item2.Price.HasValue && it.tp.IsTargetSellPriceReached(it.Item2.Price.Value))
                .Select(it => (it.tp, it.Item2.Price.Value))
                .ToList();

            if (targetReachedPrices.Any())
            {
                await SendNotification(targetReachedPrices);
                await targetSellPriceReachedService.SetNotified(targetReachedPrices.Select(t => t.tp.Listing.Id).Distinct());
            }
            else
                logger.LogInformation($"No target prices set or non reached the target price based on quoted price on {DateTime.Now:dd MMM yyyy HH:mm}");

        }

        private async Task CheckEodPrices(IEnumerable<TargetSellPrice> targetSellPrices, List<Listing> listings)
        {
            var listingIds = listings.Select(l => l.Id).ToList();
            var eodPrices = await endOfDayPriceService.GetLatestEndOfDayPrices(listingIds);
            var targetReachedPrices = targetSellPrices
                .Select(tp => (tp, eodPrices.FirstOrDefault(p => tp.Listing.Id == p.ListingId)))
                .Where(it => it.Item2 != null && it.tp.IsTargetSellPriceReached(it.Item2.ClosePrice.Value))
                .Select(it => (it.tp, it.Item2.ClosePrice.Value))
                .ToList();

            if (targetReachedPrices.Any())
            {
                await SendNotification(targetReachedPrices);
                await targetSellPriceReachedService.SetNotified(targetReachedPrices.Select(t => t.tp.Listing.Id).Distinct());
            }
            else
                logger.LogInformation("No target prices set or non reached the target price based on End of day price");
        }

        private async Task SendNotification(IEnumerable<(TargetSellPrice, decimal)> targetReachedPrices)
        {
            StringBuilder message = new StringBuilder();
            var subject = "Sell target price reached";
            foreach (var target in targetReachedPrices)
            {
                message.AppendLine($"{target.Item1.Listing.Symbol} has exceeded the target sell price of {target.Item1.TargetPrice}. New price is {target.Item2}");
            }
            await notificationService.Send(subject, message.ToString());
        }
    }
}
