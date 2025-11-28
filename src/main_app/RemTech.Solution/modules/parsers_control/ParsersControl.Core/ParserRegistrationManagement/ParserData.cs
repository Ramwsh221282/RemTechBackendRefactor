namespace ParsersControl.Core.ParserRegistrationManagement;

public sealed record ParserData(
    Guid Id, 
    string Type, 
    string Domain);