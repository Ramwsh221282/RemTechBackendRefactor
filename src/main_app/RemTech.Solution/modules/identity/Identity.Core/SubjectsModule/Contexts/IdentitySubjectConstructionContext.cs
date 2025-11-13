namespace Identity.Core.SubjectsModule.Contexts;

public sealed record IdentitySubjectConstructionContext(
    SubjectMetadataConstructionContext MetaCtx,
    SubjectCredentialsConstructionContext CredCtx,
    SubjectActivationConstructionContext ActivationCtx,
    SubjectPermissionsConstructionContext PermissionsCtx);