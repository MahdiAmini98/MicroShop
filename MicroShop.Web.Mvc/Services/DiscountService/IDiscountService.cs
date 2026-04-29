using Common.Core.Dtos;
using DiscountService.API.Protos;
using Grpc.Net.Client;

namespace MicroShop.Web.Mvc.Services.DiscountService
{
    public interface IDiscountService
    {
        ResultDto<DiscountDto> GetDiscountByCode(string Code);
        ResultDto<DiscountDto> GetDiscountById(Guid Id);
        ResultDto UseDiscount(Guid DiscountId);
    }

    public class DiscountService : IDiscountService
    {
        private readonly GrpcChannel channel;
        private readonly IConfiguration configuration;

        public DiscountService(IConfiguration configuration)
        {
            this.configuration = configuration;
            string discountServer = configuration["MicroservicAddress:Discount:Uri"];

            channel = GrpcChannel.ForAddress(discountServer);
        }

        public ResultDto<DiscountDto> GetDiscountByCode(string Code)
        {
            var grpc_discountService = new
                DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.GetDiscountByCode(new RequestGetDiscountByCode
            {
                Code = Code,
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

        public ResultDto<DiscountDto> GetDiscountById(Guid Id)
        {
            var grpc_discountService = new DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.GetDiscountById(new RequestGetDiscountById
            {
                Id = Id.ToString(),
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

        public ResultDto UseDiscount(Guid DiscountId)
        {
            var grpc_discountService = new DiscountServiceProto.DiscountServiceProtoClient(channel);
            var result = grpc_discountService.UseDiscount(new RequestUseDiscount
            {
                Id = DiscountId.ToString(),
            });
            return new ResultDto
            {
                IsSuccess = result.IsSuccess
            };
        }
    }

    public class DiscountDto
    {
        public int Amount { get; set; }
        public string Code { get; set; }
        public Guid Id { get; set; }
        public bool Used { get; set; }
    }
}
