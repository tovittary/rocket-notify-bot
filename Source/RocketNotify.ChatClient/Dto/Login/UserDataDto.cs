namespace RocketNotify.ChatClient.Dto.Login
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// DTO containing user data required for further API usage.
    /// </summary>
    public class UserDataDto
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user authorization token.
        /// </summary>
        [JsonPropertyName("authToken")]
        public string AuthToken { get; set; }
    }
}
