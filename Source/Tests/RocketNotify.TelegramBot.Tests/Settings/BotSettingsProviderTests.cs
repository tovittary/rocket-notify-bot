namespace RocketNotify.TelegramBot.Tests.Settings
{
    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Settings;

    [TestFixture]
    public class BotSettingsProviderTests
    {
        private Mock<IConfiguration> _configurationMock;

        private BotSettingsProvider _settingsProvider;

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _settingsProvider = new BotSettingsProvider(_configurationMock.Object);
        }

        [Test]
        public void GetBotUserName_NoSection_ShouldReturnStringEmpty()
        {
            var actual = _settingsProvider.GetBotUserName();

            Assert.AreEqual(string.Empty, actual);
        }

        [Test]
        public void GetBotUserName_StringEmpty_ShouldReturnStringEmpty()
        {
            SetUpConfiguration("Telegram", "BotName", string.Empty);

            var actual = _settingsProvider.GetBotUserName();

            Assert.AreEqual(string.Empty, actual);
        }

        [Test]
        public void GetBotUserName_BotWithAtSign_ShouldReturnAsIs()
        {
            var expected = "@testName_bot";
            SetUpConfiguration("Telegram", "BotName", expected);

            var actual = _settingsProvider.GetBotUserName();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetBotUserName_BotWithoutAtSign_ShouldAddPrefixAndReturn()
        {
            var botName = "testName_bot";
            var expected = $"@{botName}";
            SetUpConfiguration("Telegram", "BotName", botName);

            var actual = _settingsProvider.GetBotUserName();

            Assert.AreEqual(expected, actual);
        }

        private void SetUpConfiguration(string sectionName, string key, string value)
        {
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.Setup(x => x[key]).Returns(value);

            _configurationMock.Setup(x => x.GetSection(sectionName)).Returns(sectionMock.Object);
        }
    }
}
