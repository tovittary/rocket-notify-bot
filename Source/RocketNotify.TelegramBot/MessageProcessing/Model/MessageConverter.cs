namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    using System;

    using RocketNotify.TelegramBot.MessageProcessing.Model.Markups;

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
                MessageId = message.MessageId,
                Text = message.Text,
                Sender = new MessageSender
                {
                    Id = message.Chat.Id,
                    Name = message.Chat.Title ?? message.Chat.FirstName
                }
            };

            if (message.ReplyToMessage != null)
                msg.ReplyInfo = new MessageReply { SourceMessageId = message.ReplyToMessage.MessageId };

            return msg;
        }

        /// <summary>
        /// Converts an implementation of the <see cref="IMessageMarkup"/> interface to an
        /// implementation of the <see cref="Telegram.Bot.Types.ReplyMarkups.IReplyMarkup"/> interface.
        /// </summary>
        /// <param name="markup">The <see cref="IMessageMarkup"/> interface implementation instance.</param>
        /// <returns>An instance of the <see cref="Telegram.Bot.Types.ReplyMarkups.IReplyMarkup"/> interface implementation.</returns>
        internal static Telegram.Bot.Types.ReplyMarkups.IReplyMarkup ConvertMarkup(IMessageMarkup markup)
        {
            return markup switch
            {
                ForceReplyMarkup _ => new Telegram.Bot.Types.ReplyMarkups.ForceReplyMarkup(),
                _ => throw new ArgumentOutOfRangeException(nameof(markup)),
            };
        }
    }
}
