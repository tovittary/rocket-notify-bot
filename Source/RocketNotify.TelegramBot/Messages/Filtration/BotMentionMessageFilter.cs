﻿namespace RocketNotify.TelegramBot.Messages.Filtration
{
    using System;
    using System.Linq;

    using RocketNotify.TelegramBot.Settings;

    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    /// <summary>
    /// Filters a message based on whether the bot is mentioned in it.
    /// </summary>
    public class BotMentionMessageFilter : IMessageFilter
    {
        /// <summary>
        /// Bot settings provider.
        /// </summary>
        private readonly IBotSettingsProvider _settingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotMentionMessageFilter"/> class.
        /// </summary>
        /// <param name="settingProvider">Bot settings provider.</param>
        public BotMentionMessageFilter(IBotSettingsProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        /// <inheritdoc/>
        public FiltrationResult Filter(Message message)
        {
            var botUserName = _settingProvider.GetBotUserName();
            if (string.IsNullOrEmpty(botUserName))
            {
                // TODO Logging
                throw new InvalidOperationException("The bot name must be provided for it to accept commands from group chats");
            }

            var entities = message.GetEntities();
            var mentions = entities.Where(entity => entity.Type == MessageEntityType.Mention);
            var botMentionFound = mentions.Any(m => m.Value.Equals(botUserName, StringComparison.InvariantCultureIgnoreCase));
            if (botMentionFound)
                return FiltrationResult.NextFilter(typeof(ContainsCommandMessageFilter));

            var commands = entities.Where(entity => entity.Type == MessageEntityType.BotCommand);
            var commandHasMention = commands.Any(c => c.Value.Contains(botUserName, StringComparison.InvariantCultureIgnoreCase));
            if (commandHasMention)
                return FiltrationResult.NextFilter(typeof(ContainsCommandMessageFilter));

            return FiltrationResult.Ignore();
        }
    }
}
