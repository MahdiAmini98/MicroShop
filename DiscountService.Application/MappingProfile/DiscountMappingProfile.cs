using AutoMapper;
using Common.Core.Dtos;
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
