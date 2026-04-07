using Moq;
using NotificationService.Application.Features.Notifications.Commands.MarkAsRead;
using NotificationService.Domain.Entities;
using NotificationService.Application.Common.Interfaces;
using FluentAssertions;

namespace NotificationService.Tests.Application.Notifications
{
    public class MarkNotificationReadTests
    {
        private Mock<IUnitOfWork> _uowMock;
        private MarkNotificationReadHandler _handler;

        [SetUp]
        public void Setup()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _handler = new MarkNotificationReadHandler(_uowMock.Object);
        }

        [Test]
        public async Task Should_Mark_Notification_As_Read()
        {
            var id = Guid.NewGuid();

            var notification = new Notification { Id = id, IsRead = false };

            _uowMock.Setup(x => x.Notifications.GetByIdAsync(id))
                    .ReturnsAsync(notification);

            var result = await _handler.Handle(new MarkNotificationReadCommand(id), CancellationToken.None);

            result.Should().BeTrue();
            notification.IsRead.Should().BeTrue();
        }
    }
}
