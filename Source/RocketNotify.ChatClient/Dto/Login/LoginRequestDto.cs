namespace RocketNotify.ChatClient.Dto.Login
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Authentication request DTO.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
