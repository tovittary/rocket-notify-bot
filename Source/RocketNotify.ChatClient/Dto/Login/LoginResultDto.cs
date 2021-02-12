namespace RocketNotify.ChatClient.Dto.Login
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Authentication result DTO.
    /// </summary>
    public class LoginResultDto
    {
        /// <summary>
        /// Gets or sets the authentication status.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        [JsonPropertyName("data")]
        public UserDataDto Data { get; set; }
    }
}
