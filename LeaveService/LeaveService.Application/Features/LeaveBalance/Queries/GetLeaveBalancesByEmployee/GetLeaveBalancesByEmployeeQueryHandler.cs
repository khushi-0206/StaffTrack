using AutoMapper;
using LeaveService.Application.DTOs.LeaveBalance;
using LeaveService.Application.Features.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveBalance.Queries.GetLeaveBalancesByEmployee;

public class GetLeaveBalancesByEmployeeQueryHandler
    : IRequestHandler<GetLeaveBalancesByEmployeeQuery, IReadOnlyList<LeaveBalanceResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetLeaveBalancesByEmployeeQueryHandler(
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

    public async Task<IReadOnlyList<LeaveBalanceResponseDto>> Handle(
        GetLeaveBalancesByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        var team = await _employees.GetEmployeeIdsForManagerAsync(callerEmployeeId, cancellationToken);

        LeaveRequestAuthorization.EnsureCanViewEmployeeLeaves(
            _current, request.EmployeeId, callerEmployeeId, team);

        var list = await _unitOfWork.LeaveBalances.ListByEmployeeAsync(request.EmployeeId, cancellationToken);
        return _mapper.Map<IReadOnlyList<LeaveBalanceResponseDto>>(list);
    }
}
