namespace RocketNotify.ChatClient.IntegrationTests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Model;

    [TestFixture]
    public class RestApiClientTests
    {
        #region Rocket.Chat client settings

        private const string _server = "";

        private const string _userName = "";

        private const string _password = "";

        private const string _authToken = "";

        private const string _groupChatName = "";

        #endregion

        private IRestApiClient _client;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            var httpClient = new HttpClient();
            var httpClientWrapper = new HttpClientWrapper(httpClient);
            var logger = Mock.Of<ILogger<RestApiClient>>();

            _client = new RestApiClient(httpClientWrapper, logger);

            var authData = new AuthenticationData
            {
                User = _userName,
                Password = _password,
                AuthToken = _authToken
            };

            await _client.AuthenticateAsync(_server, authData).ConfigureAwait(false);
        }

        [Test]
        public async Task GetLastMessageInGroupAsync_ShouldReturnLatestMessage()
        {
            var message = await _client.GetLastMessageInGroupAsync(_groupChatName).ConfigureAwait(false);

            Assert.NotNull(message);
        }

        [Test]
        public async Task GetRecentMessagesInGroupAsync_ShouldReturnLastMessages()
        {
            var message = await _client.GetRecentMessagesInGroupAsync(_groupChatName, 10).ConfigureAwait(false);

            Assert.NotNull(message);
        }
    }
}
