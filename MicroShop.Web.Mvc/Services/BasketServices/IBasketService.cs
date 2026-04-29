using Common.Core.Dtos;
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
        ResultDto ApplyDiscountToBasket(Guid basketId, Guid discountId);
        ResultDto Checkout(CheckoutDto checkout);
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

        public ResultDto ApplyDiscountToBasket(Guid basketId, Guid discountId)
        {
            //https://localhost:6001/api/Basket/7d9df6bc-8e91-476f-c28d-08d983442ffa/9d9df6bc-8e91-476f-c28d-08d983442ffa

            var request = new RestRequest($"/api/Basket/{basketId}/{discountId}", Method.PUT);
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);

        }

        public ResultDto Checkout(CheckoutDto checkout)
        {
            var request = new RestRequest($"/api/Basket/CheckoutBasket", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            string serializeModel = JsonSerializer.Serialize(checkout);
            request.AddParameter("application/json", serializeModel, ParameterType.RequestBody);
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);

        }
    }

    public class BasketDto
    {
        public string id { get; set; }
        public string userId { get; set; }
        public Guid? discountId { get; set; }
        public DiscountInBasketDto DiscountDetail { get; set; } = null;

        public List<BasketItem> items { get; set; }
        public int TotalPrice()
        {
            int result = items.Sum(p => p.unitPrice * p.quantity);
            if (discountId.HasValue)
                result = result - DiscountDetail.Amount;
            return result;
        }
    }

    public class DiscountInBasketDto
    {
        public int Amount { get; set; }
        public string DiscountCode { get; set; }
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

    public class CheckoutDto
    {
        public Guid BasketId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
    }
}
