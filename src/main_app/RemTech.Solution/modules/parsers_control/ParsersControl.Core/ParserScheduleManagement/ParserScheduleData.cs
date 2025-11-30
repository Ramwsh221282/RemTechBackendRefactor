namespace ParsersControl.Core.ParserScheduleManagement;

public sealed record ParserScheduleData(Guid Id, DateTime? End, int? WaitDays);