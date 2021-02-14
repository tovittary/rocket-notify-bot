namespace RocketNotify.Subscription.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;

    using NUnit.Framework;

    using RocketNotify.Subscription.Data;
    using RocketNotify.Subscription.Exceptions;
    using RocketNotify.Subscription.Model;

    [TestFixture]
    public class JsonSubscribersRepositoryTests
    {
        private Mock<IFileStorage> _storageMock;

        private JsonSubscribersRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _storageMock = new Mock<IFileStorage>();
            _storageMock.Setup(x => x.Initialize());
            _storageMock.Setup(x => x.SaveSubscribersData(It.IsAny<ICollection<Subscriber>>()));
            _storageMock.Setup(x => x.LoadSubscribersDataAsync()).ReturnsAsync(Array.Empty<Subscriber>());

            _repository = new JsonSubscribersRepository(_storageMock.Object);
        }

        [Test]
        public async Task CreateSubscriberAsync_NewSubscriber_ShouldCreate()
        {
            var expected = 12345;
            await _repository.CreateSubscriberAsync(expected).ConfigureAwait(false);

            var actual = await _repository.GetAllSubscribersAsync().ConfigureAwait(false);

            Assert.AreEqual(expected, actual.First().ChatId);

            _storageMock.Verify(x => x.Initialize(), Times.Once);
            _storageMock.Verify(x => x.LoadSubscribersDataAsync(), Times.Once);
            _storageMock.Verify(x => x.SaveSubscribersData(It.IsAny<ICollection<Subscriber>>()), Times.Once);
        }

        [Test]
        public async Task CreateSubscriberAsync_ExistingSubsriber_ShouldThrowException()
        {
            var chatId = 12345;
            await _repository.CreateSubscriberAsync(chatId).ConfigureAwait(false);

            Assert.ThrowsAsync<SubscriberAlreadyExistsException>(() => _repository.CreateSubscriberAsync(chatId));

            _storageMock.Verify(x => x.Initialize(), Times.Once);
            _storageMock.Verify(x => x.LoadSubscribersDataAsync(), Times.Once);
            _storageMock.Verify(x => x.SaveSubscribersData(It.IsAny<ICollection<Subscriber>>()), Times.Once);
        }

        [Test]
        public async Task DeleteSubscriberAsync_SubscriberExists_ShouldDelete()
        {
            var chatId = 12345;
            await _repository.CreateSubscriberAsync(chatId).ConfigureAwait(false);
            await _repository.DeleteSubscriberAsync(chatId).ConfigureAwait(false);

            var actual = await _repository.GetAllSubscribersAsync().ConfigureAwait(false);
            Assert.IsEmpty(actual);

            _storageMock.Verify(x => x.Initialize(), Times.Once);
            _storageMock.Verify(x => x.LoadSubscribersDataAsync(), Times.Once);
            _storageMock.Verify(x => x.SaveSubscribersData(It.IsAny<ICollection<Subscriber>>()), Times.Exactly(2));
        }

        [Test]
        public async Task DeleteSubscriberAsync_SubscriberNotFound_ShouldThrowException()
        {
            var chatId = 12345;
            await _repository.CreateSubscriberAsync(chatId).ConfigureAwait(false);
            await _repository.DeleteSubscriberAsync(chatId).ConfigureAwait(false);

            Assert.ThrowsAsync<SubscriberNotFoundException>(() => _repository.DeleteSubscriberAsync(chatId));

            _storageMock.Verify(x => x.Initialize(), Times.Once);
            _storageMock.Verify(x => x.LoadSubscribersDataAsync(), Times.Once);
            _storageMock.Verify(x => x.SaveSubscribersData(It.IsAny<ICollection<Subscriber>>()), Times.Exactly(2));
        }
    }
}
