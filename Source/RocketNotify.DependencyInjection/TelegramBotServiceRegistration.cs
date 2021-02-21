namespace RocketNotify.DependencyInjection
{
    using Microsoft.Extensions.DependencyInjection;

    using RocketNotify.TelegramBot.Client;
    using RocketNotify.TelegramBot.Client.Factory;
    using RocketNotify.TelegramBot.Commands;
    using RocketNotify.TelegramBot.Messages;
    using RocketNotify.TelegramBot.Messages.Filtration;
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

            services.AddTransient<IInitialMessageFilter, MessageTypeMessageFilter>();
            services.AddTransient<IMessageFilter, ChatTypeMessageFilter>();
            services.AddTransient<IMessageFilter, BotMentionMessageFilter>();
            services.AddTransient<IMessageFilter, ContainsCommandMessageFilter>();
            services.AddTransient<IBotMessageHandler, BotMessageHandler>();

            services.AddTransient<ITelegramBotClientFactory, TelegramBotClientFactory>();
            services.AddSingleton<ITelegramMessagePollingClient, TelegramBotPollingClient>();
            services.AddSingleton<ITelegramMessageSender>(srv => srv.GetRequiredService<ITelegramMessagePollingClient>());
        }
    }
}
