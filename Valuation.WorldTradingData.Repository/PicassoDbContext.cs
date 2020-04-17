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

            //modelBuilder.Entity<Currency>(c =>
            //{
            //    c.Property(c => c.Symbol);
            //    c.HasKey(p => p.Symbol);
            //})
            //.Entity<Exchange>(e =>
            //{
            //    e.Property(e => e.Symbol);
            //    e.HasKey(p => p.Symbol);
            //})
            //.Entity<Company>(e =>
            //{
            //    e.Property(e => e.AdditionalInformation);
            //    e.Property(e => e.Name);
            //    e.Property(e => e.Id);

            //})
            modelBuilder.Entity<CurrencyRate>(e =>
            {
                e.Property(e => e.Id);
                e.Property(e => e.From);
                e.Property(e => e.To);
                e.Property(e => e.Day);
                e.Property(e => e.Rate);

            })
            .Entity<ListingValuation>(e =>
            {
                e.ToTable("Valuation");
                e.Property(l => l.Currency).HasColumnName("ShareCurrency");
                e.Property(l => l.Price).HasColumnName("SharePrice");
                e.Property(l => l.Rate).HasColumnName("GBPCurrencyRate");
                e.HasKey(l => l.Id);
            })
            .Entity<ListingValuation>(e =>
            {
                e.ToTable("Valuation");
            })
            .Entity<Listing>(l =>
            {
                l.Property(l => l.Id);
                l.Property(l => l.Symbol);
                l.Property(l => l.Suffix);
                // l.Property(l => l.Company);
                //l.Property(l => l.Currency);
                //l.Property(l => l.Exchange);
            });

            var l = modelBuilder.Entity<Listing>();
            l.OwnsOne(l => l.Company).HasKey(c => c.Id);
            l.OwnsOne(l => l.Currency).HasKey(c => c.Symbol);
            l.OwnsOne(l => l.Exchange).HasKey(e => e.Symbol);
            //l.HasOne(l => l.Company).WithMany().IsRequired().HasForeignKey("CompanyId");
            //l.HasOne(l => l.Currency).WithMany().IsRequired().HasForeignKey("CurrencySymbol");
            //l.HasOne(l => l.Exchange).WithMany().IsRequired().HasForeignKey("ExchangeSymbol");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Listing> Listings { get; set; }
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

        public DbSet<ValuationLogEntity> ValuationLogs { get; set; }

    }
}
