using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Features.Employees.Commands.SoftDeleteEmployee;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using EmployeeService.Domain.Enums;
using Moq;

namespace EmployeeService.Tests.Application;

[TestFixture]
public class SoftDeleteEmployeeCommandHandlerTests
{
    [Test]
    public async Task Handle_EmployeeMissing_ThrowsNotFoundException()
    {
        var employees = new Mock<IEmployeeRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Employees).Returns(employees.Object);

        employees.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var handler = new SoftDeleteEmployeeCommandHandler(uow.Object);
        var cmd = new SoftDeleteEmployeeCommand(Guid.NewGuid());

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
    public async Task Handle_ValidRequest_MarksDeletedAndSaves()
    {
        var empId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = empId,
            FirstName = "X",
            LastName = "Y",
            Email = "x@y.test",
            DepartmentId = 1,
            RoleId = 1,
            DateOfJoining = DateTime.UtcNow,
            Status = EmployeeStatus.Active,
            IsDeleted = false
        };

        var employees = new Mock<IEmployeeRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.SetupGet(x => x.Employees).Returns(employees.Object);

        employees.Setup(x => x.GetByIdAsync(empId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new SoftDeleteEmployeeCommandHandler(uow.Object);

        await handler.Handle(new SoftDeleteEmployeeCommand(empId), CancellationToken.None);

        Assert.That(employee.IsDeleted, Is.True);
        Assert.That(employee.DeletedAt, Is.Not.Null);
        employees.Verify(x => x.Update(employee), Times.Once);
        uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
