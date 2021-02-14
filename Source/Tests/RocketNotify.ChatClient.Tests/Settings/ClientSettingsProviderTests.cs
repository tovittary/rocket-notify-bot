namespace RocketNotify.ChatClient.Tests.Settings
{
    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.ChatClient.Settings;

    [TestFixture]
    public class ClientSettingsProviderTests
    {
        private Mock<IConfiguration> _configurationMock;

        private IClientSettingsProvider _settingsProvider;

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _settingsProvider = new ClientSettingsProvider(_configurationMock.Object);
        }

        [Test]
        public void GetServer_NoServerInConfig_ShouldReturnStringEmpty()
        {
            var expected = string.Empty;

            var actual = _settingsProvider.GetServer();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetServer_ServerWithSlashAtEnd_ShouldReturnTrimmed()
        {
            var initial = "http://someserver.com/";
            var expected = initial.TrimEnd('/');
            SetUpConfiguration("RocketChat", "Server", initial);

            var actual = _settingsProvider.GetServer();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetServer_ServerWithoutSlashAtEnd_ShouldReturnAsIs()
        {
            var expected = "http://someserver.com";
            SetUpConfiguration("RocketChat", "Server", expected);

            var actual = _settingsProvider.GetServer();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetGroupName_NoGroupNameInConfig_ShouldReturnStringEmpty()
        {
            var expected = string.Empty;

            var actual = _settingsProvider.GetGroupName();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetGroupName_GroupNameInUpperCase_ShouldReturnInLowerCase()
        {
            var initial = "SomeGroup";
            var expected = initial.ToLowerInvariant();
            SetUpConfiguration("RocketChat", "GroupName", initial);

            var actual = _settingsProvider.GetGroupName();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetGroupName_GroupNameInLowerCase_ShouldReturnAsIs()
        {
            var expected = "somegroup";
            SetUpConfiguration("RocketChat", "GroupName", expected);

            var actual = _settingsProvider.GetGroupName();

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
