using RemTech.Core.Shared.Result;

namespace Cleaner.WebApi.CleaningLogic;

internal interface ICheckChallenge
{
    Task<Status<string>> ItemIsOutdated();
}
