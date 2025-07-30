using System.Text;
using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specification;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public interface IQueryVehiclesSpecification
{
    void ApplyTo(VehiclesSqlQuery query);
}

public sealed record VehiclesQueryRequest(
    VehicleKindIdQueryFilterArgument KindId,
    VehicleBrandIdQueryFilterArgument BrandId,
    VehicleModelIdQueryFilterArgument ModelId,
    VehiclePaginationQueryFilterArgument Pagination,
    VehicleSortOrderQueryFilterArgument? SortOrder = null,
    VehicleRegionIdQueryFilterArgument? RegionId = null,
    VehiclePriceQueryFilterArgument? Price = null,
    VehicleCharacteristicsQueryArguments? Characteristics = null
) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        CompositeVehicleSpeicification composite = new();
        composite = KindId.ApplyTo(composite);
        composite = BrandId.ApplyTo(composite);
        composite = ModelId.ApplyTo(composite);
        composite = Pagination.ApplyTo(composite);
        composite = RegionId.ApplyIfProvided(composite);
        composite = Price.ApplyIfProvided(composite);
        composite = SortOrder.ApplyIfProvided(composite);
        composite = RegionId.ApplyIfProvided(composite);
        composite = Price.ApplyIfProvided(composite);
        composite = Characteristics.ApplyIfProvided(composite);
        return composite;
    }
}

public abstract record VehicleQueryFilterArgument
{
    public abstract CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    );
}

public sealed record VehiclePaginationQueryFilterArgument(int Page) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePaginationQuerySpecification(Page));
    }
}

public sealed record VehicleSortOrderQueryFilterArgument(string Order) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePriceSortQuerySpecification(Order));
    }
}

public sealed record VehicleKindIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        VehicleKindIdentity identity = new(
            new VehicleKindId(Id),
            new VehicleKindText(string.Empty)
        );
        return speicification.With(new VehicleKindQuerySpecification(identity));
    }
}

public sealed record VehicleBrandIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleBrandQuerySpecification(
                new VehicleBrandIdentity(new VehicleBrandId(Id), new VehicleBrandText(string.Empty))
            )
        );
    }
}

public sealed record VehicleModelIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleModelQuerySpecification(new VehicleModelIdentity(Id))
        );
    }
}

public sealed record VehicleRegionIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleRegionQuerySpecification(
                new GeoLocationIdentity(
                    new GeoLocationId(new NotEmptyGuid(Id)),
                    new GeolocationText(string.Empty),
                    new GeolocationText(string.Empty)
                )
            )
        );
    }
}

public sealed record VehiclePriceQueryFilterArgument(double? PriceFrom, double? PriceTo, bool IsNds)
    : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(new VehiclePriceQuerySpecification(PriceTo, PriceFrom, IsNds));
    }
}

public sealed record VehicleCharacteristicQueryArgument(Guid Id, string Name, string Value)
{
    public VehicleCharacteristic AsCharacteristic()
    {
        CharacteristicIdentity identity = new(
            new CharacteristicId(Id),
            new CharacteristicText(Name)
        );
        Characteristic characteristic = new(identity);
        return new VehicleCharacteristic(characteristic, new VehicleCharacteristicValue(Value));
    }
}

public sealed class VehiclesSqlQuery
{
    private const int MaxPageSize = 20;
    private readonly List<string> _filters = [];
    private readonly List<NpgsqlParameter> _parameters = [];
    private string _ordering = string.Empty;
    private string _pagination = string.Empty;

    private readonly string _sql = string.Intern(
        """
        WITH aggregated_ctxes AS (
            SELECT vehicle_id, jsonb_agg(jsonb_build_object(
                             'ctx_id', ctx.ctx_id,
                             'ctx_name', ctx_name,
                             'ctx_value', ctx.ctx_value,
                             'ctx_measure', ctx.ctx_measure
                             )) as characteristics
            FROM parsed_advertisements_module.parsed_vehicle_characteristics ctx
            GROUP BY vehicle_id
            )
        SELECT
            v.id as vehicle_id,
            v.price as vehicle_price,
            v.photos as vehicle_photos,
            v.is_nds as vehicle_nds,
            k.id as kind_id,
            k.text as kind_name,
            b.id as brand_id,
            b.text as brand_name,
            m.id as model_id,
            m.text as model_name,
            g.id as geo_id,
            g.text as geo_text,
            c.characteristics as vehicle_characteristics
        FROM parsed_advertisements_module.parsed_vehicles v
                 INNER JOIN aggregated_ctxes c ON v.id = c.vehicle_id
                 INNER JOIN parsed_advertisements_module.vehicle_kinds k ON k.id = v.kind_id
                 INNER JOIN parsed_advertisements_module.vehicle_brands b ON b.id = v.brand_id
                 INNER JOIN parsed_advertisements_module.vehicle_models m ON m.id = v.model_id
                 INNER JOIN parsed_advertisements_module.geos g on v.geo_id = g.id
        """
    );

    public void AcceptFilter(string filter, NpgsqlParameter parameter)
    {
        _filters.Add(filter);
        _parameters.Add(parameter);
    }

    public void AcceptFilter(string filter, IEnumerable<NpgsqlParameter> parameters)
    {
        _filters.Add(filter);
        _parameters.AddRange(parameters);
    }

    public void AcceptAscending(string orderingField)
    {
        _ordering = $" ORDER BY {orderingField} ASC ";
    }

    public void AcceptDescending(string orderingField)
    {
        _ordering = $" ORDER BY {orderingField} DESC ";
    }

    public async Task<IPgCommandSource> PrepareCommand(
        NpgsqlCommand command,
        CancellationToken ct = default
    )
    {
        command.CommandText = GenerateSql();
        foreach (NpgsqlParameter parameter in _parameters)
            command.Parameters.Add(parameter);
        await command.PrepareAsync(ct);
        return new DefaultPgCommandSource(command);
    }

    public void AcceptPagination(int page)
    {
        if (page <= 0)
        {
            _pagination = " LIMIT 0 ";
            return;
        }

        int offset = (page - 1) * MaxPageSize;
        _pagination = " LIMIT @limit OFFSET @offset ";
        _parameters.Add(new NpgsqlParameter<int>("@limit", MaxPageSize));
        _parameters.Add(new NpgsqlParameter<int>("@offset", offset));
    }

    private string GenerateSql()
    {
        StringBuilder sb = new StringBuilder(_sql);
        sb = sb.AppendLine(GenerateFilters());
        if (!string.IsNullOrWhiteSpace(_ordering))
            sb = sb.AppendLine(_ordering);
        if (!string.IsNullOrWhiteSpace(_pagination))
            sb = sb.AppendLine(_pagination);
        return sb.ToString();
    }

    private string GenerateFilters()
    {
        if (_filters.Count == 0)
            return string.Empty;
        StringBuilder stringBuilder = new();
        string joined = string.Join(" AND ", _filters);
        return stringBuilder.AppendLine(" WHERE 1=1 AND ").AppendLine(joined).ToString();
    }
}
