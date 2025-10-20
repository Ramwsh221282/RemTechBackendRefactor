using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeWaitDays;

internal sealed record ChangeCleanerWaitDays(int Days) : ICommand;
