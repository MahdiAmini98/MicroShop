using AutoMapper;
using BasketService.Application.Dtos;
using BasketService.Domain.Entities;

namespace BasketService.Application.MappingProfile
{
    public class BasketMappingProfile : Profile
    {
        public BasketMappingProfile()
        {
            CreateMap<BasketItem, AddItemToBasketDto>().ReverseMap();
        }
    }
}
