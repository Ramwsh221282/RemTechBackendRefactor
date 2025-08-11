using Shared.Infrastructure.Module.Cqrs;

namespace Models.Module.Features.GetModel;

internal sealed record GetModelCommand(string Name) : ICommand;
