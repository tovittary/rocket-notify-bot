namespace RocketNotify.ChatClient.Model
{
    /// <summary>
    /// Authorization data for API requests.
    /// </summary>
    public class AuthorizationData
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user authorization token.
        /// </summary>
        public string AuthToken { get; set; }
    }
}
