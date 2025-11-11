using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Contexts;

public sealed record IdentitySubjectMetadataConstructionContext(string Login, Optional<Guid> Id);