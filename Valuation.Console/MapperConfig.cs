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
            MapEndOfDayPrice();
            MapListingVolume();
        }

        private void MapListingVolume()
        {
            CreateMap<ListingVolumeEntity, ListingVolume>()
                .ConstructUsing((entity, ctx) => new ListingVolume(entity.Id, ctx.Mapper.Map<Listing>(entity.Listing), entity.Quantity, entity.Day))
                .ReverseMap();

        }

        private void MapEndOfDayPrice()
        {
            CreateMap<EndOfDayPriceEntity, Option<EndOfDayPrice>>()
                .ConstructUsing((entity, ctx) => new EndOfDayPrice(entity.Listing.Id, entity.Day, entity.Open, entity.Close, entity.High, entity.Low, entity.Volume))
                .ReverseMap();
        }

        private void MapListing()
        {
            CreateMap<ListingEntity, Option<Listing>>()
                .ConstructUsing((entity, ctx) => new Listing(entity.Id, ctx.Mapper.Map<Company>(entity.Company), ctx.Mapper.Map<Exchange>(entity.Exchange), ctx.Mapper.Map<Currency>(entity.Currency), entity.Symbol, entity.Suffix))
                .ReverseMap();
        }

        private void MapCompany()
        {
            CreateMap<CompanyEntity, Option<Company>>()
                .ConstructUsing(entity => new Company(entity.Id, entity.Name, entity.AdditionalInformation))
                .ReverseMap();
        }

        private void MapExchange()
        {
            CreateMap<ExchangeEntity, Option<Exchange>>()
                            .ConstructUsing(entity => new Exchange(entity.Symbol))
                            .ReverseMap();
        }

        private void MapCurrency()
        {
            CreateMap<CurrencyEntity, Option<Currency>>()
                            .ConstructUsing(entity => new Currency(entity.Symbol))
                            .ReverseMap();
        }
    }
}
