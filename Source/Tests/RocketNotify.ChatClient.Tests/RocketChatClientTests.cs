namespace RocketNotify.ChatClient.Tests
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Dto.Messages;
    using RocketNotify.ChatClient.Exceptions;
    using RocketNotify.ChatClient.Settings;

    [TestFixture]
    public class RocketChatClientTests
    {
        private Mock<IRestApiClient> _apiClientMock;

        private Mock<IClientSettingsProvider> _settingsProviderMock;

        private IRocketChatClient _rocketChatClient;

        [SetUp]
        public void SetUp()
        {
            _apiClientMock = new Mock<IRestApiClient>();
            _settingsProviderMock = new Mock<IClientSettingsProvider>();

            _settingsProviderMock.Setup(x => x.GetServer()).Returns("https://someserver.com");
            _settingsProviderMock.Setup(x => x.GetGroupName()).Returns(() => "SomeGroupName");

            _rocketChatClient = new RocketChatClient(_apiClientMock.Object, _settingsProviderMock.Object);
        }

        [Test]
        public void InitializeAsync_NoServer_ShouldThrowException()
        {
            _settingsProviderMock.Setup(x => x.GetServer()).Returns(string.Empty);
            Assert.ThrowsAsync<InvalidOperationException>(() => _rocketChatClient.InitializeAsync());
        }

        [Test]
        public void InitializeAsync_AuthDataEmpty_ShouldThrowException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _rocketChatClient.InitializeAsync());
        }

        [Test]
        public void InitializeAsync_NoTokenAndUser_ShouldThrowException()
        {
            _settingsProviderMock.Setup(x => x.GetPassword()).Returns("SomePassword");
            Assert.ThrowsAsync<InvalidOperationException>(() => _rocketChatClient.InitializeAsync());
        }

        [Test]
        public void InitializeAsync_NoTokenAndPassword_ShouldThrowException()
        {
            _settingsProviderMock.Setup(x => x.GetUserName()).Returns("SomeUserName");
            Assert.ThrowsAsync<InvalidOperationException>(() => _rocketChatClient.InitializeAsync());
        }

        [Test]
        public Task InitializeAsync_HasToken_ShouldAuthenticate()
        {
            _settingsProviderMock.Setup(x => x.GetAuthToken()).Returns("SomeToken");

            return _rocketChatClient.InitializeAsync();
        }

        [Test]
        public Task InitializeAsync_HasUserAndPassword_ShouldAuthenticate()
        {
            _settingsProviderMock.Setup(x => x.GetUserName()).Returns("SomeUserName");
            _settingsProviderMock.Setup(x => x.GetPassword()).Returns("SomePassword");

            return _rocketChatClient.InitializeAsync();
        }

        [Test]
        public void GetLastMessageTimeStampAsync_NoGroupName_ShouldThrowException()
        {
            _settingsProviderMock.Setup(x => x.GetGroupName()).Returns(string.Empty);
            Assert.ThrowsAsync<InvalidOperationException>(() => _rocketChatClient.GetLastMessageTimeStampAsync());
        }

        [Test]
        public async Task GetLastMessageTimeStampAsync_NoMessages_ShouldReturnDateTimeMinValue()
        {
            _apiClientMock.Setup(x => x.GetLastMessageInGroupAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult((MessageDto)null));

            var actual = await _rocketChatClient.GetLastMessageTimeStampAsync().ConfigureAwait(false);

            Assert.AreEqual(DateTime.MinValue, actual);
        }

        [Test]
        public async Task GetLastMessageTimeStampAsync_GotMessage_ShouldReturnMessageTimeStamp()
        {
            var expected = new DateTime(2021, 02, 01, 12, 32, 44);
            _apiClientMock.Setup(x => x.GetLastMessageInGroupAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new MessageDto { TimeStamp = expected }));

            var actual = await _rocketChatClient.GetLastMessageTimeStampAsync().ConfigureAwait(false);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetLastMessageTimeStampAsync_ClientThrowsHttpRequestException_ShouldRethrowRocketChatApiClientException()
        {
            _apiClientMock.Setup(x => x.GetLastMessageInGroupAsync(It.IsAny<string>())).Throws<HttpRequestException>();
            Assert.ThrowsAsync<RocketChatApiException>(() => _rocketChatClient.GetLastMessageTimeStampAsync());
        }

        [Test]
        public void GetLastMessageTimeStampAsync_ClientThrowsJsonException_ShouldRethrowRocketChatApiClientException()
        {
            _apiClientMock.Setup(x => x.GetLastMessageInGroupAsync(It.IsAny<string>())).Throws<JsonException>();
            Assert.ThrowsAsync<RocketChatApiException>(() => _rocketChatClient.GetLastMessageTimeStampAsync());
        }
    }
}
