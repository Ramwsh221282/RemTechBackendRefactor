﻿using RemTech.Json.Library.Serialization.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Json;

public sealed class JsonVehicle(Vehicle origin) : Vehicle(origin)
{
    public string Read()
    {
        return new PlainSerJson()
            .With(new StringSerJson("kind_name", Kind.Name()))
            .With(new StringSerJson("brand_name", Brand.Name()))
            .With(new StringSerJson("model_name", Model.NameString()))
            .With(new StringSerJson("location_name", Location.Name()))
            .With(new StringSerJson("location_kind", Location.Kind()))
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
                        .With(new StringSerJson("ctx_measure", c.WhatCharacteristic().Measure()))
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
