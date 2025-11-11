namespace Identity.Core.SubjectsModule.Models;

public sealed record IdentitySubject
{
    private readonly IdentitySubjectMetadata _metaData;
    private readonly IdentitySubjectCredentials _credentials;
    private readonly IdentitySubjectActivationStatus _activation;
    private readonly IdentitySubjectPermissions _permissions;

    public IdentitySubject(
        IdentitySubjectMetadata metadata, 
        IdentitySubjectCredentials credentials, 
        IdentitySubjectActivationStatus status,
        IdentitySubjectPermissions permissions)
    {
        _metaData = metadata;
        _credentials = credentials;
        _activation = status;
        _permissions = permissions;
    }
    
    public void Register()
    {
        throw new NotImplementedException();
    }
}