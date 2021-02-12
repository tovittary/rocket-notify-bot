namespace RocketNotify.ChatClient.Dto.Error
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// DTO containing error data.
    /// </summary>
    public class ErrorDto
    {
        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
