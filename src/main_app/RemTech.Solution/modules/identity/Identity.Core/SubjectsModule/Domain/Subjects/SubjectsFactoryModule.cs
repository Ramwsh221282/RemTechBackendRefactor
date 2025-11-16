using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.ActivationStatus;
using Identity.Core.SubjectsModule.Domain.Credentials;
using Identity.Core.SubjectsModule.Domain.Metadata;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Tickets;

namespace Identity.Core.SubjectsModule.Domain.Subjects;

public static class SubjectsFactoryModule
{
    extension(Subject)
    {
        public static Result<Subject> Create(
            Result<SubjectMetadata> metadata, 
            Result<SubjectCredentials> credentials, 
            Result<SubjectActivationStatus> activation, 
            Result<SubjectPermissions> permissions,
            Result<SubjectTicketsDictionary> tickets)
        {
            return metadata.Continue(credentials)
                           .Continue(activation)
                           .Continue(permissions)
                           .Continue(tickets)
                           .Map(() => new Subject(metadata, credentials, activation, permissions, tickets));
        }
        
        public static Result<Subject> Create(Result<SubjectMetadata> metadata, Result<SubjectCredentials> credentials)
        {
            SubjectPermissions permissions = SubjectPermissions.Empty();
            SubjectActivationStatus activation = SubjectActivationStatus.Create();
            return metadata.Continue(credentials).Map(() => new Subject(metadata, credentials, activation, permissions, new SubjectTicketsDictionary()));
        }

        public static async Task<Optional<Subject>> From(SubjectsStorage storage, SubjectQueryArgs args, CancellationToken ct)
        {
            return await storage.Find(args, ct);
        }
        
        public static Result<Subject> Create(
            Result<SubjectMetadata> metadata, 
            Result<SubjectCredentials> credentials,
            Result<SubjectActivationStatus> activation)
        {
            SubjectPermissions permissions = SubjectPermissions.Empty();
            return metadata.Continue(credentials).Continue(activation)
                           .Map(() => new Subject(metadata, credentials, activation, permissions, new SubjectTicketsDictionary()));
        }

        public static Result<Subject> Create(string email, string login, string password)
        {
            Result<SubjectMetadata> metadata = SubjectMetadata.Create(login);
            Result<SubjectCredentials> credentials = SubjectCredentials.Create(email, password);
            return Create(metadata, credentials);
        }

        public static Result<Subject> Create(SubjectSnapshot snapshot)
        {
            Result<SubjectMetadata> metadata = SubjectMetadata.Create(snapshot.Login, snapshot.Id);
            Result<SubjectCredentials> credentials = SubjectCredentials.Create(snapshot.Email, snapshot.Password);
            
            Result<SubjectActivationStatus> activation = snapshot.ActivationDate switch
            {
                null => SubjectActivationStatus.Create(),
                _ => SubjectActivationStatus.Create(snapshot.ActivationDate.Value),
            };
            
            Result<SubjectPermission>[] permissions = snapshot.Permissions.Select(p => SubjectPermissions.Create(p.Name, p.Id)).ToArray();
            Result<SubjectPermission>? failed = permissions.FirstOrDefault(p => p.IsFailure);
            if (failed != null) return failed.Error;

            Result<SubjectTicketsDictionary> dictionary = SubjectTicket.Create(snapshot.Tickets);
            Result<SubjectPermissions> collection = SubjectPermissions.Create(permissions.Select(p => p.Value));
            return Create(metadata, credentials, activation, collection, dictionary);
        }
    }
}