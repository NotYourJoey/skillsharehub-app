using SkillShareHub.Helpers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static SkillShareHub.Services.PostService;

namespace SkillShareHub.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private const int TimeoutSeconds = 30;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            InitializeHttpClient();
        }

        // Initialize HttpClient settings
        private void InitializeHttpClient()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
            _httpClient.BaseAddress = new Uri(Constants.ApiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Add auth token to requests
        private void AddAuthHeader()
        {
            if (!string.IsNullOrEmpty(Settings.AuthToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Settings.AuthToken);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            return await SendAsync(() => _httpClient.GetAsync(CombineUrl(endpoint)));
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            return await SendAsync(() => _httpClient.PostAsync(CombineUrl(endpoint), content));
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
        {
            return await SendAsync(() => _httpClient.PutAsync(CombineUrl(endpoint), content));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await SendAsync(() => _httpClient.DeleteAsync(CombineUrl(endpoint)));
        }

        private async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> action)
        {
            try
            {
                AddAuthHeader();
                var response = await action().ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // Throws exception if not successful
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError($"API Error: {ex.Message}");
                throw new HttpRequestException("An error occurred while making the request.", ex);
            }
        }

        // Helper to construct full URL
        private string CombineUrl(string endpoint)
        {
            return new Uri(new Uri(Constants.ApiBaseUrl), endpoint).ToString();
        }
    }
}
