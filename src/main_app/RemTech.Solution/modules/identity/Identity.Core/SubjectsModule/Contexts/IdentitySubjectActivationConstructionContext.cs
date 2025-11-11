using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Contexts;

public sealed record IdentitySubjectActivationConstructionContext(Optional<DateTime> ActivationDate);