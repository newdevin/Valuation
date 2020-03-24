using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Text;
using Valuation.Service;
using Valuation.WorldTradingData.Repository.Entities;

namespace Valuation.WorldTradingData.Repository
{
    public class PicassoDbContext : DbContext
    {
        public PicassoDbContext(DbContextOptions<PicassoDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<ExchangeEntity> Exchanges { get; set; }
        public DbSet<ListingEntity> Listings { get; set; }
        public DbSet<EndOfDayPriceEntity> EndOfDayPrices { get; set; }
        public DbSet<ListingVolumeEntity> ListingVolumes { get; set; }
        public DbSet<ApiProdiverEntity> ApiProviders { get; set; }
        public DbSet<EndOfDayPriceLogEntity> EndOfDayPriceLogs { get; set; }

    }
}
