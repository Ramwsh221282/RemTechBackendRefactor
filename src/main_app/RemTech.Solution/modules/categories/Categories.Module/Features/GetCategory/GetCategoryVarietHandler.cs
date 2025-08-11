using Categories.Module.Types;
using Shared.Infrastructure.Module.Cqrs;

namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryVarietHandler(Serilog.ILogger logger)
    : ICommandHandler<GetCategoryCommand, ICategory>
{
    private readonly Queue<ICommandHandler<GetCategoryCommand, ICategory>> _handlers = [];

    public GetCategoryVarietHandler With(ICommandHandler<GetCategoryCommand, ICategory> handler)
    {
        _handlers.Enqueue(handler);
        return this;
    }

    public async Task<ICategory> Handle(GetCategoryCommand command, CancellationToken ct = default)
    {
        while (_handlers.Count > 0)
        {
            ICommandHandler<GetCategoryCommand, ICategory> handler = _handlers.Dequeue();
            try
            {
                ICategory category = await handler.Handle(command, ct);
                logger.Information("Resolved category: {Name}", category.Name);
                return category;
            }
            catch (GetCategoryByNameNotFoundException ex)
            {
                logger.Error("{Command} {Ex}.", nameof(GetCategoryCommand), ex.Message);
            }
            catch (GetCategoryByNameNameEmptyException ex)
            {
                logger.Error("{Command} {Ex}.", nameof(GetCategoryCommand), ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error("{Command} {Ex}.", nameof(GetCategoryCommand), ex.Message);
                throw;
            }
        }

        throw new UnableToResolveCategoryException();
    }
}
