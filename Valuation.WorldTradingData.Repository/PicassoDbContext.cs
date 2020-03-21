using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Valuation.WorldTradingData.Repository.Entities;

namespace Valuation.WorldTradingData.Repository
{
    public class PicassoDbContext : DbContext
    {
        public PicassoDbContext(DbContextOptions<PicassoDbContext> options) : base(options)
        {

        }

        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<ExchangeEntity> Exchanges { get; set; }
        public DbSet<ListingEntity> Listings { get; set; }
        public DbSet<EndOfDayPriceEntity> endOfDayPrices { get; set; }


    }
}
