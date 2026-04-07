using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Features.Employees.Events;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (employee is null || employee.IsDeleted)
            throw new NotFoundException("Employee not found.");

        if (request.Dto.ManagerId == request.Id)
            throw new AppException("An employee cannot be their own manager.");

        if (await _unitOfWork.Employees.EmailExistsAsync(request.Dto.Email, request.Id, cancellationToken))
            throw new ConflictException("An employee with this email already exists.");

        if (await _unitOfWork.Departments.GetByIdAsync(request.Dto.DepartmentId, cancellationToken) is null)
            throw new NotFoundException("Department not found.");

        if (await _unitOfWork.Roles.GetByIdAsync(request.Dto.RoleId, cancellationToken) is null)
            throw new NotFoundException("Role not found.");

        if (request.Dto.ManagerId is { } mid
            && await _unitOfWork.Employees.GetByIdAsync(mid, cancellationToken: cancellationToken) is null)
            throw new NotFoundException("Manager not found.");

        employee.FirstName = request.Dto.FirstName.Trim();
        employee.LastName = request.Dto.LastName.Trim();
        employee.Email = request.Dto.Email.Trim().ToLowerInvariant();
        employee.Phone = string.IsNullOrWhiteSpace(request.Dto.Phone) ? null : request.Dto.Phone.Trim();
        employee.DepartmentId = request.Dto.DepartmentId;
        employee.RoleId = request.Dto.RoleId;
        employee.ManagerId = request.Dto.ManagerId;
        employee.DateOfJoining = request.Dto.DateOfJoining;
        employee.Status = request.Dto.Status;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new EmployeeUpdatedNotification(employee.Id), cancellationToken);

        return Unit.Value;
    }
}
