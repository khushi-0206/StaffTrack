using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Configuration;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Features.Employees.Commands.CreateEmployee;
using EmployeeService.Application.Features.Employees.Events;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class CreateEmployeeCommandHandlerTests
{
    [Test]
    public async Task Handle_DepartmentNotFound_ThrowsNotFoundException()
    {
        var uow = new Mock<IUnitOfWork>();
        var employees = new Mock<IEmployeeRepository>();
        var departments = new Mock<IDepartmentRepository>();
        var roles = new Mock<IRoleRepository>();

        uow.SetupGet(x => x.Employees).Returns(employees.Object);
        uow.SetupGet(x => x.Departments).Returns(departments.Object);
        uow.SetupGet(x => x.Roles).Returns(roles.Object);

        employees.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        departments.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Department?)null);

        var handler = new CreateEmployeeCommandHandler(
            uow.Object,
            Mock.Of<IAuthIntegrationClient>(),
            Options.Create(new AuthServiceOptions { ValidateWithAuthService = false }),
            Mock.Of<IPublisher>());

        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "A", "B", "a@b.test", null, 5, 1, null, DateTime.UtcNow.AddDays(-1), EmployeeStatus.Active));

        try
        {
            await handler.Handle(cmd, CancellationToken.None);
            Assert.Fail("Expected NotFoundException");
        }
        catch (NotFoundException)
        {
            // expected
        }
    }

    [Test]
    public async Task Handle_EmailAlreadyExists_ThrowsConflictException()
    {
        var uow = new Mock<IUnitOfWork>();
        var employees = new Mock<IEmployeeRepository>();

        uow.SetupGet(x => x.Employees).Returns(employees.Object);
        uow.SetupGet(x => x.Departments).Returns(Mock.Of<IDepartmentRepository>());
        uow.SetupGet(x => x.Roles).Returns(Mock.Of<IRoleRepository>());

        employees.Setup(x => x.EmailExistsAsync("dup@test.local", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateEmployeeCommandHandler(
            uow.Object,
            Mock.Of<IAuthIntegrationClient>(),
            Options.Create(new AuthServiceOptions { ValidateWithAuthService = false }),
            Mock.Of<IPublisher>());

        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "A", "B", "dup@test.local", null, 1, 1, null, DateTime.UtcNow.AddDays(-1), EmployeeStatus.Active));

        try
        {
            await handler.Handle(cmd, CancellationToken.None);
            Assert.Fail("Expected ConflictException");
        }
        catch (ConflictException)
        {
            // expected
        }
    }

    [Test]
    public async Task Handle_ValidRequest_AddsEmployeeAndPublishesEvent()
    {
        var uow = new Mock<IUnitOfWork>();
        var employees = new Mock<IEmployeeRepository>();
        var departments = new Mock<IDepartmentRepository>();
        var roles = new Mock<IRoleRepository>();
        var publisher = new Mock<IPublisher>();

        uow.SetupGet(x => x.Employees).Returns(employees.Object);
        uow.SetupGet(x => x.Departments).Returns(departments.Object);
        uow.SetupGet(x => x.Roles).Returns(roles.Object);

        employees.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        departments.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Department { Id = 1, Name = "Eng" });
        roles.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 2, Name = "Employee" });
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateEmployeeCommandHandler(
            uow.Object,
            Mock.Of<IAuthIntegrationClient>(),
            Options.Create(new AuthServiceOptions { ValidateWithAuthService = false }),
            publisher.Object);

        var cmd = new CreateEmployeeCommand(new CreateEmployeeRequestDto(
            "Sam", "Lee", "sam.lee@test.local", null, 1, 2, null, DateTime.UtcNow.AddMonths(-2), EmployeeStatus.Active));

        var id = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        employees.Verify(x => x.Add(It.Is<Employee>(e =>
            e.FirstName == "Sam" && e.Email == "sam.lee@test.local")), Times.Once);
        publisher.Verify(x => x.Publish(
            It.Is<EmployeeCreatedNotification>(n => n.EmployeeId == id),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
