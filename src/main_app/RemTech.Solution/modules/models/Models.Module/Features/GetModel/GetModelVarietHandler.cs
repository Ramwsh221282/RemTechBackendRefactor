using Models.Module.Types;
using Shared.Infrastructure.Module.Cqrs;

namespace Models.Module.Features.GetModel;

internal sealed class GetModelVarietHandler(Serilog.ILogger logger)
    : ICommandHandler<GetModelCommand, IModel>
{
    private readonly Queue<ICommandHandler<GetModelCommand, IModel>> _handlers = [];

    public GetModelVarietHandler With(ICommandHandler<GetModelCommand, IModel> handler)
    {
        _handlers.Enqueue(handler);
        return this;
    }

    public async Task<IModel> Handle(GetModelCommand command, CancellationToken ct = default)
    {
        while (_handlers.Count > 0)
        {
            ICommandHandler<GetModelCommand, IModel> handler = _handlers.Dequeue();
            try
            {
                IModel model = await handler.Handle(command, ct);
                logger.Information("Resolved model: {Name}", model.Name);
                return model;
            }
            catch (GetModelByNameNotFoundException ex)
            {
                logger.Error("{Command} {Ex}", nameof(GetModelCommand), ex.Message);
            }
            catch (GetModelByNameNameEmptyException ex)
            {
                logger.Error("{Command} {Ex}", nameof(GetModelCommand), ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error("{Command} {Ex}", nameof(GetModelCommand), ex.Message);
                throw;
            }
        }

        throw new UnableToResolveModelException();
    }
}
