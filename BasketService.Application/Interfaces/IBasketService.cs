using BasketService.Application.Dtos;
using Common.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Application.Interfaces
{
    public interface IBasketService
    {
        BasketDto GetOrCreateBasketForUser(string UserId);
        BasketDto GetBasket(string UserId);
        void AddItemToBasket(AddItemToBasketDto item);
        void RemoveItemFromBasket(Guid ItemId);
        void SetQuantities(Guid itemId, int quantity);
        void TransferBasket(string anonymousId, string UserId);
        void ApplyDiscountToBasket(Guid BasketId, Guid DiscountId);
        ResultDto CheckoutBasket(CheckoutBasketDto checkoutBasket, IDiscountService discountService);

    }
}
