namespace RocketNotify.TelegramBot.MessageProcessing.Model
{
    using System;

    using RocketNotify.TelegramBot.MessageProcessing.Model.Markups;

    using Telegram.Bot.Types;

    /// <summary>
    /// Provides functionality for conversion between the <see cref="Telegram.Bot.Types.Message"/> and <see cref="BotMessage"/> classes.
    /// </summary>
    public static class MessageConverter
    {
        /// <summary>
        /// Converts an instance of the <see cref="Telegram.Bot.Types.Message"/> class to an instance of the <see cref="BotMessage"/> class.
        /// </summary>
        /// <param name="message">The <see cref="Telegram.Bot.Types.Message"/> instance being converted.</param>
        /// <returns>An instance of the <see cref="BotMessage"/> class.</returns>
        public static BotMessage Convert(Message message)
        {
            var msg = new BotMessage
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
