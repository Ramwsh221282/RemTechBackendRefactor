using Identity.Core.SubjectsModule.Contexts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Modules;

public static class IdentitySubjectsFactoryModule
{
    extension(Subject)
    {
        public static Result<Subject> Create(IdentitySubjectConstructionContext ctx)
        {
            Result<SubjectMetadata> metadata = ctx.MetaCtx.Construct();
            Result<SubjectCredentials> creds = ctx.CredCtx.Construct();
            Result<SubjectActivationStatus> activation = ctx.ActivationCtx.Create();
            Result<SubjectPermissions> permissions = ctx.PermissionsCtx.Create();
            return metadata.Continue(creds).Continue(activation).Continue(permissions)
                           .Map(() => new Subject(metadata, creds, activation, permissions));
        }
        
        public static Result<Subject> Create(IdentitySubjectConstructionContext ctx, SubjectEventsRegistry registry)
        {
            Result<Subject> subject = Create(ctx);
            if (subject.IsFailure) return subject.Error;
            return subject.Value.With(registry);
        }

        public static Subject Create(IdentitySubjectSnapshot snapshot)
        {
            SubjectMetadata metadata = snapshot.MetadataFromSnapshot();
            SubjectCredentials credentials = snapshot.CredentialsFromSnapshot();
            SubjectActivationStatus activation = snapshot.ActivationFromSnapshot();
            SubjectPermissions permissions = snapshot.PermissionsFromSnapshot();
            return new Subject(metadata, credentials, activation, permissions);
        }
    }

    extension(IdentitySubjectSnapshot snapshot)
    {
        private SubjectMetadata MetadataFromSnapshot()
        {
            return new SubjectMetadata(snapshot.Id, snapshot.Login);
        }

        private SubjectCredentials CredentialsFromSnapshot()
        {
            return new SubjectCredentials(snapshot.Email, snapshot.Password);
        }

        private SubjectPermissions PermissionsFromSnapshot()
        {
            IdentitySubjectPermission[] permissions = snapshot.Permissions.Map(snapshot.PermissionFromSnapshot);
            return new SubjectPermissions(permissions);
        }

        private IdentitySubjectPermission PermissionFromSnapshot(IdentitySubjectPermissionSnapshot permSnapshot)
        {
            return new IdentitySubjectPermission(permSnapshot.Id, permSnapshot.Name);
        }

        private SubjectActivationStatus ActivationFromSnapshot()
        {
            return snapshot.ActivationFromSnapshot(snapshot.ActivationDate);
        }
        
        private SubjectActivationStatus ActivationFromSnapshot(DateTime? date)
        {
            return date.HasValue
                ? new SubjectActivationStatus(date.Value)
                : SubjectActivationStatus.Inactive();
        }
    }
    
    extension(Subject subject)
    {
        public Subject With(SubjectEventsRegistry registry)
        {
            return new Subject(subject, registry);
        }
    }

    extension(IdentitySubjectPermissionsConstructionContext ctx)
    {
        public SubjectPermissions Empty()
        {
            return new SubjectPermissions();
        }

        private Result<SubjectPermissions> Create()
        {
            const int maxNameLength = SubjectPermissions.MAX_NAME_LENGTH;
            if (!ctx.Permissions.All(p => p.Name.Length < maxNameLength))
                Validation($"Название разрешения превышает длину: {maxNameLength}");
            
            Result<UniqueSequence<IdentitySubjectPermission>> unique =
                UniqueSequence<IdentitySubjectPermission>.Create(ctx.Permissions);
            return unique.IsFailure
                ? Validation("Список разрешений не уникален")
                : new SubjectPermissions(ctx.Permissions);
        }
    }
    
    extension(IdentitySubjectActivationConstructionContext ctx)
    {
        private Result<SubjectActivationStatus> Create()
        {
            return ctx.ActivationDate.HasValue
                ? SubjectActivationStatus.Create(ctx.ActivationDate.Value)
                : SubjectActivationStatus.Inactive();
        }
    }
    
    extension(IdentitySubjectMetadataConstructionContext ctx)
    {
        private Result<SubjectMetadata> Construct()
        {
            Guid id = ctx.Id.HasValue ? ctx.Id.Value : Guid.NewGuid();
            return new SubjectMetadata(id, ctx.Login).Validated();
        }
    }

    extension(IdentitySubjectCredentialsConstructionContext ctx)
    {
        private Result<SubjectCredentials> Construct()
        {
            return new SubjectCredentials(ctx.Email, ctx.Password).Validated();
        }
    }
}