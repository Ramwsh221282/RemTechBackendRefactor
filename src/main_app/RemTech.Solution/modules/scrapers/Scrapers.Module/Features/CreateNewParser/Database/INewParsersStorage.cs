using Scrapers.Module.Features.CreateNewParser.Models;

namespace Scrapers.Module.Features.CreateNewParser.Database;

internal interface INewParsersStorage
{
    Task Save(NewParser parser, CancellationToken ct = default);
}
