using Brands.Module.Types;
using RemTech.Core.Shared.Cqrs;

namespace Brands.Module.Features.GetBrand;

internal sealed class GetBrandVarietHandler(Serilog.ILogger logger)
    : ICommandHandler<GetBrandCommand, IBrand>
{
    private readonly List<ICommandHandler<GetBrandCommand, IBrand>> _handlers = [];

    public GetBrandVarietHandler With(ICommandHandler<GetBrandCommand, IBrand> handler)
    {
        _handlers.Add(handler);
        return this;
    }

    public async Task<IBrand> Handle(GetBrandCommand command, CancellationToken ct = default)
    {
        foreach (ICommandHandler<GetBrandCommand, IBrand> handler in _handlers)
        {
            try
            {
                IBrand brand = await handler.Handle(command, ct);
                logger.Information("Found brand: {Name}.", brand.Name);
                return brand;
            }
            catch
            {
                // ignored
            }
        }

        throw new UnableToResolveBrandException();
    }
}
