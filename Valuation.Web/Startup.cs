using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Valuation.Infrastructure;
using Valuation.Repository;
using Valuation.Service;
using Valuation.Service.Repository;
using Valuation.WorldTradingData.Service;
using IObjectMapper = Valuation.Infrastructure.IObjectMapper;

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
            var uriString = Configuration["WorldTradingDataUri"];

            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<PicassoDbContext>(options => options.UseSqlServer(conStr), ServiceLifetime.Transient);

            services.AddScoped<IEndOfDayPriceRepository, EndOfDayPriceRepository>();
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<IWorldTradingDataRepository, WorldTradingDataRepository>();
            services.AddScoped<IEndOfDayLogRepository, EndOfDayLogRepository>();
            services.AddScoped<ICurrencyRatesLogRepository, CurrencyRatesLogRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
            services.AddScoped<IValuationRepository, ValuationRepository>();
            services.AddScoped<IBuyTradeRepository, BuyTradeRepository>();
            services.AddScoped<ISellTradeRepository, SellTradeRepository>();

            services.AddHttpClient();
            services.AddScoped<IEndOfDayPriceDownloadService, EndOfDayPriceDownloadService>();
            services.AddScoped<ICurrencyRatesDownloadService, CurrencyRatesDownloadService>();
            services.AddScoped<IObjectMapper, ObjectMapper>(s => new ObjectMapper(s.GetService<IMapper>()));
            services.AddScoped<IWorldTradingDataService, WorldTradingDataService>(s =>
            new WorldTradingDataService(new System.Uri(uriString), s.GetService<IWorldTradingDataRepository>()));
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
