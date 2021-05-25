namespace RocketNotify.ChatClient.Dto.Messages
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// User mention found in the message.
    /// </summary>
    public class MentionDto
    {
        /// <summary>
        /// Gets or sets the mentioned user username.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
