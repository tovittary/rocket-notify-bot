namespace RocketNotify.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Commands;
    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Start;
    using RocketNotify.TelegramBot.MessageProcessing.Subscribe;
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

            RegisterMessageProcessing(services);
            RegisterMessageFilters(services);
            services.AddTransient<IBotMessageHandler, BotMessageHandler>();

            services.AddTransient<ITelegramBotClientFactory, TelegramBotClientFactory>();
            services.AddSingleton<ITelegramMessagePollingClient, TelegramBotPollingClient>();
            services.AddSingleton<ITelegramMessageSender>(srv => srv.GetRequiredService<ITelegramMessagePollingClient>());
        }

        /// <summary>
        /// Registers message processing services.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterMessageProcessing(IServiceCollection services)
        {
            services.AddTransient<IMessageProcessor, StartCommandProcessor>();
            RegisterSubscriptionCommandProcessing(services);
            services.AddTransient<Func<IEnumerable<IMessageProcessor>>>(sp => sp.GetServices<IMessageProcessor>);

            services.AddSingleton<IMessageProcessorStorage, MessageProcessorStorage>();
            services.AddTransient<IMessageProcessorFactory, MessageProcessorFactory>();
            services.AddTransient<IMessageProcessorInvoker, MessageProcessorInvoker>();
        }

        /// <summary>
        /// Registers subscription command processing services.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterSubscriptionCommandProcessing(IServiceCollection services)
        {
            services.AddTransient<SubscriptionCompleteState>();
            services.AddTransient<Func<SubscriptionCompleteState>>(sp => sp.GetRequiredService<SubscriptionCompleteState>);
            services.AddTransient<VerifySubscriptionState>();
            services.AddTransient<Func<VerifySubscriptionState>>(sp => sp.GetRequiredService<VerifySubscriptionState>);
            services.AddTransient<InitialSubscribeState>();
            services.AddTransient<Func<InitialSubscribeState>>(sp => sp.GetRequiredService<InitialSubscribeState>);

            services.AddTransient<IMessageProcessor, SubscribeCommandProcessor>();
        }

        /// <summary>
        /// Registers message filters in the correct order.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterMessageFilters(IServiceCollection services)
        {
            services.AddTransient<IChainedMessageFilter, MessageTypeMessageFilter>();
            services.AddTransient<IChainedMessageFilter, ChatTypeMessageFilter>();
            services.AddTransient<IChainedMessageFilter, RelevantMessageFilter>();
            services.AddTransient<IChainedMessageFilter, BotMentionMessageFilter>();
            services.AddTransient<IChainedMessageFilter, ContainsCommandMessageFilter>();
            services.AddTransient<Func<IEnumerable<IChainedMessageFilter>>>(sp => sp.GetServices<IChainedMessageFilter>);

            services.AddTransient<IMessageFilterFactory, MessageFilterFactory>();
        }
    }
}
