using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Core.Dtos
{
    public record ResultRecordDto(bool IsSuccess, string? Message = null);
    public record ResultRecordDto<T>(bool IsSuccess, string? Message = null, T? Data = null) where T : class;
}
