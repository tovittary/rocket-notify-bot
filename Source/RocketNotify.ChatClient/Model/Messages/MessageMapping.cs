namespace RocketNotify.ChatClient.Model.Messages
{
    using System.Linq;

    using RocketNotify.ChatClient.Dto.Messages;

    /// <summary>
    /// Provides mapping functionality for message objects.
    /// </summary>
    public static class MessageMapping
    {
        /// <summary>
        /// Converts the <see cref="MessageDto"/> instance to the <see cref="Message"/> instance.
        /// </summary>
        /// <param name="messageDto">The instance of the <see cref="MessageDto"/> class.</param>
        /// <returns>The <see cref="Message"/> class instance.</returns>
        public static Message ToMessage(MessageDto messageDto)
        {
            return new Message
            {
                Id = messageDto.Id,
                TimeStamp = messageDto.TimeStamp,
                MessageText = messageDto.Message,
                Mentions = messageDto.Mentions.Select(ToMention).ToArray()
            };
        }

        /// <summary>
        /// Converts the <see cref="MentionDto"/> instance to the <see cref="Mention"/> instance.
        /// </summary>
        /// <param name="mentionDto">The instance of the <see cref="MentionDto"/> class.</param>
        /// <returns>The <see cref="Mention"/> class instance.</returns>
        private static Mention ToMention(MentionDto mentionDto)
        {
            return new Mention { Username = mentionDto.Username };
        }
    }
}
