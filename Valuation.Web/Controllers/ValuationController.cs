using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Valuation.Common;
using Valuation.Service;
using Valuation.Web.Models;

namespace Valuation.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuationController : ControllerBase
    {
        private readonly IValuationService valuationService;

        public ValuationController(IValuationService valuationService)
        {
            this.valuationService = valuationService;
        }

        [HttpGet]
        [Route("summary/{interval}")]
        public async Task<ActionResult> GetValuationSummary(string interval)
        {
            var vals = await valuationService.GetValuationSummary();

            Summary summary = GetSummary(interval, vals);

            return Ok(summary);
        }

        private static Summary GetSummary(string interval, IEnumerable<ValuationSummary> vals)
        {
            interval ??= "1M";
            var maxDate = DateTime.Now.Date;
            var minDate = maxDate.AddDays(-7);
            if (interval == "1W")
                minDate = maxDate.AddDays(-7);
            else if (interval == "1M")
                minDate = maxDate.AddMonths(-1);
            else if (interval == "3M")
                minDate = maxDate.AddMonths(-3);
            else if (interval == "6M")
                minDate = maxDate.AddMonths(-6);
            else if (interval == "1Y")
                minDate = maxDate.AddYears(-1);
            else if (interval == "3Y")
                minDate = maxDate.AddYears(-3);
            else if (interval == "5Y")
                minDate = maxDate.AddYears(-5);
            else
                minDate = vals.Min(v => v.Day);

            var range = DateTimeExtensions.GetDateRange(minDate, maxDate);
            var summary = new Summary
            {
                TimeSeries = range.OrderBy(r => r).ToArray()
            };
            summary.Valuation = vals.Where(v => summary.TimeSeries.Any(t => t == v.Day)).OrderBy(v=>v.Day).Select(v => (int)v.ValuationInGbp).ToArray();
            summary.Cost = vals.Where(v => summary.TimeSeries.Any(t => t == v.Day)).OrderBy(v => v.Day).Select(v => (int)v.TotalCostInGbp).ToArray();
            summary.Sold = vals.Where(v => summary.TimeSeries.Any(t => t == v.Day)).OrderBy(v => v.Day).Select(v => (int)v.TotalSellInGbp).ToArray();
            summary.Profit = vals.Where(v => summary.TimeSeries.Any(t => t == v.Day)).OrderBy(v => v.Day).Select(v => (int)v.TotalProfitInGbp).ToArray();
            summary.Realised = vals.Where(v => summary.TimeSeries.Any(t => t == v.Day)).OrderBy(v => v.Day).Select(v => (int)v.TotalRealisedInGbp).ToArray();
            return summary;
        }
    }
}