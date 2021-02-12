namespace RocketNotify.ChatClient.Model
{
    /// <summary>
    /// Authentication data.
    /// </summary>
    public class AuthenticationData
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the existing authorization token.
        /// </summary>
        public string AuthToken { get; set; }
    }
}
