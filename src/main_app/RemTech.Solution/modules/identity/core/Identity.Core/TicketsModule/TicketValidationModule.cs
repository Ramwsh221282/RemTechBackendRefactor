namespace Identity.Core.TicketsModule;

public static class TicketValidationModule
{
    public const int MaxTypeLength = 128;

    extension(Ticket ticket)
    {
        public Result<Ticket> Validated()
        {
            Guid id = ticket.Id;
            string type = ticket.Type;
            if (Guids.Empty(id)) return Validation("Идентификатор заявки пустой.");
            if (Strings.EmptyOrWhiteSpace(type)) return Validation("Тип заявки пустой.");
            if (Strings.GreaterThan(type, MaxTypeLength)) return Validation("Тип заявки невалиден.");
            return ticket;
        }
    }
}