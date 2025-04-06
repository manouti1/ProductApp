using ProductApp.Shared;
using System.Net.Http.Json;

namespace ProductApp.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _http;
        public ProductService(HttpClient http) => _http = http;

        public async Task<List<Product>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/products");
            return await ApiResponseHandler.HandleResponse<List<Product>>(response) ?? new();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var response = await _http.PostAsJsonAsync("api/products", product);
            var createdProduct = await ApiResponseHandler.HandleResponse<Product>(response);
            return createdProduct ?? new Product();
        }

        public async Task UpdateAsync(Product product)
        {
            if (product.Price < 0 || product.Quantity < 0)
            {
                throw new ArgumentException("Price and quantity must be non-negative");
            }

            var response = await _http.PutAsJsonAsync($"api/products/{product.Id}", product);
            await ApiResponseHandler.HandleResponse<Product>(response);
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/products/{id}");
            var result = await ApiResponseHandler.HandleResponse<Dictionary<string, string>>(response);

            if (result != null && result.TryGetValue("Message", out var message))
            {
                Console.WriteLine(message);
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/products/{id}");
            return await ApiResponseHandler.HandleResponse<Product>(response) ?? new Product();
        }
    }
}