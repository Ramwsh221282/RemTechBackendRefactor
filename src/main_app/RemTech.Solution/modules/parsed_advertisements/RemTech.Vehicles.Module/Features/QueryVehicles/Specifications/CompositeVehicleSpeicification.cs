﻿namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

public sealed class CompositeVehicleSpeicification : IQueryVehiclesSpecification
{
    private readonly Queue<IQueryVehiclesSpecification> _specs = [];

    public CompositeVehicleSpeicification With(IQueryVehiclesSpecification spec)
    {
        _specs.Enqueue(spec);
        return this;
    }

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        while (_specs.Count > 0)
        {
            IQueryVehiclesSpecification spec = _specs.Dequeue();
            spec.ApplyTo(query);
        }
    }
}
