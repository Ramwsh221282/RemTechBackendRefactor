﻿using System.Data.Common;
using RemTech.Core.Shared.Exceptions;
using RemTech.Vehicles.Module.Types.Characteristics.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Characteristics.Adapters.Storage.Postgres;

public sealed class PgSingleRiddenCharacteristicFromStore
{
    private readonly DbDataReader _reader;

    public PgSingleRiddenCharacteristicFromStore(DbDataReader reader)
    {
        _reader = reader;
    }

    public async Task<Characteristic> Read()
    {
        if (!await _reader.ReadAsync())
            throw new OperationException("Характеристика не найдена.");
        Guid id = _reader.GetGuid(_reader.GetOrdinal("id"));
        string text = _reader.GetString(_reader.GetOrdinal("text"));
        string measure = _reader.GetString(_reader.GetOrdinal("measuring"));
        return new Characteristic(
            new CharacteristicIdentity(new CharacteristicId(id), new CharacteristicText(text)),
            new CharacteristicMeasure(measure)
        );
    }
}
