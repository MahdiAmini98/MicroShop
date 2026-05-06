using Newtonsoft.Json;
using RestSharp;

namespace MicroShop.Web.Mvc.Services.OrderService
{
    public interface IOrderService
    {
        List<OrderDto> GetOrders(string UserId);
        OrderDetailDto OrderDetail(Guid OrderId);
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
    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public int ItemCount { get; set; }
        public int TotalPrice { get; set; }
        public bool OrderPaid { get; set; }
        public DateTime OrderPlaced { get; set; }

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

    }

    public class OrderLineDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
