using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Contexts;

public sealed record SubjectMetadataConstructionContext(string Login, Optional<Guid> Id);