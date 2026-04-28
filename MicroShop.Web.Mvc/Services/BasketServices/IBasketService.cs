using MicroShop.Web.Mvc.Models.Dtos;
using RestSharp;
using System.Text.Json;

namespace MicroShop.Web.Mvc.Services.BasketServices
{
    public interface IBasketService
    {
        BasketDto GetBasket(string UserId);
        ResultDto AddToBasket(AddToBasketDto addToBasket, string UserId);
        ResultDto DeleteFromBasket(Guid Id);
        ResultDto UpdateQuantity(Guid BasketItemId, int quantity);
    }
    public class BasketService : IBasketService
    {
        private readonly RestClient restClient;

        public BasketService(RestClient restClient)
        {
            this.restClient = restClient;
            restClient.Timeout = -1;
        }

        public ResultDto AddToBasket(AddToBasketDto addToBasket, string UserId)
        {
            var request = new RestRequest($"/api/Basket?UserId={UserId}", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            string serializeModel = JsonSerializer.Serialize(addToBasket);
            request.AddParameter("application/json", serializeModel, ParameterType.RequestBody);
            var response = restClient.Execute(request);
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

        public BasketDto GetBasket(string UserId)
        {
            var request = new RestRequest($"/api/Basket?UserId={UserId}", Method.GET);
            IRestResponse response = restClient.Execute(request);
            var basket = JsonSerializer.Deserialize<BasketDto>(response.Content);
            return basket;
        }

        public ResultDto DeleteFromBasket(Guid Id)
        {
            var request = new RestRequest($"/api/Basket?ItemId={Id}", Method.DELETE);
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);
        }

        public ResultDto UpdateQuantity(Guid BasketItemId, int quantity)
        {
            var request = new RestRequest($"/api/Basket?basketItemId={BasketItemId}&quantity={quantity}", Method.PUT);
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);
        }
    }

    public class BasketDto
    {
        public string id { get; set; }
        public string userId { get; set; }
        public List<BasketItem> items { get; set; }
        public int TotalPrice()
        {
            return items.Sum(p => p.unitPrice * p.quantity);
        }
    }
    public class BasketItem
    {
        public string id { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public int unitPrice { get; set; }
        public int quantity { get; set; }
        public string imageUrl { get; set; }

        public int TotalPrice()
        {
            return unitPrice * quantity;
        }
    }
    public class AddToBasketDto
    {
        public string BasketId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
