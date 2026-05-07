using Common.Core.Dtos;
using Newtonsoft.Json;
using RestSharp;
using static Common.Core.Enums.Enumration;

namespace MicroShop.Web.Mvc.Services.OrderService
{
    public interface IOrderService
    {
        List<OrderDto> GetOrders(string UserId);
        OrderDetailDto OrderDetail(Guid OrderId); 
        ResultDto RequestPayment(Guid OrderId);

    }

    public class OrderService : IOrderService
    {
        private readonly RestClient restClient;
        public OrderService(RestClient restClient)
        {
            this.restClient = restClient;
            restClient.Timeout = -1;
        }


        public List<OrderDto> GetOrders(string UserId)
        {
            var request = new RestRequest("/api/Order", Method.GET);
            IRestResponse response = restClient.Execute(request);
            var orders = JsonConvert.DeserializeObject<List<OrderDto>>(response.Content);
            return orders;
        }

        public OrderDetailDto OrderDetail(Guid OrderId)
        {
            var request = new RestRequest($"/api/Order/{OrderId}", Method.GET);
            IRestResponse response = restClient.Execute(request);
            var orderdetail = JsonConvert.DeserializeObject<OrderDetailDto>(response.Content);
            return orderdetail;
        }

        public ResultDto RequestPayment(Guid OrderId)
        {
            var request = new RestRequest($"/api/OrderPayment?OrderId={OrderId}", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);
        }
        private static ResultDto GetResponseStatusCode(IRestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new ResultDto
                {
                    IsSuccess = true,
                };
            }
            else
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = response.ErrorMessage
                };
            }
        }
    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public int ItemCount { get; set; }
        public int TotalPrice { get; set; }
        public bool OrderPaid { get; set; }
        public DateTime OrderPlaced { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

    }

    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderPlaced { get; set; }
        public bool OrderPaid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalPrice { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

    }

    public class OrderLineDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
