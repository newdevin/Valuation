using AutoMapper;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;
using Valuation.Domain;
using Valuation.WorldTradingData.Repository.Entities;

namespace Valuation.Console
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            MapCurrency();
            MapExchange();
            MapCompany();
            MapListing();
        }

        private void MapListing()
        {
            //CreateMap<ListingEntity, Option<Listing>>()
            //    .ConstructUsing((entity, ctx) => Listing.Create(entity.Id, ctx., entity.Exchange));
        }

        private void MapCompany()
        {
            CreateMap<CompanyEntity, Option<Company>>()
                .ConstructUsing(entity => Company.Create(entity.Id, entity.Name, entity.AdditionalInformation));
            CreateMap<Company, CompanyEntity>();
        }

        private void MapExchange()
        {
            CreateMap<ExchangeEntity, Option<Exchange>>()
                            .ConstructUsing(entity => Exchange.Create(entity.Symbol));
            CreateMap<Exchange, ExchangeEntity>();
        }

        private void MapCurrency()
        {
            CreateMap<CurrencyEntity, Option<Currency>>()
                            .ConstructUsing(entity => Currency.Create(entity.Symbol));
            CreateMap<Currency, CurrencyEntity>();
        }
    }
}
