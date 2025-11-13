using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Notifications;

public sealed record EmailChanged(SubjectSnapshot Snapshot) : Notification;