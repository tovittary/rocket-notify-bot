namespace RocketNotify.ChatClient.Tests.ApiClient
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.ChatClient.ApiClient;
    using RocketNotify.ChatClient.Dto.Messages;
    using RocketNotify.ChatClient.Exceptions;
    using RocketNotify.ChatClient.Model;

    [TestFixture]
    public class RestApiClientTests
    {
        private Mock<IHttpClientWrapper> _httpClientMock;

        private Mock<RestApiClient> _clientMock;

        [SetUp]
        public void SetUp()
        {
            _httpClientMock = new Mock<IHttpClientWrapper>();
            _clientMock = new Mock<RestApiClient>(_httpClientMock.Object) { CallBase = true };
        }

        [Test]
        public void AuthenticateAsync_AlreadyAuthenticated_ShouldThrowException()
        {
            _clientMock.Setup(x => x.AuthData).Returns(new AuthorizationData(string.Empty, string.Empty));
            Assert.ThrowsAsync<InvalidOperationException>(() => _clientMock.Object.AuthenticateAsync(string.Empty, new AuthenticationData()));
        }

        [Test]
        public async Task AuthenticateAsync_HasAuthToken_ShouldAuthenticate()
        {
            var expected = new AuthorizationData("SomeUserId", "SomeAuthToken");
            using var responseContent = new StringContent($"{{ \"status\" : \"success\", \"data\" : {{ \"userId\" : \"{expected.UserId}\", \"authToken\": \"{expected.AuthToken}\" }}}}", Encoding.UTF8, "application/json");
            using var fakeResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent };
            _httpClientMock.Setup(x => x.PostAsync("/api/v1/login", It.IsAny<HttpContent>())).ReturnsAsync(fakeResponse);

            var authenticationData = new AuthenticationData { AuthToken = "SomeToken" };
            await _clientMock.Object.AuthenticateAsync("https://someserver.com", authenticationData).ConfigureAwait(false);
            var actual = _clientMock.Object.AuthData;

            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.AuthToken, actual.AuthToken);
        }

        [Test]
        public async Task AuthenticateAsync_HasUserAndPassword_ShouldAuthenticate()
        {
            var expected = new AuthorizationData("SomeUserId", "SomeAuthToken");
            using var responseContent = new StringContent($"{{ \"status\" : \"success\", \"data\" : {{ \"userId\" : \"{expected.UserId}\", \"authToken\": \"{expected.AuthToken}\" }}}}", Encoding.UTF8, "application/json");
            using var fakeResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = responseContent };
            _httpClientMock.Setup(x => x.PostAsync("/api/v1/login", It.IsAny<HttpContent>())).ReturnsAsync(fakeResponse);

            var authenticationData = new AuthenticationData { User = "SomeUser", Password = "SomePassword" };
            await _clientMock.Object.AuthenticateAsync("https://someserver.com", authenticationData).ConfigureAwait(false);
            var actual = _clientMock.Object.AuthData;

            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.AuthToken, actual.AuthToken);
        }

        [Test]
        public async Task AuthenticateAsync_HasAuthDataButTokenInvalid_ShouldAuthenticateOnSecondAttempt()
        {
            var expected = new AuthorizationData("SomeUserId", "SomeAuthToken");
            using var failedResponseContent = new StringContent("{ \"status\" : \"fail\", \"error\" : \"TestError\" }", Encoding.UTF8, "application/json");
            using var failedFakeResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, Content = failedResponseContent };
            using var successResponseContent = new StringContent($"{{ \"status\" : \"success\", \"data\" : {{ \"userId\" : \"{expected.UserId}\", \"authToken\": \"{expected.AuthToken}\" }}}}", Encoding.UTF8, "application/json");
            using var successFakeResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = successResponseContent };

            var requestNumber = 0;
            _httpClientMock
                .Setup(x => x.PostAsync("/api/v1/login", It.IsAny<HttpContent>()))
                .ReturnsAsync(() =>
                {
                    if (requestNumber++ == 0)
                        return failedFakeResponse;

                    return successFakeResponse;
                });

            var authenticationData = new AuthenticationData { AuthToken = "SomeToken", User = "SomeUser", Password = "SomePassword" };
            await _clientMock.Object.AuthenticateAsync("https://someserver.com", authenticationData).ConfigureAwait(false);
            var actual = _clientMock.Object.AuthData;

            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.AuthToken, actual.AuthToken);
        }

        [Test]
        public void AuthenticateAsync_AuthenticationFailed_ShouldThrowException()
        {
            using var failedResponseContent = new StringContent("{ \"status\" : \"fail\", \"error\" : \"TestError\" }", Encoding.UTF8, "application/json");
            using var failedFakeResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized, Content = failedResponseContent };
            _httpClientMock.Setup(x => x.PostAsync("/api/v1/login", It.IsAny<HttpContent>())).ReturnsAsync(failedFakeResponse);

            var authenticationData = new AuthenticationData { AuthToken = "SomeToken", User = "SomeUser", Password = "SomePassword" };
            Assert.ThrowsAsync<AuthenticationException>(() => _clientMock.Object.AuthenticateAsync("https://someserver.com", authenticationData));
        }

        [Test]
        public void GetLastMessageInGroupAsync_NoAuthData_ShouldThrowException()
        {
            _clientMock.Setup(x => x.AuthData).Returns((AuthorizationData)null);
            Assert.ThrowsAsync<InvalidOperationException>(() => _clientMock.Object.GetLastMessageInGroupAsync(string.Empty));
        }

        [Test]
        public async Task GetLastMessageInGroupAsync_HasAuthData_ShouldAddAuthHeaders()
        {
            var expected = new AuthorizationData("UserId", "AuthToken");
            _clientMock.Setup(x => x.AuthData).Returns(expected);
            var groupName = "SomeGroup";

            using var emptyResponseContent = new StringContent("{ \"messages\": [], \"total\": 0, \"success\": true }", Encoding.UTF8, "application/json");
            using var emptyResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = emptyResponseContent };

            var actual = new string[2];
            _httpClientMock
                .Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(msg => msg.RequestUri.Equals($"/api/v1/groups.messages?count=1&roomName={groupName}")), It.IsAny<HttpCompletionOption>()))
                .Callback<HttpRequestMessage, HttpCompletionOption>((msg, t) =>
                {
                    actual[0] = msg.Headers.GetValues("X-Auth-Token").FirstOrDefault();
                    actual[1] = msg.Headers.GetValues("X-User-Id").FirstOrDefault();
                })
                .ReturnsAsync(emptyResponse);

            await _clientMock.Object.GetLastMessageInGroupAsync(groupName).ConfigureAwait(false);

            Assert.AreEqual(expected.AuthToken, actual[0]);
            Assert.AreEqual(expected.UserId, actual[1]);
        }

        [Test]
        public async Task GetLastMessageInGroupAsync_RequestSuccess_ShouldReturnMessage()
        {
            var authData = new AuthorizationData("UserId", "AuthToken");
            _clientMock.Setup(x => x.AuthData).Returns(authData);
            var groupName = "SomeGroup";

            var expected = new MessageDto { TimeStamp = new DateTime(2021, 02, 01, 12, 32, 44) };
            using var successReponseContent = new StringContent($"{{ \"messages\": [ {{ \"ts\": \"{expected.TimeStamp:s}\" }} ], \"total\": 0, \"success\": true }}", Encoding.UTF8, "application/json");
            using var successResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = successReponseContent };
            _httpClientMock.Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(msg => msg.RequestUri.Equals($"/api/v1/groups.messages?count=1&roomName={groupName}")), It.IsAny<HttpCompletionOption>()))
                .ReturnsAsync(successResponse);

            var actual = await _clientMock.Object.GetLastMessageInGroupAsync(groupName).ConfigureAwait(false);

            Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
        }

        [Test]
        public void GetLastMessageInGroupAsync_RequestFail_ShouldThrowException()
        {
            var authData = new AuthorizationData("UserId", "AuthToken");
            _clientMock.Setup(x => x.AuthData).Returns(authData);
            var groupName = "SomeGroup";

            var expected = new MessageDto { TimeStamp = new DateTime(2021, 02, 01, 12, 32, 44) };
            using var failReponseContent = new StringContent($"{{ \"error\": \"TestError\", \"success\": false }}", Encoding.UTF8, "application/json");
            using var failResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = failReponseContent };
            _httpClientMock.Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(msg => msg.RequestUri.Equals($"/api/v1/groups.messages?count=1&roomName={groupName}")), It.IsAny<HttpCompletionOption>()))
                .ReturnsAsync(failResponse);

            Assert.ThrowsAsync<RocketChatApiException>(() => _clientMock.Object.GetLastMessageInGroupAsync(groupName));
        }
    }
}
