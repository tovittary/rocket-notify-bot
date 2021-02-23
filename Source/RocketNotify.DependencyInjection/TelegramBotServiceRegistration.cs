namespace RocketNotify.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Commands;
    using RocketNotify.TelegramBot.Messages;
    using RocketNotify.TelegramBot.Messages.Filtration;
    using RocketNotify.TelegramBot.Messages.Filtration.Factory;
    using RocketNotify.TelegramBot.Settings;

    /// <summary>
    /// Registers telegram bot services.
    /// </summary>
    public class TelegramBotServiceRegistration : IServiceRegistration
    {
        /// <inheritdoc />
        public void Register(IServiceCollection services)
        {
            services.AddTransient<ICommand, HelpCommand>();
            services.AddTransient<ICommand, StartCommand>();
            services.AddTransient<ICommand, SubscribeCommand>();
            services.AddTransient<ICommand, UnsubscribeCommand>();

            services.AddTransient<IBotSettingsProvider, BotSettingsProvider>();
            services.AddTransient<IBotMessageProcessor, BotMessageProcessor>();

            RegisterMessageFilters(services);
            services.AddTransient<IBotMessageHandler, BotMessageHandler>();

            services.AddTransient<ITelegramBotClientFactory, TelegramBotClientFactory>();
            services.AddSingleton<ITelegramMessagePollingClient, TelegramBotPollingClient>();
            services.AddSingleton<ITelegramMessageSender>(srv => srv.GetRequiredService<ITelegramMessagePollingClient>());
        }

        /// <summary>
        /// Registers message filters in the correct order.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterMessageFilters(IServiceCollection services)
        {
            services.AddTransient<IChainedMessageFilter, MessageTypeMessageFilter>();
            services.AddTransient<IChainedMessageFilter, ChatTypeMessageFilter>();
            services.AddTransient<IChainedMessageFilter, BotMentionMessageFilter>();
            services.AddTransient<IChainedMessageFilter, ContainsCommandMessageFilter>();
            services.AddTransient<Func<IEnumerable<IChainedMessageFilter>>>(sp => sp.GetServices<IChainedMessageFilter>);

            services.AddTransient<IMessageFilterFactory, MessageFilterFactory>();
        }
    }
}
