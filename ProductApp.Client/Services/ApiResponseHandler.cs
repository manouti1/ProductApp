using System.Net.Http.Json;

namespace ProductApp.Client.Services
{
    public static class ApiResponseHandler
    {
        public static async Task<T?> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            var error = await response.Content.ReadAsStringAsync();
            throw new ApplicationException($"API Error: {error}");
        }
    }
}
