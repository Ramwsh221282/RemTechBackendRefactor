using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Models;

public sealed record IdentitySubject
{
    private readonly IdentitySubjectMetadata _metaData;
    private readonly IdentitySubjectCredentials _credentials;
    private readonly IdentitySubjectActivationStatus _activation;
    private readonly IdentitySubjectPermissions _permissions;
    private readonly IdentitySubjectNotificationTarget _target;

    public IdentitySubject(
        IdentitySubjectMetadata metadata,
        IdentitySubjectCredentials credentials,
        IdentitySubjectActivationStatus status,
        IdentitySubjectPermissions permissions,
        IdentitySubjectNotificationTarget? listener = null)
    {
        _metaData = metadata;
        _credentials = credentials;
        _activation = status;
        _permissions = permissions;
        _target = listener ?? new IdentitySubjectDummyTarget();
    }

    public Result<IdentitySubject> Registered()
    {
        Result<IdentitySubjectActivationStatus> activated = _activation.Activate();
        if (activated.IsFailure) return activated.Error;
        _target.Record(new IdentitySubjectRegisteredNotification(_metaData, _credentials, _permissions));
        return this;
    }
}