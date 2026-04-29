using Common.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasketService.Application.Interfaces
{
    public interface IDiscountService
    {
        DiscountDto GetDiscountById(Guid id);
        ResultDto<DiscountDto> GetDiscountByCode(string code);
    }
}
