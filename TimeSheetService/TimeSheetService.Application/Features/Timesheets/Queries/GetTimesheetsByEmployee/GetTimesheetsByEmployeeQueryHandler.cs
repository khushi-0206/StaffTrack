using AutoMapper;
using TimeSheetService.Application.DTOs.Timesheets;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Timesheets.Queries.GetTimesheetsByEmployee;

public class GetTimesheetsByEmployeeQueryHandler
    : IRequestHandler<GetTimesheetsByEmployeeQuery, IReadOnlyList<TimesheetResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IMapper _mapper;

    public GetTimesheetsByEmployeeQueryHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IMapper mapper)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TimesheetResponseDto>> Handle(
        GetTimesheetsByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        var team = await _employees.GetEmployeeIdsForManagerAsync(callerId, cancellationToken);
        TimesheetAuthorization.EnsureCanViewEmployeeTimesheets(_current, request.EmployeeId, callerId, team);

        var list = await _uow.Timesheets.GetByEmployeeAsync(request.EmployeeId, cancellationToken);
        return _mapper.Map<IReadOnlyList<TimesheetResponseDto>>(list.Where(x => !x.IsDeleted).ToList());
    }
}
