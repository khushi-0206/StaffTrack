using AutoMapper;
using LeaveService.Application.Common;
using LeaveService.Application.Common.Exceptions;
using LeaveService.Application.DTOs.LeaveRequests;
using LeaveService.Application.Features.LeaveRequests;
using LeaveService.Application.Interfaces;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveRequests.Queries.GetLeavesForManager;

public class GetLeavesForManagerQueryHandler
    : IRequestHandler<GetLeavesForManagerQuery, IReadOnlyList<LeaveRequestResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetLeavesForManagerQueryHandler(
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

    public async Task<IReadOnlyList<LeaveRequestResponseDto>> Handle(
        GetLeavesForManagerQuery request,
        CancellationToken cancellationToken)
    {
        var callerEmployeeId = await LeaveRequestAuthorization.GetCallerEmployeeIdAsync(
            _employees, _current, cancellationToken);

        if (!LeaveRoles.IsHrOrAdmin(_current.Roles) && callerEmployeeId != request.ManagerId)
            throw new ForbiddenAppException("You can only load team leaves for your own manager profile.");

        var ids = await _employees.GetEmployeeIdsForManagerAsync(request.ManagerId, cancellationToken);
        if (ids.Count == 0)
            return Array.Empty<LeaveRequestResponseDto>();

        var list = await _unitOfWork.LeaveRequests.GetByEmployeeIdsAsync(ids, cancellationToken);
        return _mapper.Map<IReadOnlyList<LeaveRequestResponseDto>>(list);
    }
}
