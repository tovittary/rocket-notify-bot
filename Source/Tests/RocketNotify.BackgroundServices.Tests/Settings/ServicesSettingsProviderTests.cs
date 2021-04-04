namespace RocketNotify.BackgroundServices.Tests.Settings
{
    using System;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.BackgroundServices.Settings;

    [TestFixture]
    public class ServicesSettingsProviderTests
    {
        private TimeSpan _defaultInterval = TimeSpan.FromSeconds(6);

        private Mock<IConfiguration> _configurationMock;

        private ServicesSettingsProvider _settingsProvider;

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _settingsProvider = new ServicesSettingsProvider(_configurationMock.Object);
        }

        [Test]
        public void GetMessageCheckInterval_NoSection_ShouldReturnDefaultValue()
        {
            var actual = _settingsProvider.GetMessageCheckInterval();
            Assert.AreEqual(_defaultInterval, actual);
        }

        [Test]
        public void GetMessageCheckInterval_InvalidFormat_ShouldReturnDefaultValue()
        {
            SetUpConfiguration("Notifications", "MessageCheckIntervalSec", "q");

            var actual = _settingsProvider.GetMessageCheckInterval();
            Assert.AreEqual(_defaultInterval, actual);
        }

        [Test]
        public void GetMessageCheckInterval_ValidInterval_ShouldReturnValue()
        {
            var expected = TimeSpan.FromSeconds(30);
            SetUpConfiguration("Notifications", "MessageCheckIntervalSec", "30");

            var actual = _settingsProvider.GetMessageCheckInterval();
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
