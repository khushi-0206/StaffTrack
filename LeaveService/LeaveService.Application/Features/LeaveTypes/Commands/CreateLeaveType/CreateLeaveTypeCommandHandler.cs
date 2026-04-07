using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using LeaveService.Domain.Entities;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Commands.CreateLeaveType;

public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _current;

    public CreateLeaveTypeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService current)
    {
        _unitOfWork = unitOfWork;
        _current = current;
    }

    public async Task<int> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        if (!LeaveRoles.IsHrOrAdmin(_current.Roles))
            throw new ForbiddenAppException();

        if (await _unitOfWork.LeaveTypes.NameExistsAsync(request.Dto.Name, null, cancellationToken))
            throw new ConflictException("A leave type with this name already exists.");

        var entity = new LeaveType
        {
            Name = request.Dto.Name.Trim(),
            MaxDays = request.Dto.MaxDays
        };

        _unitOfWork.LeaveTypes.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
