namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    /// <summary>
    /// Provides functionality for conversion between the <see cref="Telegram.Bot.Types.Message"/> and <see cref="Message"/> classes.
    /// </summary>
    internal static class MessageConverter
    {
        /// <summary>
        /// Converts an instance of the <see cref="Telegram.Bot.Types.Message"/> class to an instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="message">The <see cref="Telegram.Bot.Types.Message"/> instance being converted.</param>
        /// <returns>An instance of the <see cref="Message"/> class.</returns>
        internal static Message Convert(Telegram.Bot.Types.Message message)
        {
            var msg = new Message
            {
                Id = message.MessageId,
                Text = message.Text,
                Sender = new MessageSender
                {
                    Id = message.Chat.Id,
                    Name = message.Chat.Title ?? message.Chat.FirstName
                }
            };

            if (message.ReplyToMessage != null)
                msg.Reply = new MessageReply { SourceMessageId = message.ReplyToMessage.MessageId };

            return msg;
        }
    }
}
