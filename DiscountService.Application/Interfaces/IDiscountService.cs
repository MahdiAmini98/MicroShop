using Common.Core.Dtos;

namespace DiscountService.Application.Interfaces
{
    public interface IDiscountService
    {
        DiscountDto GetDiscountByCode(string Code);
        DiscountDto GetDiscountById(Guid Id);
        bool UseDiscount(Guid Id);
        bool AddNewDiscount(string Code, int Amount);
    }
}
