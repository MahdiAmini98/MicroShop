
namespace BasketService.Application.Interfaces
{
    public interface IProductService
    {
        bool ProductUpdatedName(Guid ProductId, string productName);
    }
}
