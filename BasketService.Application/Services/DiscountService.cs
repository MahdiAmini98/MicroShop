using BasketService.Application.Interfaces;
using Common.Core.Dtos;
using BasketService.Domain.Repository;

namespace BasketService.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            this.discountRepository = discountRepository;
        }

        public ResultDto<DiscountDto> GetDiscountByCode(string code)
        {
            return discountRepository.GetDiscountByCode(code); 
        }

        public DiscountDto GetDiscountById(Guid id)
        {
           return discountRepository.GetDiscountById(id);
        }
    }
}
