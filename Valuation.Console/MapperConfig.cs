using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
            CreateMap<EndOfDayPriceEntity, EndOfDayPrice>()
                .ConstructUsing((entity, ctx) => new EndOfDayPrice(entity.Listing.Id, entity.Day, entity.Open, entity.Close, entity.High, entity.Low, entity.Volume))
                .ReverseMap()
                .ConvertUsing(model => new EndOfDayPriceEntity { ListingId = model.ListingId, Day = model.Day, Open = model.OpenPrice, Close = model.ClosePrice, High = model.HighPrice, Low = model.LowPrice, Volume = model.Volume });
        }

        private void MapListing()
        {
            CreateMap<ListingEntity, Listing>()
                .ConstructUsing((entity, ctx) => new Listing(entity.Id, ctx.Mapper.Map<Company>(entity.Company), ctx.Mapper.Map<Exchange>(entity.Exchange), ctx.Mapper.Map<Currency>(entity.Currency), entity.Symbol, entity.Suffix))
                .ReverseMap();

            CreateMap<IEnumerable<ListingEntity>, IEnumerable<Listing>>()
                .ConstructUsing((entities, ctx) =>  entities.Select(entity=> new Listing(entity.Id, ctx.Mapper.Map<Company>(entity.Company), ctx.Mapper.Map<Exchange>(entity.Exchange), ctx.Mapper.Map<Currency>(entity.Currency), entity.Symbol, entity.Suffix)))
                .ReverseMap();
        }

        private void MapCompany()
        {
            CreateMap<CompanyEntity, Company>()
                .ConstructUsing(entity => new Company(entity.Id, entity.Name, entity.AdditionalInformation))
                .ReverseMap();
        }

        private void MapExchange()
        {
            CreateMap<ExchangeEntity, Exchange>()
                            .ConstructUsing(entity => new Exchange(entity.Symbol))
                            .ReverseMap();
        }

        private void MapCurrency()
        {
            CreateMap<CurrencyEntity, Currency>()
                            .ConstructUsing(entity => new Currency(entity.Symbol))
                            .ReverseMap();
        }
    }
}
