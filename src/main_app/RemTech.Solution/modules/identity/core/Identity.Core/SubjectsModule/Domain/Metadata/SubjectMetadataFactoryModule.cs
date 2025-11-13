using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Domain.Metadata;

public static class SubjectMetadataFactoryModule
{
    extension(SubjectMetadata)
    {
        public static Result<SubjectMetadata> Create(string login, Guid id)
        {
            return new SubjectMetadata(id, login).Validated();
        }
        
        public static Result<SubjectMetadata> Create(string login)
        {
            return new SubjectMetadata(Guid.NewGuid(), login).Validated();
        }
    }
}