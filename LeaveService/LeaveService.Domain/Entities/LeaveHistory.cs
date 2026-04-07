using LeaveService.Domain.Enums;

namespace LeaveService.Domain.Entities;

public class LeaveHistory
{
    public int Id { get; set; }

    public Guid LeaveRequestId { get; set; }

    public LeaveRequest LeaveRequest { get; set; } = null!;

    public LeaveHistoryAction Action { get; set; }

    /// <summary>Auth user or employee id performing the action (typically employee Guid).</summary>
    public Guid ActionBy { get; set; }

    public DateTime ActionDate { get; set; }

    public string? Remarks { get; set; }
}
