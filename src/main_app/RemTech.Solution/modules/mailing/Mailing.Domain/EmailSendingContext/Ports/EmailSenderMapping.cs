namespace Mailing.Domain.EmailSendingContext.Ports;

public delegate T EmailSenderIdSink<out T>(Guid id);

public delegate T EmailSenderEmailSink<out T>(string email);

public delegate T EmailSenderServiceSink<out T>(string service, string password);

public delegate T EmailSenderServiceStatisticsSink<out T>(int limit, int currentSent);

public delegate T EmailSenderDataSink<out T>(
    Guid id,
    string email,
    string service,
    string password,
    int sendLimit,
    int currentSent
);