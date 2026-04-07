using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using MediatR;

namespace EmployeeService.Application.Features.LeaveTypes.Commands.CreateLeaveType;

public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeaveTypeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
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
