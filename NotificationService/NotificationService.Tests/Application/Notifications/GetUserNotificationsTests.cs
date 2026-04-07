using AutoMapper;
using Moq;
using NotificationService.Application.Features.Notifications.Queries.GetUserNotification;
using NotificationService.Domain.Entities;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Mappings;
using FluentAssertions;

namespace NotificationService.Tests.Application.Notifications
{
    public class GetUserNotificationsTests
    {
        private Mock<IUnitOfWork> _uowMock;
        private IMapper _mapper;
        private GetUserNotificationsHandler _handler;

        [SetUp]
        public void Setup()
        {
            _uowMock = new Mock<IUnitOfWork>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();

            _handler = new GetUserNotificationsHandler(_uowMock.Object, _mapper);
        }

        [Test]
        public async Task Should_Return_User_Notifications()
        {
            var userId = Guid.NewGuid();

            var data = new List<Notification>
        {
            new Notification { Id = Guid.NewGuid(), RecipientId = userId },
            new Notification { Id = Guid.NewGuid(), RecipientId = Guid.NewGuid() }
        };

            _uowMock.Setup(x => x.Notifications.GetAllAsync()).ReturnsAsync(data);

            var result = await _handler.Handle(new GetUserNotificationsQuery(userId), CancellationToken.None);

            result.Count.Should().Be(1);
        }
    }
}
