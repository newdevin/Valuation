using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Text;
using Valuation.Domain;
using Valuation.Service;
using Valuation.Repository.Entities;

namespace Valuation.Repository
{
    public class PicassoDbContext : DbContext
    {
        public PicassoDbContext(DbContextOptions<PicassoDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ListingValuation>(e =>
            {
                e.ToTable("Valuation");
                e.Property(l => l.Currency).HasColumnName("ShareCurrency");
                e.Property(l => l.Price).HasColumnName("SharePrice");
                e.Property(l => l.Rate).HasColumnName("GBPCurrencyRate");
                e.HasKey(l => l.Id);
            });
            modelBuilder.Entity<CurrencyRate>().ToTable("CurrencyRate");
            modelBuilder.Entity<Currency>(c =>
            {
                c.Property(x => x.Symbol);
                c.ToTable("Currency");
                c.HasKey(p => p.Symbol);
            });
            modelBuilder.Entity<BuyTrade>(e =>
            {
                e.ToTable("BuyTrade");
            });
            modelBuilder.Entity<SellTrade>(e =>
            {
                e.ToTable("SellTrade");
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeEntity> Exchanges { get; set; }
        public DbSet<ListingEntity> Listings { get; set; }
        public DbSet<EndOfDayPriceEntity> EndOfDayPrices { get; set; }
        public DbSet<ListingVolumeEntity> ListingVolumes { get; set; }
        public DbSet<ApiProdiverEntity> ApiProviders { get; set; }
        public DbSet<EndOfDayPriceLogEntity> EndOfDayPriceLogs { get; set; }
        public DbSet<CurrencyRatesLogEntity> CurrencyRatesLogs { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<ListingValuation> Valuations { get; set; }
        public DbSet<ValuationSummary> ValuationSummaries { get; set; }
        public DbSet<BuyTrade> BuyTrades { get; set; }
        public DbSet<SellTrade> SellTrades { get; set; }

    }
}
