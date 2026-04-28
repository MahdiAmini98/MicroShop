using DiscountService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

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
