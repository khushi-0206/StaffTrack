namespace LeaveService.Domain.Enums;

/// <summary>Workflow state for a leave application.</summary>
public enum LeaveRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}
