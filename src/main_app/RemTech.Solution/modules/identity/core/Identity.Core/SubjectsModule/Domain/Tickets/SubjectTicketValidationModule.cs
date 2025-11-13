using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Domain.Tickets;

public static class SubjectTicketValidationModule
{
    public const int MaxTypeLength = 128;

    extension(SubjectTicket ticket)
    {
        public Result<SubjectTicket> Validated()
        {
            Guid id = ticket._id;
            string type = ticket._type;
            if (Guids.Empty(id)) return Validation("Идентификатор заявки пустой.");
            if (Strings.EmptyOrWhiteSpace(type)) return Validation("Тип заявки пустой.");
            if (Strings.GreaterThan(type, 128)) return Validation("Тип заявки невалиден.");
            return ticket;
        }
    }
}