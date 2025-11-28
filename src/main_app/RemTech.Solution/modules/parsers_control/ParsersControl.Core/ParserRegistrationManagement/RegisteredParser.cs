namespace ParsersControl.Core.ParserRegistrationManagement;

public sealed class RegisteredParser(ParserData data)
{
    public void Write(
        Action<Guid>? writeId = null,
        Action<string>? writeDomain = null,
        Action<string>? writeType = null)
    {
        writeId?.Invoke(data.Id);
        writeDomain?.Invoke(data.Domain);
        writeType?.Invoke(data.Type);
    }
}