namespace RocketNotify.ChatClient.Dto.Messages
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// DTO containing messages from a group chat.
    /// </summary>
    public class MessagesDto
    {
        /// <summary>
        /// Gets or sets group chat messages.
        /// </summary>
        [JsonPropertyName("messages")]
        public MessageDto[] Messages { get; set; }

        /// <summary>
        /// Gets or sets a total number of messages in a group chat.
        /// </summary>
        [JsonPropertyName("total")]
        public int TotalMessages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request was completed successfully.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
