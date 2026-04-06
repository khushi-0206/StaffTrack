using AutoMapper;
using LeaveService.Application.DTOs.LeaveTypes;
using LeaveService.Application.Features.LeaveTypes.Queries.GetLeaveTypes;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Application.Mapping;
using LeaveService.Domain.Entities;
using Moq;

namespace LeaveService.Tests.Application;

[TestFixture]
public class GetLeaveTypesQueryHandlerTests
{
    [Test]
    public async Task Handle_ReturnsMappedLeaveTypes()
    {
        var entities = new List<LeaveType>
        {
            new()
            {
                Id = 1,
                Name = "Sick",
                MaxDays = 10,
                CreatedAt = DateTime.UtcNow
            }
        };

        var typeRepo = new Mock<ILeaveTypeRepository>();
        typeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.LeaveTypes).Returns(typeRepo.Object);

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        var handler = new GetLeaveTypesQueryHandler(uow.Object, mapper);

        var result = await handler.Handle(new GetLeaveTypesQuery(), CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Sick"));
        Assert.That(result[0], Is.TypeOf<LeaveTypeResponseDto>());
    }
}
