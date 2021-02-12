namespace RocketNotify.ChatClient.Dto.Messages
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Chat message DTO.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message timestamp.
        /// </summary>
        [JsonPropertyName("ts")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        [JsonPropertyName("msg")]
        public string Message { get; set; }
    }
}
