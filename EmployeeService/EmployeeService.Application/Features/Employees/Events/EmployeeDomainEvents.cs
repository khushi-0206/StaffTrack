using MediatR;

namespace EmployeeService.Application.Features.Employees.Events;

/// <summary>Optional integration hook (logging, messaging, analytics).</summary>
public record EmployeeCreatedNotification(Guid EmployeeId) : INotification;

public record EmployeeUpdatedNotification(Guid EmployeeId) : INotification;
