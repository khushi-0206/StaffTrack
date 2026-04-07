using NotificationService.Domain.Entities;
using FluentAssertions;
namespace NotificationService.Tests.Application.Templates
{
    public class TemplateTests
    {
        [Test]
        public void Should_Create_Template_Object()
        {
            var template = new NotificationTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Subject = "Hello",
                Body = "World"
            };

            template.Should().NotBeNull();
            template.Body.Should().Be("World");
        }
    }
}
