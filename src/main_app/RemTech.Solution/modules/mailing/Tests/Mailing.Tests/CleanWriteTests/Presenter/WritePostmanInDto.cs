using Mailing.Tests.CleanWriteTests.Models;
using Mailing.Tests.CleanWriteTests.Presenter.Implementation;

namespace Mailing.Tests.CleanWriteTests.Presenter;

public sealed class WritePostmanInDto(PostmanDto dto) : IWritePostmanPresenterCommand
{
    private readonly WritePostmanMetadataPresenter _metaPres = new(dto);
    private readonly WritePostmanStatisticsPresenter _statsPres = new(dto);

    public void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        metadata.Write(_metaPres);
        statistics.Write(_statsPres);
    }
}