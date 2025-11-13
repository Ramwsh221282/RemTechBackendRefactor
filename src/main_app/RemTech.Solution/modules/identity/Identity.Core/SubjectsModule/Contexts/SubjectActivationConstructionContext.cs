using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Contexts;

public sealed record SubjectActivationConstructionContext(Optional<DateTime> ActivationDate);