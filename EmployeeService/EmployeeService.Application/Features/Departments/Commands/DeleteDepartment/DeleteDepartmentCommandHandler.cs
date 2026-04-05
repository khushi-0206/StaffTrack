using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var dept = await _unitOfWork.Departments.GetByIdAsync(request.Id, cancellationToken);
        if (dept is null)
            throw new NotFoundException("Department not found.");

        var count = await _unitOfWork.Departments.CountEmployeesAsync(request.Id, cancellationToken);
        if (count > 0)
            throw new ConflictException("Cannot delete a department that still has employees.");

        _unitOfWork.Departments.Remove(dept);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
