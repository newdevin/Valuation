using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valuation.Service
{
    public class PriceAlertSerice : IPriceAlertService
    {
        private readonly ILogger<PriceAlertSerice> logger;
        private readonly ITargetSellPriceReachedService targetSellPriceReachedService;
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly INotificationService notificationService;

        public PriceAlertSerice(ILogger<PriceAlertSerice> logger, 
            ITargetSellPriceReachedService targetSellPriceReachedService, 
            IEndOfDayPriceService endOfDayPriceService,
            INotificationService notificationService)
        {
            this.logger = logger;
            this.targetSellPriceReachedService = targetSellPriceReachedService;
            this.endOfDayPriceService = endOfDayPriceService;
            this.notificationService = notificationService;
        }
        public async Task CheckAndSendAlert()
        {
            var targetSellPrices = await targetSellPriceReachedService.GetTargetSellPrices();
            var listingIds = targetSellPrices.Select(tp => tp.Listing.Id).ToList();
            var currentPrices = await endOfDayPriceService.GetLatestEndOfDayPrices(listingIds);

            var targetReachedPrices =  targetSellPrices.Where(tp =>
            {
                var eodPrice = currentPrices.FirstOrDefault(p => tp.Listing.Id == p.ListingId);
                return eodPrice != null && tp.IsTargetSellPriceReached(eodPrice.ClosePrice.Value);
            })
            .ToList();

            if (targetReachedPrices.Any())
            {
                await SendNotification(targetReachedPrices);
                await targetSellPriceReachedService.SetNotified(targetReachedPrices.Select(t => t.Listing.Id).Distinct());
            }
            else
                logger.LogInformation("No target prices set or non reached the target price");
            
        }

        private async Task SendNotification(IEnumerable<TargetSellPrice> targetReachedPrices)
        {
            StringBuilder message = new StringBuilder();
            var subject = "Sell target price reached";
            foreach(var target in targetReachedPrices)
            {
                message.AppendLine($"{target.Listing.Symbol} has exceeded the target sell price of {target.TargetPrice}");
            }
            await notificationService.Send(subject, message.ToString());
        }
    }
}
