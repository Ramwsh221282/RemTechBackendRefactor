using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.Ports;

public delegate Task<Status<EmailSender>> EmailSenderByid(Guid id, CancellationToken ct = default);

public delegate Task<Status<EmailSender>> AvailableSender(CancellationToken ct = default);

public delegate Task<IEnumerable<EmailSender>> GetAll(CancellationToken ct = default);