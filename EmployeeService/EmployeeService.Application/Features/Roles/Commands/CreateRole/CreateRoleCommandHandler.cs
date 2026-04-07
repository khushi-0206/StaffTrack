using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using MediatR;

namespace EmployeeService.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Roles.NameExistsAsync(request.Dto.Name, null, cancellationToken))
            throw new ConflictException("A role with this name already exists.");

        var role = new Role { Name = request.Dto.Name.Trim() };
        _unitOfWork.Roles.Add(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return role.Id;
    }
}
