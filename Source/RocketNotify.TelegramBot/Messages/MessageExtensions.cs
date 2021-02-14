namespace RocketNotify.TelegramBot.Messages
{
    using System;
    using System.Linq;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Extension methods for the <see cref="Message"/> class.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Gets entities from the message as a Tuple.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <returns>Message entities.</returns>
        public static (MessageEntityType Type, string Value)[] GetEntities(this Message message)
        {
            var entities = message.Entities;
            var entityValues = message.EntityValues;

            if (entities == null || entityValues == null)
                return Array.Empty<(MessageEntityType Type, string Value)>();

            return entities.Zip(entityValues, (e, ev) => (Entity: e.Type, Value: ev)).ToArray();
        }
    }
}
