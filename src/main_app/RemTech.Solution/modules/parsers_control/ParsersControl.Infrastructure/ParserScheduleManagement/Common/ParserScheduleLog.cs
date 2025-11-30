using ParsersControl.Core.ParserScheduleManagement;

namespace ParsersControl.Infrastructure.ParserScheduleManagement.Common;

public sealed class ParserScheduleLog
{
    private readonly Serilog.ILogger _logger;
    private Guid _id;
    private DateTime? _endDate;
    private int? _daysCount;
    
    private void AddId(Guid id) => _id = id;
    
    private void AddEndDate(DateTime? endDate) => _endDate = endDate;
    
    private void AddDays(int? days) => _daysCount = days;

    public void Log()
    {   
        string[] properties = 
        [
            _id.ToString(),
            _endDate == null ? "Дата окончания неизвестна" : _endDate.Value.ToString("yyyy-MM-dd"),
            _daysCount == null ? "Дни ожидания не заданы" : _daysCount.Value.ToString()
        ];
        
        _logger.Information("""
                            Parser schedule info:
                            Id: {Id}
                            End date: {End}
                            Wait days: {WaitDays}
                            """, properties);
    }
    
    public ParserScheduleLog(Serilog.ILogger logger, ParserScheduleData data)
    {
        _logger = logger;
        new ParserSchedule(data).Write(AddId, AddEndDate, AddDays);
    }
}