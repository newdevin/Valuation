using System;
using System.Collections.Generic;
using System.Linq;
using Valuation.Domain;

namespace Valuation.WorldTradingData.Service
{
    public class YahooFinanceDataService : ITradingDataService
    {
        private readonly string _baseUri;

        public YahooFinanceDataService(string baseUri)
        {
            _baseUri = baseUri;
        }
        public Uri GetCurrencyRateUri(DateTime? day, string symbol)
        {
            //https://query1.finance.yahoo.com/v7/finance/download/GBPUSD=X?period1=1580586213&period2=1612208613&interval=1d&events=history&includeAdjustedClose=true
            day ??= DateTime.Now;
            var from = (day.Value.Date - DateTime.UnixEpoch).TotalSeconds;
            var toDate = day.Value.AddYears(1);
            if (toDate > DateTime.Now.Date)
                toDate = DateTime.Now.Date;
            var to = (toDate.Date - DateTime.UnixEpoch).TotalSeconds;

            var query = $"{_baseUri}/v7/finance/download/{symbol}GBP=X?period1={from}&period2={to}&interval=1d&events=history&includeAdjustedClose=true";

            return new Uri(query);
        }
        public IEnumerable<CurrencyRate> GetRates(IEnumerable<string> data, string symbol)
        {
            return data
                   .Where(d => !string.IsNullOrWhiteSpace(d))
                   .Select(d =>
                   {
                       var p = d.Split(',', StringSplitOptions.None);
                       DateTime.TryParse(p[0], out DateTime day);
                       decimal.TryParse(p[4], out decimal rate);
                       return new CurrencyRate { From = symbol, Day = day, To = "GBP", Rate = rate };
                   })
                   .GroupBy(cr => cr.Day)
                   .Select(grp => grp.First());
        }

        public Uri GetEndOfDayPriceUri(DateTime? day, string symbol, string suffix)
        {
            //https://query1.finance.yahoo.com/v7/finance/download/RBL.AX?period1=1580557598&period2=1612179998&interval=1d&events=history//&includeAdjustedClose=true
            day ??= DateTime.Now;
            var from = (day.Value.Date - DateTime.UnixEpoch).TotalSeconds;
            var toDate = day.Value.AddYears(1);
            if (toDate > DateTime.Now.Date)
                toDate = DateTime.Now.Date;
            var to = (toDate.Date - DateTime.UnixEpoch).TotalSeconds;

            string query;
            if (!string.IsNullOrEmpty(suffix))
                suffix = $".{suffix}";

            if (toDate - day < TimeSpan.FromDays(30))
                query = $"{_baseUri}/v7/finance/download/{symbol}{suffix}?period2={to}&interval=1d&events=history&includeAdjustedClose=true";
            else
                query = $"{_baseUri}/v7/finance/download/{symbol}{suffix}?period1={from}&period2={to}&interval=1d&events=history&includeAdjustedClose=true";

            return new Uri(query);
        }

        public IEnumerable<EndOfDayPrice> GetPrices(IEnumerable<string> data, int listingId, string currency)
        {
            //Date,Open,High,Low,Close,Adj Close,Volume
            return data.Where(d => !string.IsNullOrWhiteSpace(d))
                .Select(d =>
                {
                    var p = d.Split(',', StringSplitOptions.None);
                    decimal? openPrice = null;
                    decimal? closePrice = null;
                    decimal? highPrice = null;
                    decimal? lowPrice = null;
                    int? volume = null;
                    DateTime.TryParse(p[0], out DateTime day);

                    if (decimal.TryParse(p[1], out decimal price))
                    {
                        if (currency == "GBP")
                            price /= 100;
                        openPrice = price;
                    }
                    if (decimal.TryParse(p[2], out price))
                    {
                        if (currency == "GBP")
                            price /= 100;
                        highPrice = price;
                    }
                    if (decimal.TryParse(p[3], out price))
                    {
                        if (currency == "GBP")
                            price /= 100;
                        lowPrice = price;
                    }
                    if (decimal.TryParse(p[4], out price))
                    {
                        if (currency == "GBP")
                            price /= 100;
                        closePrice = price;
                    }

                    if (int.TryParse(p[6], out int vol))
                    {
                        volume = vol;
                    }

                    return new EndOfDayPrice(listingId, day, openPrice, closePrice, highPrice, lowPrice, volume);
                })
                .Where(eod => eod.ClosePrice.HasValue)
                .GroupBy(eod => eod.Day)
                .Select(grp => grp.First());
        }

        public Quote GetQuote(string data, Listing listing)
        {
            throw new NotImplementedException();
        }

        public Uri GetQuoteUri(string symbol, string suffix)
        {
            throw new NotImplementedException();
        }


    }
}
