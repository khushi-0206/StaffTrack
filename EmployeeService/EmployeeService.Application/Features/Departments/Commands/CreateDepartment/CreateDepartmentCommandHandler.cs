using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Departments.NameExistsAsync(request.Dto.Name, null, cancellationToken))
            throw new ConflictException("A department with this name already exists.");

        var entity = new Department
        {
            Name = request.Dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Dto.Description)
                ? null
                : request.Dto.Description.Trim()
        };

        _unitOfWork.Departments.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
