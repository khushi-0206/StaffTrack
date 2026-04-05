using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var dept = await _unitOfWork.Departments.GetByIdAsync(request.Id, cancellationToken);
        if (dept is null)
            throw new NotFoundException("Department not found.");

        if (await _unitOfWork.Departments.NameExistsAsync(request.Dto.Name, request.Id, cancellationToken))
            throw new ConflictException("A department with this name already exists.");

        dept.Name = request.Dto.Name.Trim();
        dept.Description = string.IsNullOrWhiteSpace(request.Dto.Description)
            ? null
            : request.Dto.Description.Trim();

        _unitOfWork.Departments.Update(dept);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
