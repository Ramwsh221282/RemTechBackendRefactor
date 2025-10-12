using FluentValidation;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Cqrs;
using RemTech.UseCases.Shared.Logging;
using RemTech.UseCases.Shared.Validations;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.UseCases.BrandContext.Commands.CreateBrand;

public sealed record CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, Brand>
{
    private readonly IBrandsDataSource _brands;
    private readonly Serilog.ILogger _logger;
    private readonly IValidator<CreateBrandCommand> _validator;

    public CreateBrandCommandHandler(
        IBrandsDataSource brands,
        Serilog.ILogger logger,
        IValidator<CreateBrandCommand> validator
    )
    {
        _brands = brands;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Brand>> Handle(
        CreateBrandCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await _validator.ValidateAsync(command, ct);
        if (validation.NotValid())
            return _logger.LoggedError(validation.Failure<Brand>(), nameof(CreateBrandCommand));

        BrandName name = BrandName.Create(command.Name);
        Result<Brand> brand = await Brand.Create(name, _brands, ct);

        return brand.IsFailure
            ? _logger.LoggedError(brand.Error, nameof(CreateBrandCommand))
            : brand;
    }
}
