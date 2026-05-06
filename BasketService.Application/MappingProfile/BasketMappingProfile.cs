using AutoMapper;
using BasketService.Application.Dtos;
using BasketService.Domain.Entities;
using Common.EventBus.Messages.BasketToOrder;

namespace BasketService.Application.MappingProfile
{
    public class BasketMappingProfile : Profile
    {
        public BasketMappingProfile()
        {
            CreateMap<BasketItem, AddItemToBasketDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<AddItemToBasketDto, ProductDto>().ReverseMap();
            CreateMap<CheckoutBasketDto, BasketCheckoutMessage>().ReverseMap();
        }
    }
}
