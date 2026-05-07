using Common.Core.Dtos;
using RestSharp;
using System.Text.Json;

namespace MicroShop.Admin.Mvc.Services.ProductServices
{

    public interface IProductManagementService
    {
        List<ProductDto> GetProducts();
        ResultRecordDto UpdateName(UpdateProductDto updateProduct);

    }

    public class ProductManagementService : IProductManagementService
    {

        private readonly RestClient restClient;
        public ProductManagementService(RestClient restClient)
        {
            this.restClient = restClient;
            restClient.Timeout = -1;
        }

        public List<ProductDto> GetProducts()
        {
            var request = new RestRequest("/api/ProductManagement", Method.GET);
            IRestResponse response = restClient.Execute(request);
            var products = JsonSerializer.Deserialize<List<ProductDto>>(response.Content);
            return products;
        }

        public ResultRecordDto UpdateName(UpdateProductDto updateProduct)
        {
            var request = new RestRequest($"/api/ProductManagement", Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            string serializeModel = JsonSerializer.Serialize(updateProduct);
            request.AddParameter("application/json", serializeModel, ParameterType.RequestBody);
            IRestResponse response = restClient.Execute(request);
            return GetResponseStatusCode(response);
        }


        private static ResultRecordDto GetResponseStatusCode(IRestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new ResultRecordDto(false);
            }
            else
            {
                return new ResultRecordDto(false, response.ErrorMessage);
            }
        }
    }
    public record UpdateProductDto(Guid ProductId, string Name);

    public record ProductDto(Guid id, string? name, string? description,
        string? image, int price,
        ProductCategoryDto? productCategory);
    public record ProductCategoryDto(Guid categoryId, string category);
}
