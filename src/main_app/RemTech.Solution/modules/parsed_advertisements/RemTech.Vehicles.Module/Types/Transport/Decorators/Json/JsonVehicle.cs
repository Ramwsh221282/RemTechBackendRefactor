using RemTech.Core.Shared.Serialization.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Json;

internal sealed class JsonVehicle(Vehicle inner) : Vehicle(inner)
{
    public string Read()
    {
        return new PlainSerJson()
            .With(new StringSerJson("kind_name", Category.Name))
            .With(new StringSerJson("brand_name", Brand.Name))
            .With(new StringSerJson("model_name", Model.Name))
            .With(new StringSerJson("location_name", Location.Text))
            .With(new StringSerJson("location_kind", Location.KindText))
            .With(new StringSerJson("location_city", Location.CityText))
            .With(
                new ObjectsArraySerJson<VehicleCharacteristic>(
                    "characteristics",
                    Characteristics.Read()
                ).ForEach(c =>
                    new PlainSerJson()
                        .With(
                            new StringSerJson(
                                "ctx_name",
                                c.WhatCharacteristic().Identity.ReadText()
                            )
                        )
                        .With(new StringSerJson("ctx_value", c.WhatValue()))
                )
            )
            .With(
                new ObjectsArraySerJson<string>(
                    "photos",
                    Photos.Read().Select(p => (string)p)
                ).ForEach(s => new PlainSerJson().With(new StringSerJson("source", s)))
            )
            .Read();
    }
}
