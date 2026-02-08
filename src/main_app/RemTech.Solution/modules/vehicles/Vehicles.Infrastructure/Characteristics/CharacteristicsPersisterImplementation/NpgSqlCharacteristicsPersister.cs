using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Characteristics;
using Vehicles.Domain.Characteristics.Contracts;

namespace Vehicles.Infrastructure.Characteristics.CharacteristicsPersisterImplementation;

/// <summary>
/// Реализация персистера характеристик на основе NpgSql и векторных представлений.
/// </summary>
/// <param name="session">Сессия для работы с базой данных NpgSql.</param>
/// <param name="embeddings">Провайдер векторных представлений.</param>
public sealed class NpgSqlCharacteristicsPersister(NpgSqlSession session, EmbeddingsProvider embeddings)
	: ICharacteristicsPersister
{
	private const double MAX_DISTANCE = 0.6;

	/// <summary>
	/// Сохраняет характеристику в базе данных.
	/// </summary>
	/// <param name="characteristic">Характеристика для сохранения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат операции сохранения характеристики.</returns>
	public async Task<Result<Characteristic>> Save(Characteristic characteristic, CancellationToken ct = default)
    {
        const string sql = """
                            WITH vector_filtered AS (
                                SELECT 
                                    id as id, 
                                    name as name,
                                    1 - (embedding <=> @input_embedding) as score     
                            FROM vehicles_module.characteristics
                            WHERE
                                1 - (embedding <=> @input_embedding) >= 0.4
                            ORDER BY score DESC
                            LIMIT 15
                            ) 
                            SELECT
                            trgrm_filtered.id as Id,
                            trgrm_filtered.name as Name
                            FROM vector_filtered
                            JOIN LATERAL (
                                SELECT
                                    vector_filtered.id as id,
                                    vector_filtered.name as name,
                                    word_similarity(vector_filtered.name, @name) as sml
                                FROM 
                                    vector_filtered
                                WHERE
                                    vector_filtered.name = @name
                                    OR word_similarity(vector_filtered.name, @name) >= 0.4
                                ORDER BY
                                    CASE
                                        WHEN vector_filtered.name = @name THEN 0
                                        when word_similarity(vector_filtered.name, @name) >= 0.4 THEN 1
                                        ELSE 2
                                    END,
                                    sml DESC
                                    LIMIT 1
                            ) AS trgrm_filtered ON TRUE
                            LIMIT 1
                            """;

		Vector vector = new(embeddings.Generate(characteristic.Name.Value));
		DynamicParameters parameters = BuildParameters(characteristic, vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		NpgSqlSearchResult? result = await session.QueryMaybeRow<NpgSqlSearchResult?>(command);
		if (result is null)
		{
            return Error.NotFound("Unable to resolve characteristic");
		}

		return MapToCharacteristicFromExactSearch(result);
	}

	private static Characteristic MapToCharacteristicFromExactSearch(NpgSqlSearchResult result)
	{
		CharacteristicId id = CharacteristicId.Create(result.Id);
		CharacteristicName name = CharacteristicName.Create(result.Name);
		return new Characteristic(id, name);
	}

	private static DynamicParameters BuildParameters(Characteristic characteristic, Vector vector)
	{
		DynamicParameters parameters = new();
		parameters.Add("@name", characteristic.Name.Value, DbType.String);
		parameters.Add("@input_embedding", vector);
		return parameters;
	}

	private Task SaveAsNewCharacteristic(Characteristic characteristic, Vector vector, CancellationToken ct)
	{
		const string sql =
			"INSERT INTO vehicles_module.characteristics (id, name, embedding) VALUES (@id, @name, @input_embedding) ON CONFLICT (name) DO NOTHING";
		DynamicParameters parameters = new();
		parameters.Add("@id", characteristic.Id.Value, DbType.Guid);
		parameters.Add("@name", characteristic.Name.Value, DbType.String);
		parameters.Add("@input_embedding", vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		return session.Execute(command);
	}

	private sealed class NpgSqlSearchResult
	{
		public required Guid Id { get; init; }
		public required string Name { get; init; }
	}
}
