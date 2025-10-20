using Categories.Module.Types;
using RemTech.Core.Shared.Cqrs;

namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryVarietHandler(Serilog.ILogger logger)
    : ICommandHandler<GetCategoryCommand, ICategory>
{
    private readonly List<ICommandHandler<GetCategoryCommand, ICategory>> _handlers = [];

    public GetCategoryVarietHandler With(ICommandHandler<GetCategoryCommand, ICategory> handler)
    {
        _handlers.Add(handler);
        return this;
    }

    public async Task<ICategory> Handle(GetCategoryCommand command, CancellationToken ct = default)
    {
        foreach (ICommandHandler<GetCategoryCommand, ICategory> handler in _handlers)
        {
            try
            {
                ICategory category = await handler.Handle(command, ct);
                logger.Information("Resolved category: {Name}", category.Name);
                return category;
            }
            catch
            {
                // ignored
            }
        }

        throw new UnableToResolveCategoryException();
    }
}
