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
        private readonly ValuationCalculator valuationCalculator;
        private readonly ValuationSummaryCalculator valuationSummaryCalculator;

        public ValuationService(IEndOfDayPriceService endOfDayPriceService,
            ICurrencyRateService currencyRateService,
            IListingService listingService,
            IValuationRepository valuationRepository,
            IBuyTradeRepository buyTradeRepository,
            ISellTradeRepository sellTradeRepository,
            ValuationCalculator valuationCalculator,
            ValuationSummaryCalculator valuationSummaryCalculator
            )
        {
            this.endOfDayPriceService = endOfDayPriceService;
            this.currencyRateService = currencyRateService;
            this.listingService = listingService;
            this.valuationRepository = valuationRepository;
            this.buyTradeRepository = buyTradeRepository;
            this.sellTradeRepository = sellTradeRepository;
            this.valuationCalculator = valuationCalculator;
            this.valuationSummaryCalculator = valuationSummaryCalculator;
        }

        public Task<PortfolioValuationSummary> GetPortfolioValuation(DateTime day)
        {
            return valuationRepository.GetPortfolioValuation(day);
        }

        public Task<IEnumerable<ListingValuation>> GetValuations(DateTime day)
        {
            return valuationRepository.GetValuations(day);
        }

        public Task<IEnumerable<ValuationSummary>> GetValuationSummary()
        {
            return valuationRepository.GetValuationSummary();
        }

        public async Task ValuePortfolio(DateTime upToDate)
        {
            //throw new NotImplementedException();
            var listingVolumes = await listingService.GetListingVolumes();
            var firstDay = listingVolumes.Min(lv => lv.Day);

            var eodPrices = await endOfDayPriceService.GetEndOfDayPriceSince(firstDay);
            var currencyRates = await currencyRateService.GetCurencyRatesSince(firstDay);

            var valuations = valuationCalculator.GetValuations(upToDate, listingVolumes, eodPrices, currencyRates);
            var buyTrades = await buyTradeRepository.GetBuyTrades();
            var sellTrades = await sellTradeRepository.GetSellTrades();

            var summary = valuationSummaryCalculator.GetSummary(buyTrades, sellTrades, valuations);

            await valuationRepository.Save(valuations, summary);
        }

        public Task ValuePortfolio()
        {
            return valuationRepository.ValuePortfolio();
        }
    }

}

