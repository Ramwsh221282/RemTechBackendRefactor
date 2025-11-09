using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record CreateMailerFunctionArgument(
    CreateMailerMetadataArguments MetadataArgs, 
    CreateMailerStatisticsFunctionArgument StatisticsArg) : IFunctionArgument;

public sealed class CreateMailerFunction : IFunction<CreateMailerFunctionArgument, Result<Mailer>>
{
    private readonly IFunction<CreateMailerMetadataArguments, Result<MailerMetadata>> _createMeta;
    private readonly IFunction<CreateMailerStatisticsFunctionArgument, Result<MailerStatistics>> _createStats;

    public CreateMailerFunction(
        IFunction<CreateMailerMetadataArguments, Result<MailerMetadata>> createMeta,
        IFunction<CreateMailerStatisticsFunctionArgument, Result<MailerStatistics>> createStats
        ) => (_createMeta, _createStats) = (createMeta, createStats);

    public Result<Mailer> Invoke(CreateMailerFunctionArgument argument)
    {
        var metaResult = _createMeta.Invoke(argument.MetadataArgs);
        var statsResult = _createStats.Invoke(argument.StatisticsArg);
        if (metaResult.IsFailure) return metaResult.Error;
        if (statsResult.IsFailure) return statsResult.Error;
        return new Mailer(metaResult.Value, statsResult.Value);
    }
}