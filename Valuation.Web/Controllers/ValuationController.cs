using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Valuation.Service;

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
        [Route("summary")]
        public async Task<ActionResult> GetValuationSummary()
        {
            var summary = await valuationService.GetValuationSummary();
            return Ok(summary);
        }
    }
}