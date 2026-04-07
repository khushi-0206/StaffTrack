using Moq;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Features.Notifications.Commands.SendNotification;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using FluentAssertions;

namespace NotificationService.Tests.Application.Notifications
{
    public class SendNotificationHandlerTests
    {
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IEmailService> _emailMock;
        private Mock<IGenericRepository<Notification>> _notificationRepoMock;
        private SendNotificationHandler _handler;

        [SetUp]
        public void Setup()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _emailMock = new Mock<IEmailService>();
            _notificationRepoMock = new Mock<IGenericRepository<Notification>>();

            _uowMock.Setup(x => x.Notifications)
                    .Returns(_notificationRepoMock.Object);

            _uowMock.Setup(x => x.SaveChangesAsync())
                    .ReturnsAsync(1);

            _handler = new SendNotificationHandler(_uowMock.Object, _emailMock.Object);
        }

        [Test]
        public async Task Should_Create_Notification_Successfully()
        {
            var command = new SendNotificationCommand(
                "Test",
                "Hello",
                NotificationType.Email,
                Guid.NewGuid()
            );

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeEmpty();

            _notificationRepoMock.Verify(
                x => x.AddAsync(It.IsAny<Notification>()),
                Times.Once
            );
        }
    }
}
