using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeWaitDays;

internal sealed record ChangeCleanerWaitDays(int Days) : ICommand;
