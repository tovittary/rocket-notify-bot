namespace RocketNotify.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.ChatClient;
    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Settings;

    /// <summary>
    /// Registers chat client services.
    /// </summary>
    public class ChatClientServiceRegistration : IServiceRegistration
    {
        /// <inheritdoc />
        public void Register(IServiceCollection services)
        {
            services.AddTransient<IClientSettingsProvider, ClientSettingsProvider>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddTransient<IRestApiClient, RestApiClient>();
            services.AddTransient<IRocketChatClient, RocketChatClient>();
        }
    }
}
