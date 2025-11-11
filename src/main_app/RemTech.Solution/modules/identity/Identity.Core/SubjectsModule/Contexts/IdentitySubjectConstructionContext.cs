namespace Identity.Core.SubjectsModule.Contexts;

public sealed record IdentitySubjectConstructionContext(
    IdentitySubjectMetadataConstructionContext MetaCtx,
    IdentitySubjectCredentialsConstructionContext CredCtx,
    IdentitySubjectActivationConstructionContext ActivationCtx,
    IdentitySubjectPermissionsConstructionContext PermissionsCtx);