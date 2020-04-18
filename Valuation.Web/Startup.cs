using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using Valuation.Repository;
using Valuation.Repository.Mapper;
using Valuation.Service;
using Valuation.Service.Repository;
using Valuation.WorldTradingData.Service;
using IObjectMapper = Valuation.Repository.Mapper.IObjectMapper;

namespace Valuation.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Valuations", Version = "v1" });
            });
            AddApplicationServices(services);

        }
        private void AddApplicationServices(IServiceCollection services)
        {
            var conStr = Configuration.GetConnectionString("PicassoDbConnectionString");
            var uriString = Configuration["AlphaVantage"];
            int.TryParse(Configuration["delay"], out int delay);

            services.AddAutoMapper(typeof(ObjectMapper));

            services.AddDbContext<PicassoDbContext>(options => options.UseSqlServer(conStr), ServiceLifetime.Transient);

            services.AddScoped<IEndOfDayPriceRepository, EndOfDayPriceRepository>();
            services.AddScoped<IValuationLogRepository, ValuationLogRepository>();
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<IApiRepository, ApiRepository>();
            services.AddScoped<IEndOfDayLogRepository, EndOfDayLogRepository>();
            services.AddScoped<ICurrencyRatesLogRepository, CurrencyRatesLogRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
            services.AddScoped<IValuationRepository, ValuationRepository>();
            services.AddScoped<IBuyTradeRepository, BuyTradeRepository>();
            services.AddScoped<IValuationLogService, ValuatinLogService>();
            services.AddScoped<ISellTradeRepository, SellTradeRepository>();
            services.AddScoped<IEndOfDayPriceDownloadService, EndOfDayPriceDownloadService>(s =>
            {
                return new EndOfDayPriceDownloadService(s.GetService<ILogger<EndOfDayPriceDownloadService>>(), s.GetService<ITradingDataService>(),
                    s.GetService<IEndOfDayPriceRepository>(), s.GetService<IListingService>(), s.GetService<IHttpClientFactory>(), delay);
            });

            services.AddHttpClient();
            services.AddScoped<ICurrencyRatesDownloadService, CurrencyRatesDownloadService>(s =>
            {
                return new CurrencyRatesDownloadService(s.GetService<ITradingDataService>(), s.GetService<ICurrencyRateService>(),
                    s.GetService<IHttpClientFactory>(), delay);
            });
            services.AddScoped<IObjectMapper, ObjectMapper>(s => new ObjectMapper(s.GetService<IMapper>()));
            services.AddScoped<ITradingDataService, AlphaVantageDataService>(s =>
            new AlphaVantageDataService(new System.Uri(uriString), s.GetService<IApiRepository>()));
            services.AddScoped<IListingService, ListingService>();
            services.AddScoped<IEndOfDayLogService, EndOfDayLogService>();
            services.AddScoped<ICurrencyRatesLogService, CurrencyRatesLogService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ICurrencyRateService, CurrencyRateService>();
            services.AddScoped<IValuationService, ValuationService>();
            services.AddScoped<IEndOfDayPriceService, EndOfDayPriceService>();
            services.AddScoped<ValuationCalculator>();
            services.AddScoped<ValuationSummaryCalculator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Valuation API V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

        }
    }
}
