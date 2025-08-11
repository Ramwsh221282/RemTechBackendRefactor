using Brands.Module.Types;
using Shared.Infrastructure.Module.Cqrs;

namespace Brands.Module.Features.GetBrand;

internal sealed class GetBrandVarietHandler(Serilog.ILogger logger)
    : ICommandHandler<GetBrandCommand, IBrand>
{
    private readonly Queue<ICommandHandler<GetBrandCommand, IBrand>> _handlers = [];

    public GetBrandVarietHandler With(ICommandHandler<GetBrandCommand, IBrand> handler)
    {
        _handlers.Enqueue(handler);
        return this;
    }

    public async Task<IBrand> Handle(GetBrandCommand command, CancellationToken ct = default)
    {
        while (_handlers.Count > 0)
        {
            ICommandHandler<GetBrandCommand, IBrand> handler = _handlers.Dequeue();
            try
            {
                IBrand brand = await handler.Handle(command, ct);
                logger.Information("Found brand: {Name}.", brand.Name);
            }
            catch (BrandRawByNameNotFoundException ex)
            {
                logger.Error("{Command}. {Ex}.", nameof(GetBrandCommand), ex.Message);
            }
            catch (BrandRawByNameEmptyNameException ex)
            {
                logger.Error("{Command}. {Ex}.", nameof(GetBrandCommand), ex.Message);
            }
            catch (Exception ex)
            {
                logger.Fatal("{Command}. {Ex}.", nameof(GetBrandCommand), ex.Message);
            }
        }

        throw new UnableToResolveBrandException();
    }
}
