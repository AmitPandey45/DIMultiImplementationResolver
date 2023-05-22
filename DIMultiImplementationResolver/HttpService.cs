using Newtonsoft.Json;
using System.Text;

namespace DIMultiImplementationResolver
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<HttpResponseMessage> GetAsync(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var httpClient = CreateHttpClient(configureHttpClient);
            return await httpClient.GetAsync(url, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var httpClient = CreateHttpClient(configureHttpClient);
            return await httpClient.PostAsync(url, content, cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent content, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var httpClient = CreateHttpClient(configureHttpClient);
            return await httpClient.PutAsync(url, content, cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> PatchAsync(string url, HttpContent content, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var httpClient = CreateHttpClient(configureHttpClient);
            using var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };

            return await httpClient.SendAsync(request, cancellationTokenSource.Token);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var httpClient = CreateHttpClient(configureHttpClient);
            return await httpClient.DeleteAsync(url, cancellationTokenSource.Token);
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            var response = await this.GetAsync(url, configureHttpClient, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await this.ReadResponse<TResponse>(response);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            var content = this.CreateHttpContent(requestData);
            var response = await this.PostAsync(url, content, configureHttpClient, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await this.ReadResponse<TResponse>(response);
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest requestData, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            var content = this.CreateHttpContent(requestData);
            var response = await this.PutAsync(url, content, configureHttpClient, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await this.ReadResponse<TResponse>(response);
        }

        public async Task<TResponse> PatchAsync<TRequest, TResponse>(string url, TRequest requestData, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            var content = this.CreateHttpContent(requestData);
            var response = await this.PatchAsync(url, content, configureHttpClient, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await this.ReadResponse<TResponse>(response);
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default)
        {
            var response = await this.DeleteAsync(url, configureHttpClient, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await this.ReadResponse<TResponse>(response);
        }

        private HttpClient CreateHttpClient(Action<HttpClient> configureHttpClient)
        {
            var httpClient = _httpClient;
            configureHttpClient?.Invoke(httpClient); // Apply customizations if provided
            return httpClient;
        }

        private HttpContent CreateHttpContent<TRequest>(TRequest content)
        {
            string jsonPayload = JsonConvert.SerializeObject(content);
            return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        }

        private async Task<TResponse> ReadResponse<TResponse>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseBody);
        }
    }
}
