namespace RocketNotify.TelegramBot.Tests.Filtration.Factory
{
    using System;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.TelegramBot.Filtration;
    using RocketNotify.TelegramBot.Filtration.Factory;

    [TestFixture]
    public class MessageFilterFactoryTests
    {
        private Mock<IChainedMessageFilter> _firstFilter;

        private Mock<IChainedMessageFilter> _secondFilter;

        private Mock<IChainedMessageFilter> _thirdFilter;

        [SetUp]
        public void SetUp()
        {
            _firstFilter = new Mock<IChainedMessageFilter>();
            _secondFilter = new Mock<IChainedMessageFilter>();
            _thirdFilter = new Mock<IChainedMessageFilter>();
        }

        [Test]
        public void GetFilter_NoFilters_ShouldThrowException()
        {
            var factory = new MessageFilterFactory(() => Array.Empty<IChainedMessageFilter>());

            Assert.Throws<ArgumentException>(() => factory.GetFilter());
        }

        [Test]
        public void GetFilter_HasOneFilter_ShouldReturnOneFilter()
        {
            var factory = new MessageFilterFactory(() => new[] { _firstFilter.Object });

            var actual = factory.GetFilter();

            Assert.AreEqual(_firstFilter.Object, actual);
            _firstFilter.Verify(x => x.SetNextFilter(It.IsAny<IMessageFilter>()), Times.Never);
        }

        [Test]
        public void GetFilter_HasFilters_ShouldSetUpChainAndReturn()
        {
            var factory = new MessageFilterFactory(
                () => new[] { _firstFilter.Object, _secondFilter.Object, _thirdFilter.Object });

            var actual = factory.GetFilter();

            Assert.AreEqual(_firstFilter.Object, actual);
            _firstFilter.Verify(x => x.SetNextFilter(_secondFilter.Object), Times.Once);
            _secondFilter.Verify(x => x.SetNextFilter(_thirdFilter.Object), Times.Once);
            _thirdFilter.Verify(x => x.SetNextFilter(It.IsAny<IMessageFilter>()), Times.Never);
        }
    }
}
