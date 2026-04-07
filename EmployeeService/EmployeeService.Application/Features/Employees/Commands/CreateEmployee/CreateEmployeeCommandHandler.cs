using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Configuration;
using EmployeeService.Application.Features.Employees.Events;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace EmployeeService.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthIntegrationClient _authClient;
    private readonly IOptions<AuthServiceOptions> _authOptions;
    private readonly IPublisher _publisher;

    public CreateEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        IAuthIntegrationClient authClient,
        IOptions<AuthServiceOptions> authOptions,
        IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _authClient = authClient;
        _authOptions = authOptions;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (_authOptions.Value.ValidateWithAuthService
            && !await _authClient.ValidateCallerAsync(cancellationToken))
            throw new ForbiddenAppException("Token could not be validated with Auth Service.");

        if (await _unitOfWork.Employees.EmailExistsAsync(request.Dto.Email, null, cancellationToken))
            throw new ConflictException("An employee with this email already exists.");

        if (await _unitOfWork.Departments.GetByIdAsync(request.Dto.DepartmentId, cancellationToken) is null)
            throw new NotFoundException("Department not found.");

        if (await _unitOfWork.Roles.GetByIdAsync(request.Dto.RoleId, cancellationToken) is null)
            throw new NotFoundException("Role not found.");

        if (request.Dto.ManagerId is { } mid)
        {
            if (await _unitOfWork.Employees.GetByIdAsync(mid, cancellationToken: cancellationToken) is null)
                throw new NotFoundException("Manager not found.");
        }

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.Dto.FirstName.Trim(),
            LastName = request.Dto.LastName.Trim(),
            Email = request.Dto.Email.Trim().ToLowerInvariant(),
            Phone = string.IsNullOrWhiteSpace(request.Dto.Phone) ? null : request.Dto.Phone.Trim(),
            DepartmentId = request.Dto.DepartmentId,
            RoleId = request.Dto.RoleId,
            ManagerId = request.Dto.ManagerId,
            DateOfJoining = request.Dto.DateOfJoining,
            Status = request.Dto.Status
        };

        _unitOfWork.Employees.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new EmployeeCreatedNotification(employee.Id), cancellationToken);

        return employee.Id;
    }
}
