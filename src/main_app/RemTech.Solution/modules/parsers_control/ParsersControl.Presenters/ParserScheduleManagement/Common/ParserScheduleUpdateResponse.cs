using ParsersControl.Core.ParserScheduleManagement;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserScheduleManagement.Common;

public sealed class ParserScheduleUpdateResponse : IResponse
{
    public Guid Id { get; private set; }
    private void AddId(Guid id) => Id = id;
    public ParserScheduleUpdateResponse(ParserSchedule schedule) => schedule.Write(AddId);
}