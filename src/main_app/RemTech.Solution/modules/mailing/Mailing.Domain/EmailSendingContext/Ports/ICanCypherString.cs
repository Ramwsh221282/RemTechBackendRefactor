namespace Mailing.Domain.EmailSendingContext.Ports;

public interface ICanCypherString
{
    string Hash(string input);
    string Open(string hashed);
}