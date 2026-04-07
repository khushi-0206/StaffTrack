using TimeSheetService.Application.Common.Exceptions;
using TimeSheetService.Application.Features.Timesheets;
using TimeSheetService.Application.Configuration;
using TimeSheetService.Application.Interfaces;
using TimeSheetService.Application.Interfaces.Persistence;
using TimeSheetService.Domain.Entities;
using TimeSheetService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace TimeSheetService.Application.Features.Timesheets.Commands.CreateTimesheet;

public class CreateTimesheetCommandHandler : IRequestHandler<CreateTimesheetCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmployeeServiceClient _employees;
    private readonly ICurrentUserService _current;
    private readonly IAuthIntegrationClient _auth;
    private readonly IOptions<AuthServiceOptions> _authOptions;

    public CreateTimesheetCommandHandler(
        IUnitOfWork uow,
        IEmployeeServiceClient employees,
        ICurrentUserService current,
        IAuthIntegrationClient auth,
        IOptions<AuthServiceOptions> authOptions)
    {
        _uow = uow;
        _employees = employees;
        _current = current;
        _auth = auth;
        _authOptions = authOptions;
    }

    public async Task<Guid> Handle(CreateTimesheetCommand request, CancellationToken cancellationToken)
    {
        if (_authOptions.Value.ValidateWithAuthService && !await _auth.ValidateCallerAsync(cancellationToken))
            throw new ForbiddenAppException("Token could not be validated with Auth Service.");

        var callerId = await TimesheetAuthorization.GetCallerEmployeeIdAsync(_employees, _current, cancellationToken);
        TimesheetAuthorization.EnsureCanCreateFor(_current, request.Dto.EmployeeId, callerId);

        var emp = await _employees.GetEmployeeAsync(request.Dto.EmployeeId, cancellationToken);
        if (emp is null)
            throw new NotFoundException("Employee was not found in Employee Service.");

        if (await _uow.Timesheets.ExistsActiveForEmployeeDateAsync(request.Dto.EmployeeId, request.Dto.Date, null, cancellationToken))
            throw new ConflictException("A timesheet already exists for this employee and date.");

        var entity = new Timesheet
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.Dto.EmployeeId,
            Date = request.Dto.Date,
            TotalHours = 0,
            Status = TimesheetStatus.Pending
        };

        _uow.Timesheets.Add(entity);
        await _uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
