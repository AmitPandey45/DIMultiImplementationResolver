namespace DIMultiImplementationResolver
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PutAsync(string url, HttpContent content, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PatchAsync(string url, HttpContent content, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAsync(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);

        Task<TResponse> GetAsync<TResponse>(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest requestData, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<TResponse> PatchAsync<TRequest, TResponse>(string url, TRequest requestData, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
        Task<TResponse> DeleteAsync<TResponse>(string url, Action<HttpClient> configureHttpClient = null, CancellationToken cancellationToken = default);
    }
}
