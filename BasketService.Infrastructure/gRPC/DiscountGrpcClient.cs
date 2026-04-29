using BasketService.Domain.Repository;
using Common.Core.Dtos;
using DiscountService.API.Protos;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace BasketService.Infrastructure.gRPC
{
    public class DiscountGrpcClient : IDiscountRepository
    {
        private readonly IConfiguration configuration;
        private readonly GrpcChannel channel;

        public DiscountGrpcClient(IConfiguration configuration)
        {
            this.configuration = configuration;
            channel = GrpcChannel.ForAddress(configuration["MicroservicAddress:Discount:Uri"]);
        }

        public ResultDto<DiscountDto> GetDiscountByCode(string Code)
        {
            var grpc_discountService = new DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.GetDiscountByCode(new RequestGetDiscountByCode
            {
                Code = Code
            });

            if (result.IsSuccess)
            {
                return new ResultDto<DiscountDto>
                {
                    Data = new DiscountDto
                    {
                        Amount = result.Data.Amount,
                        Code = result.Data.Code,
                        Id = Guid.Parse(result.Data.Id),
                        Used = result.Data.Used
                    },
                    IsSuccess = result.IsSuccess,
                    Message = result.Message,
                };
            }
            return new ResultDto<DiscountDto>
            {
                IsSuccess = false,
                Message = result.Message,
            };
        }

        public DiscountDto GetDiscountById(Guid DiscountId)
        {
            var grpc_discountService = new DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.GetDiscountById(new RequestGetDiscountById
            {
                Id = DiscountId.ToString(),
            });

            if (result.IsSuccess)
            {
                return new DiscountDto
                {
                    Amount = result.Data.Amount,
                    Code = result.Data.Code,
                    Id = Guid.Parse(result.Data.Id),
                    Used = result.Data.Used,
                };
            }
            return null;
        }
    }
}
