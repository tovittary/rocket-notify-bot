namespace RocketNotify.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.MessageProcessing;
    using RocketNotify.TelegramBot.MessageProcessing.Commands;
    using RocketNotify.TelegramBot.MessageProcessing.Help;
    using RocketNotify.TelegramBot.MessageProcessing.Start;
    using RocketNotify.TelegramBot.MessageProcessing.Subscribe;
    using RocketNotify.TelegramBot.MessageProcessing.Unsubscribe;
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
            RegisterSubscriptionCommandProcessingStates(services);
            RegisterCommandsDescriptionServices(services);
            RegisterMessageProcessors(services);

            services.AddSingleton<IMessageProcessorStorage, MessageProcessorStorage>();
            services.AddTransient<IMessageProcessorFactory, MessageProcessorFactory>();
            services.AddTransient<IMessageProcessorInvoker, MessageProcessorInvoker>();
        }

        /// <summary>
        /// Registers subscription command processing states.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterSubscriptionCommandProcessingStates(IServiceCollection services)
        {
            services.AddTransient<SubscriptionCompleteState>();
            services.AddTransient<Func<SubscriptionCompleteState>>(sp => sp.GetRequiredService<SubscriptionCompleteState>);
            services.AddTransient<VerifySubscriptionState>();
            services.AddTransient<Func<VerifySubscriptionState>>(sp => sp.GetRequiredService<VerifySubscriptionState>);
            services.AddTransient<InitialSubscribeState>();
            services.AddTransient<Func<InitialSubscribeState>>(sp => sp.GetRequiredService<InitialSubscribeState>);
        }

        /// <summary>
        /// Registers services used to obtain info on available commands.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterCommandsDescriptionServices(IServiceCollection services)
        {
            services.AddTransient<ICommandDescriptionProvider, StartCommandProcessor>();
            services.AddTransient<ICommandDescriptionProvider, UnsubscribeCommandProcessor>();
            services.AddTransient<ICommandDescriptionProvider, SubscribeCommandProcessor>();

            services.AddSingleton<ICommandInfoAggregator, CommandInfoAggregator>();
        }

        /// <summary>
        /// Registers message processors.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        private void RegisterMessageProcessors(IServiceCollection services)
        {
            services.AddTransient<IMessageProcessor, StartCommandProcessor>();
            services.AddTransient<IMessageProcessor, UnsubscribeCommandProcessor>();
            services.AddTransient<IMessageProcessor, SubscribeCommandProcessor>();
            services.AddTransient<IMessageProcessor, HelpCommandProcessor>();

            services.AddTransient<Func<IEnumerable<IMessageProcessor>>>(sp => sp.GetServices<IMessageProcessor>);
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
