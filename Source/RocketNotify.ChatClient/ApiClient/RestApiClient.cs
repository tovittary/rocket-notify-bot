namespace RocketNotify.ChatClient.ApiClient
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RocketNotify.ChatClient.Dto.Error;
    using RocketNotify.ChatClient.Dto.Login;
    using RocketNotify.ChatClient.Dto.Messages;
    using RocketNotify.ChatClient.Exceptions;
    using RocketNotify.ChatClient.Model;

    /// <summary>
    /// REST API Rocket.Chat client.
    /// </summary>
    public class RestApiClient : IRestApiClient
    {
        /// <summary>
        /// API request string template for getting the last group chat message.
        /// </summary>
        private const string MessagesApiTemplate = "/api/v1/groups.messages?count=1&roomName={0}";

        /// <summary>
        /// Authentication request URL string.
        /// </summary>
        private const string LoginApiUrl = "/api/v1/login";

        /// <summary>
        /// HTTP client instance used for sending requests.
        /// </summary>
        private readonly IHttpClientWrapper _httpClient;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<RestApiClient> _logger;

        /// <summary>
        /// Authorization data for sending API requests.
        /// </summary>
        private AuthorizationData _authData;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">HTTP client instance.</param>
        /// <param name="logger">Logger.</param>
        public RestApiClient(IHttpClientWrapper httpClient, ILogger<RestApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public virtual AuthorizationData AuthData => _authData;

        /// <inheritdoc />
        public async Task AuthenticateAsync(string server, AuthenticationData authData)
        {
            _logger.LogInformation("Rocket.Chat client authentication in progress");

            if (AuthData != null)
                throw new InvalidOperationException("The client already authenticated.");

            _httpClient.BaseAddress = new Uri(server);

            var authSuccess = await AuthenticateByAuthTokenAsync(authData).ConfigureAwait(false);
            if (authSuccess)
                return;

            authSuccess = await AuthenticateByUserNameAndPasswordAsync(authData).ConfigureAwait(false);
            if (authSuccess)
                return;

            throw new AuthenticationException("Failed to authenticate to Rocket.Chat.");
        }

        /// <inheritdoc />
        public async Task<MessageDto> GetLastMessageInGroupAsync(string groupName)
        {
            if (AuthData == null)
                throw new InvalidOperationException("The client not authenticated.");

            var messagesQueryUrl = string.Format(MessagesApiTemplate, groupName);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, messagesQueryUrl);
            requestMessage.Headers.Add("X-Auth-Token", AuthData.AuthToken);
            requestMessage.Headers.Add("X-User-Id", AuthData.UserId);

            var messages = await SendAsync<MessagesDto>(requestMessage).ConfigureAwait(false);
            return messages.Messages.FirstOrDefault();
        }

        /// <summary>
        /// Authenticates Rocket.Chat client with the provided auth token.
        /// </summary>
        /// <param name="authData">Authentication data.</param>
        /// <returns>A task that represents the authentication process.</returns>
        private async Task<bool> AuthenticateByAuthTokenAsync(AuthenticationData authData)
        {
            if (string.IsNullOrEmpty(authData.AuthToken))
                return false;

            _logger.LogInformation("Rocket.Chat auth token resume in progress");
            var authSuccess = await TryResumeAuthTokenAsync(authData.AuthToken).ConfigureAwait(false);
            if (authSuccess)
                _logger.LogInformation("Rocket.Chat auth token has been resumed successfully");
            else
                _logger.LogWarning("Rocket.Chat auth token resume has failed");

            return authSuccess;
        }

        /// <summary>
        /// Authenticates Rocket.Chat client with the provided username and password.
        /// </summary>
        /// <param name="authData">Authentication data.</param>
        /// <returns>A task that represents the authentication process.</returns>
        private async Task<bool> AuthenticateByUserNameAndPasswordAsync(AuthenticationData authData)
        {
            _logger.LogInformation("Rocket.Chat client authentication with username and password in progress");

            var authSuccess = await TryLoginAsync(authData.User, authData.Password).ConfigureAwait(false);
            if (authSuccess)
                _logger.LogInformation("Rocket.Chat client has been authenticated successfully");

            return authSuccess;
        }

        /// <summary>
        /// Attempts to resume the provided auth token validity.
        /// </summary>
        /// <param name="authToken">Auth token.</param>
        /// <returns><c>true</c> if auth token validity has been successfully resumed, <c>false</c> otherwise.</returns>
        private Task<bool> TryResumeAuthTokenAsync(string authToken)
        {
            var dto = new ResumeRequestDto { Resume = authToken };
            var serialized = JsonSerializer.Serialize(dto);

            return TryAuthenticateAsync(serialized);
        }

        /// <summary>
        /// Attempts to authenticate using username and password.
        /// </summary>
        /// <param name="userName">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns><c>true</c> if authentication succeeded, <c>false</c> otherwise.</returns>
        private Task<bool> TryLoginAsync(string userName, string password)
        {
            var dto = new LoginRequestDto { User = userName, Password = password };
            var serialized = JsonSerializer.Serialize(dto);

            return TryAuthenticateAsync(serialized);
        }

        /// <summary>
        /// Attempts to authenticate.
        /// </summary>
        /// <param name="jsonData">Authentication request JSON body.</param>
        /// <returns><c>true</c> if authentication succeeded, <c>false</c> otherwise.</returns>
        private async Task<bool> TryAuthenticateAsync(string jsonData)
        {
            try
            {
                var loginResultDto = await PostAsync<LoginResultDto>(LoginApiUrl, jsonData).ConfigureAwait(false);

                _authData = new AuthorizationData(loginResultDto.Data.UserId, loginResultDto.Data.AuthToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Sends a POST request.
        /// </summary>
        /// <typeparam name="TResult">Assumed type of response.</typeparam>
        /// <param name="url">Request URL.</param>
        /// <param name="jsonData">Request JSON body.</param>
        /// <returns>Response object.</returns>
        private async Task<TResult> PostAsync<TResult>(string url, string jsonData)
            where TResult : class, new()
        {
            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            return await ProcessResponseAsync<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends provided message.
        /// </summary>
        /// <typeparam name="TResult">Assumed type of response.</typeparam>
        /// <param name="message">Request message instance.</param>
        /// <returns>Responce object.</returns>
        private async Task<TResult> SendAsync<TResult>(HttpRequestMessage message)
            where TResult : class, new()
        {
            using var response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            return await ProcessResponseAsync<TResult>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes response to a request.
        /// </summary>
        /// <typeparam name="TResult">Type of response content.</typeparam>
        /// <param name="response">Response to a request.</param>
        /// <returns>Response content.</returns>
        private async Task<TResult> ProcessResponseAsync<TResult>(HttpResponseMessage response)
            where TResult : class, new()
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent);
                throw new RocketChatApiException(errorDto.Error);
            }

            return JsonSerializer.Deserialize<TResult>(responseContent);
        }
    }
}
