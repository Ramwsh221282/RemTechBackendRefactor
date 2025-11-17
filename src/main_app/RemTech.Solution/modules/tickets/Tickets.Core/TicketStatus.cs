using RemTech.Primitives.Extensions.SmartEnumerations;

namespace Tickets.Core;

[SmartEnumeration<TicketStatus>]
public abstract record TicketStatus(string Name)
{
    [SmartEnumeration<TicketStatus>]
    public record Activated() : TicketStatus("Activated")
    {
        public override Result<TicketStatus> Change(TicketStatus other)
        {
            return Conflict("Нельзя изменить активированный статус заявки.");
        }
    }
    
    [SmartEnumeration<TicketStatus>]
    public record Created() : TicketStatus("Created")
    {
        public override Result<TicketStatus> Change(TicketStatus other)
        {
            if (other is not Pending) return Conflict("Созданная заявка может перейти только в состояние ожидания.");
            return other;
        }
    }
    
    [SmartEnumeration<TicketStatus>]
    public record Pending() : TicketStatus("Pending")
    {
        public override Result<TicketStatus> Change(TicketStatus other)
        {
            if (other is not Activated) return Conflict("Ожидающая заявка может быть только активирована.");
            return other;
        }
    }

    public abstract Result<TicketStatus> Change(TicketStatus other);
}