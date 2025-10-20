using RemTech.Core.Shared.Cqrs;

namespace Models.Module.Features.GetModel;

internal sealed record GetModelCommand(string Name) : ICommand;
