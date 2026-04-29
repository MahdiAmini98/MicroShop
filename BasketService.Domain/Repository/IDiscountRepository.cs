using Common.Core.Dtos;

namespace BasketService.Domain.Repository
{
    public interface IDiscountRepository
    {
        DiscountDto GetDiscountById(Guid id);
        ResultDto<DiscountDto> GetDiscountByCode(string code);
    }
}
