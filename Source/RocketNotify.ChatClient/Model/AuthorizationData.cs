namespace RocketNotify.ChatClient.Model
{
    /// <summary>
    /// Authorization data for API requests.
    /// </summary>
    public class AuthorizationData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationData"/> class.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="authToken">Authorization token.</param>
        public AuthorizationData(string userId, string authToken)
        {
            UserId = userId;
            AuthToken = authToken;
        }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the user authorization token.
        /// </summary>
        public string AuthToken { get; }
    }
}
