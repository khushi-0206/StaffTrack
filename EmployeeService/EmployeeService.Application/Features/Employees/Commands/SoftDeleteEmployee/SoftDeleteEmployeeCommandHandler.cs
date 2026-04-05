using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Commands.SoftDeleteEmployee;

public class SoftDeleteEmployeeCommandHandler : IRequestHandler<SoftDeleteEmployeeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SoftDeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (employee is null || employee.IsDeleted)
            throw new NotFoundException("Employee not found.");

        employee.IsDeleted = true;
        employee.DeletedAt = DateTime.UtcNow;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
