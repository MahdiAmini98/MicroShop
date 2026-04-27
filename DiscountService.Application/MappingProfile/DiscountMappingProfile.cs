using AutoMapper;
using DiscountService.Application.Dtos;
using DiscountService.Domain.Entities;

namespace DiscountService.Application.MappingProfile
{
    public class DiscountMappingProfile : Profile
    {
        public DiscountMappingProfile()
        {
            CreateMap<DiscountCode, DiscountDto>().ReverseMap();
        }
    }
}
