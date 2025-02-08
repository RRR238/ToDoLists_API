using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ToDo_lists.Services
{ 
        public class CallEndpoint
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private Security _security;
        private IConfiguration _config;

        public CallEndpoint(IHttpClientFactory httpClientFactory, Security security, IConfiguration configuration) 
        {
            _httpClientFactory = httpClientFactory;
            _security = security;
            _config = configuration;
        }

        public async Task<HttpResponseMessage> PostRequest(string url, object payload)
        {
            var client = _httpClientFactory.CreateClient();
            string token = _security.GetJWTToken(_config, tokenForClient: false);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            string jsonString = await jsonContent.ReadAsStringAsync();
            HttpResponseMessage response = await client.PostAsync(url, jsonContent);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteRequest(string url)
        {
            var client = _httpClientFactory.CreateClient();
            string token = _security.GetJWTToken(_config, tokenForClient: false);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.DeleteAsync(url);

            return response;
        }
    }
}