using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.LeaveTypes.Commands.UpdateLeaveType;

public class UpdateLeaveTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLeaveTypeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.LeaveTypes.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Leave type not found.");

        entity.Name = request.Dto.Name.Trim();
        entity.MaxDays = request.Dto.MaxDays;

        _unitOfWork.LeaveTypes.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
