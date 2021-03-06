﻿using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valuation.Common;
using Valuation.Domain;
using Valuation.Service.Repository;

namespace Valuation.Service
{
    public class ValuationService : IValuationService
    {
        private readonly IEndOfDayPriceService endOfDayPriceService;
        private readonly ICurrencyRateService currencyRateService;
        private readonly IListingService listingService;
        private readonly IValuationRepository valuationRepository;
        private readonly IBuyTradeRepository buyTradeRepository;
        private readonly ISellTradeRepository sellTradeRepository;
        private readonly ILogger<ValuationService> _logger;

        public ValuationService(IEndOfDayPriceService endOfDayPriceService,
            ICurrencyRateService currencyRateService,
            IListingService listingService,
            IValuationRepository valuationRepository,
            IBuyTradeRepository buyTradeRepository,
            ISellTradeRepository sellTradeRepository,
            ILogger<ValuationService> logger
            )
        {
            this.endOfDayPriceService = endOfDayPriceService;
            this.currencyRateService = currencyRateService;
            this.listingService = listingService;
            this.valuationRepository = valuationRepository;
            this.buyTradeRepository = buyTradeRepository;
            this.sellTradeRepository = sellTradeRepository;
            _logger = logger;
        }

        public async Task<PortfolioValuationSummary> GetPortfolioValuation(DateTime onDay)
        {
            var (day, previousDay) = GetCurrentAndPreviousDays(onDay);
            _logger.LogInformation($"Get valuation for day:{day:yyyy-MM-dd}, previous day :{previousDay:yyyy-MM-dd}");

            var (currentSummary, previousSummary) = await GetCurrentAndPreviousSummaries(day, previousDay);

            var listingValuationSummaries = await GetListingValuationSummary(day, previousDay);
            return new PortfolioValuationSummary
            {
                ValuationSummary = currentSummary,
                ListingValuationSummaries = listingValuationSummaries,
                TotalCostChange = currentSummary.TotalCostInGbp - previousSummary.TotalCostInGbp,
                TotalProfitChange = currentSummary.TotalProfitInGbp - previousSummary.TotalProfitInGbp,
                TotalRealisedChange = currentSummary.TotalRealisedInGbp - previousSummary.TotalRealisedInGbp,
                TotalSellChanged = currentSummary.TotalSellInGbp - previousSummary.TotalSellInGbp,
                TotalValuationChange = currentSummary.ValuationInGbp - previousSummary.ValuationInGbp,
                TotalCashInvestedChange = currentSummary.TotalCashInvestedInGbp - previousSummary.TotalCashInvestedInGbp,
                TotalCashWithdrawnChange = currentSummary.TotalCashWithdrawnInGbp - previousSummary.TotalCashWithdrawnInGbp
            };

        }

        private async Task<(ValuationSummary, ValuationSummary)> GetCurrentAndPreviousSummaries(DateTime day, DateTime previousDay)
        {

            var currentSummary = await valuationRepository.GetValuationSummaryOnDay(day);
            var previousSummary = await valuationRepository.GetValuationSummaryOnDay(previousDay);

            return (currentSummary, previousSummary);
        }

        private (DateTime, DateTime) GetCurrentAndPreviousDays(DateTime onDay)
        {
            var day = onDay.Date;
            if (onDay.IsWeekend())
            {
                day = (onDay.DayOfWeek == DayOfWeek.Saturday) ? onDay.Date.AddDays(-1) : onDay.Date.AddDays(-2);
            }
            var previousDay = day.AddDays(-1).Date;
            if (previousDay.IsWeekend())
            {
                previousDay = (previousDay.DayOfWeek == DayOfWeek.Saturday) ? previousDay.Date.AddDays(-1) : previousDay.Date.AddDays(-2);
            }
            return (day, previousDay);
        }

        private async Task<IEnumerable<ListingValuationSummary>> GetListingValuationSummary(DateTime day, DateTime previousDay)
        {
            _logger.LogInformation($"Get sumamries for current:{day:yyyyMMdd} and previous:{previousDay:yyyyMMdd}");
            var currentListingVolumes = await listingService.GetActiveListingVolumesOnDay(day);
            var currentListingIds = currentListingVolumes.Select(lv => lv.Listing.Id);
            var currentPrices = await endOfDayPriceService.GetPrices(day, currentListingIds);

            var previousListingVolume = await listingService.GetActiveListingVolumesOnDay(previousDay);
            var previousListingIds = previousListingVolume.Select(lv => lv.Listing.Id);
            var previousPrices = await endOfDayPriceService.GetPrices(previousDay, previousListingIds);

            var listingValuationSummaries1 = previousPrices
                .GroupJoin(currentPrices, p => p.ListingId, c => c.ListingId,
                    (p, c) => new { Previous = p, Current = c.DefaultIfEmpty() })
                .Select(x =>
                {
                    var listing = previousListingVolume.FirstOrDefault(l => l.Listing.Id == x.Previous.ListingId)?.Listing;
                    return new ListingValuationSummary
                    {
                        Listing = listing,
                        Day = day,
                        CurrentShareValue = x.Current?.First()?.ClosePrice,
                        PreviousBusinessDayShareValue = x.Previous?.ClosePrice,
                        Currency = listing?.Currency
                    };
                })
                .Where(x=> x.Listing !=null)
                .ToList();

            var listingValuationSummaries2 = currentPrices
                .GroupJoin(previousPrices, c => c.ListingId, p => p.ListingId,
                    (c, p) => new { Current = c, Previous = p.DefaultIfEmpty() })
                .Select(x =>
                {
                    var listing = previousListingVolume.FirstOrDefault(l => l.Listing.Id == x.Current.ListingId)?.Listing;
                    return new ListingValuationSummary
                    {
                        Listing = listing,
                        Day = day,
                        CurrentShareValue = x.Current?.ClosePrice,
                        PreviousBusinessDayShareValue = x.Previous?.First()?.ClosePrice,
                        Currency = listing?.Currency
                    };
                })
                .Where(x => x.Listing != null)
                .ToList();
            var listingValuationSummaries = listingValuationSummaries1
                .Union(listingValuationSummaries2)
                .Where(x => x.Listing != null)
                .Distinct()
                .ToList();

            return listingValuationSummaries;
        }

        public Task<IEnumerable<ListingValuation>> GetValuations(DateTime day)
        {
            return valuationRepository.GetValuations(day);
        }

        public Task<IEnumerable<ValuationSummary>> GetValuationSummary()
        {
            return valuationRepository.GetValuationSummary();
        }
               
        public Task ValuePortfolio()
        {
            return valuationRepository.ValuePortfolio();
        }
    }

}

