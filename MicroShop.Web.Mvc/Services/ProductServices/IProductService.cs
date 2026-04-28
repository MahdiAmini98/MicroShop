using RestSharp;
using System.Text.Json;

namespace MicroShop.Web.Mvc.Services.ProductServices
{
    public interface IProductService
    {
        IEnumerable<ProductDto> GetAllProduct();
        ProductDto Getproduct(Guid Id);
    }

    public class ProductService : IProductService
    {
        private readonly RestClient restClient;
        public ProductService(RestClient restClient)
        {
            this.restClient = restClient;
            restClient.Timeout = -1;
        }


        public IEnumerable<ProductDto> GetAllProduct()
        {
            var request = new RestRequest("/api/Product", Method.GET);
            IRestResponse response = restClient.Execute(request);
            Console.WriteLine(response.Content);
            var products = JsonSerializer.Deserialize<List<ProductDto>>(response.Content);
            return products;
        }

        public ProductDto Getproduct(Guid Id)
        {
            var request = new RestRequest($"/api/Product/{Id}", Method.GET);

            IRestResponse response = restClient.Execute(request);

            var product = JsonSerializer.Deserialize<ProductDto>(response.Content);
            return product;
        }
    }


    public class ProductCategory
    {
        public string categoryId { get; set; }
        public string category { get; set; }
    }
    public class ProductDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public int price { get; set; }
        public ProductCategory productCategory { get; set; }
    }
}
