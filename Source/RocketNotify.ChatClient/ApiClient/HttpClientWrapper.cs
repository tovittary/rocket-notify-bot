namespace RocketNotify.ChatClient.ApiClient
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Wraps necessary operations of the <see cref="HttpClient"/> class.
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        /// <summary>
        /// Underlying <see cref="HttpClient"/> instance.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="httpClient">Underlying <see cref="HttpClient"/> instance.</param>
        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public Uri BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) =>
            _httpClient.PostAsync(requestUri, content);

        /// <inheritdoc />
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption) =>
            _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
    }
}
