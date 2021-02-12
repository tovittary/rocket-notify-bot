namespace RocketNotify.ChatClient.Dto.Login
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Auth token validity resume request DTO.
    /// </summary>
    public class ResumeRequestDto
    {
        /// <summary>
        /// Gets or sets the auth token to resume.
        /// </summary>
        [JsonPropertyName("resume")]
        public string Resume { get; set; }
    }
}
