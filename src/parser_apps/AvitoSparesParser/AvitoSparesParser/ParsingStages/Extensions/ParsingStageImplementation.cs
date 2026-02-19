using RemTech.SharedKernel.Infrastructure.Database;

namespace AvitoSparesParser.ParsingStages.Extensions;

public static class ParsingStageImplementation
{
    extension(ParsingStage stage)
    {
        public ParsingStage ToCatalogueStage()
        {
            return stage with { Name = ParsingStageConstants.CATALOGUE };
        }

        public ParsingStage ToConcreteItemsStage()
        {
            return stage with { Name = ParsingStageConstants.CONCRETE_ITEMS };
        }

        public ParsingStage ToFinalizationStage()
        {
            return stage with { Name = ParsingStageConstants.FINALIZATION_STAGE };
        }

        public ParsingStage ToEmptyStage()
        {
            return stage with { Name = ParsingStageConstants.EMPTY };
        }

        public async Task PermanentFinalize(NpgSqlSession session, CancellationToken ct = default)
        {
            ParsingStage finalized = stage.ToFinalizationStage();
            await finalized.Update(session, ct);            
        }
    }
}
