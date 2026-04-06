using AutoMapper;
using LeaveService.Application.Common;
using LeaveService.Application.Common.Constants;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeaveById;

public class GetLeaveByIdQueryHandler : IRequestHandler<GetLeaveByIdQuery, LeaveRequestResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetLeaveByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _employees = employees;
        _current = current;
        _mapper = mapper;
    }

    public async Task<LeaveRequestResponseDto?> Handle(GetLeaveByIdQuery request, CancellationToken cancellationToken)
    {
        var leave = await _unitOfWork.LeaveRequests.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (leave is null || leave.IsDeleted)
            return null;

        if (LeaveRoles.IsHrOrAdmin(_current.Roles))
            return _mapper.Map<LeaveRequestResponseDto>(leave);

        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        if (leave.EmployeeId == callerEmployeeId)
            return _mapper.Map<LeaveRequestResponseDto>(leave);

        if (LeaveRoles.IsManager(_current.Roles))
        {
            var team = await _employees.GetEmployeeIdsForManagerAsync(callerEmployeeId, cancellationToken);
            if (team.Contains(leave.EmployeeId))
                return _mapper.Map<LeaveRequestResponseDto>(leave);
        }

        throw new ForbiddenAppException();
    }
}
