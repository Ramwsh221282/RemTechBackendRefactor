namespace Vehicles.Infrastructure.Models.Queries.GetModels;

/// <summary>
/// Ответ с информацией о модели транспортного средства.
/// </summary>
/// <param name="Id">Идентификатор модели транспортного средства.</param>
/// <param name="Name">Название модели транспортного средства.</param>
public sealed record ModelResponse(Guid Id, string Name);
