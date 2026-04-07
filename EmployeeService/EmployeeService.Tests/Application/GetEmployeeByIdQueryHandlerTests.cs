using AutoMapper;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Features.Employees.Queries.GetEmployeeById;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;
using Moq;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class GetEmployeeByIdQueryHandlerTests
{
    [Test]
    public async Task Handle_NotFound_ReturnsNull()
    {
        var employees = new Mock<IEmployeeRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Employees).Returns(employees.Object);

        employees.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var mapper = new Mock<IMapper>();
        var handler = new GetEmployeeByIdQueryHandler(uow.Object, mapper.Object);
        var result = await handler.Handle(new GetEmployeeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.That(result, Is.Null);
        mapper.Verify(m => m.Map<EmployeeResponseDto>(It.IsAny<Employee>()), Times.Never);
    }

    [Test]
    public async Task Handle_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var joined = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var created = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var employee = new Employee
        {
            Id = id,
            FirstName = "Alex",
            LastName = "Kim",
            Email = "alex@test.local",
            DepartmentId = 1,
            RoleId = 2,
            DateOfJoining = joined,
            Status = EmployeeStatus.Active,
            CreatedAt = created,
            Department = new Department { Id = 1, Name = "Engineering" },
            Role = new Role { Id = 2, Name = "Employee" }
        };

        var expected = new EmployeeResponseDto(
            id, "Alex", "Kim", "alex@test.local", null, 1, "Engineering", 2, "Employee", null, null, joined,
            EmployeeStatus.Active, created, null);

        var employees = new Mock<IEmployeeRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Employees).Returns(employees.Object);

        employees.Setup(x => x.GetByIdWithDetailsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<EmployeeResponseDto>(employee)).Returns(expected);

        var handler = new GetEmployeeByIdQueryHandler(uow.Object, mapper.Object);
        var dto = await handler.Handle(new GetEmployeeByIdQuery(id), CancellationToken.None);

        Assert.That(dto, Is.SameAs(expected));
        mapper.Verify(m => m.Map<EmployeeResponseDto>(employee), Times.Once);
    }
}
