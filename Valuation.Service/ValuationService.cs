using LanguageExt;
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

        public ValuationService(IEndOfDayPriceService endOfDayPriceService,
            ICurrencyRateService currencyRateService,
            IListingService listingService,
            IValuationRepository valuationRepository,
            IBuyTradeRepository buyTradeRepository,
            ISellTradeRepository sellTradeRepository
            )
        {
            this.endOfDayPriceService = endOfDayPriceService;
            this.currencyRateService = currencyRateService;
            this.listingService = listingService;
            this.valuationRepository = valuationRepository;
            this.buyTradeRepository = buyTradeRepository;
            this.sellTradeRepository = sellTradeRepository;
        }

        public async Task<PortfolioValuationSummary> GetPortfolioValuation(DateTime onDay)
        {
            var (day, previousDay)= GetCurrentAndPreviousDays(onDay);
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

        private async Task<(ValuationSummary,ValuationSummary)> GetCurrentAndPreviousSummaries(DateTime day, DateTime previousDay)
        {

            var task1 = valuationRepository.GetValuationSummaryOnDay(day);
            var task2 = valuationRepository.GetValuationSummaryOnDay(previousDay);

            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        private (DateTime , DateTime) GetCurrentAndPreviousDays(DateTime onDay)
        {
            var day = onDay.Date;
            if (onDay.IsWeekend())
            {
                day = (onDay.DayOfWeek == DayOfWeek.Saturday) ? onDay.Date.AddDays(-1) : onDay.Date.AddDays(-2);
            }
            var previousDay = day.AddDays(-1).Date;
            return (day, previousDay);
        }

        private async Task<IEnumerable<ListingValuationSummary>> GetListingValuationSummary(DateTime day, DateTime previousDay)
        {
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
                    var listing = previousListingVolume.First(l => l.Listing.Id == x.Previous.ListingId).Listing;
                    return new ListingValuationSummary
                    {
                        Listing = listing,
                        Day = day,
                        CurrentShareValue = x.Current?.First()?.ClosePrice,
                        PreviousBusinessDayShareValue = x.Previous?.ClosePrice,
                        Currency = listing.Currency
                    };
                });

            var listingValuationSummaries2 = currentPrices
                .GroupJoin(previousPrices, c => c.ListingId, p => p.ListingId,
                    (c, p) => new { Current = c, Previous = p.DefaultIfEmpty() })
                .Select(x =>
                {
                    var listing = previousListingVolume.First(l => l.Listing.Id == x.Current.ListingId).Listing;
                    return new ListingValuationSummary
                    {
                        Listing = listing,
                        Day = day,
                        CurrentShareValue = x.Current?.ClosePrice,
                        PreviousBusinessDayShareValue = x.Previous?.First()?.ClosePrice,
                        Currency = listing.Currency
                    };
                });
            var listingValuationSummaries = listingValuationSummaries1
                .Union(listingValuationSummaries2)
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

        //public async Task ValuePortfolio(DateTime upToDate)
        //{
        //    //throw new NotImplementedException();
        //    var listingVolumes = await listingService.GetListingVolumes();
        //    var firstDay = listingVolumes.Min(lv => lv.Day);

        //    var eodPrices = await endOfDayPriceService.GetEndOfDayPriceSince(firstDay);
        //    var currencyRates = await currencyRateService.GetCurencyRatesSince(firstDay);

        //    var valuations = valuationCalculator.GetValuations(upToDate, listingVolumes, eodPrices, currencyRates);
        //    var buyTrades = await buyTradeRepository.GetBuyTrades();
        //    var sellTrades = await sellTradeRepository.GetSellTrades();

        //    var summary = valuationSummaryCalculator.GetSummary(buyTrades, sellTrades, valuations);

        //    await valuationRepository.Save(valuations, summary);
        //}

        public Task ValuePortfolio()
        {
            return valuationRepository.ValuePortfolio();
        }
    }

}

