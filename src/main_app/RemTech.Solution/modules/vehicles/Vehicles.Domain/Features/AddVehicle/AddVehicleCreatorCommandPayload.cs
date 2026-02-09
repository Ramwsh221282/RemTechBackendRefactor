namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Создатель транспортного средства для команды добавления транспортного средства.
/// </summary>
/// <param name="CreatorId">Идентификатор создателя транспортного средства.</param>
/// <param name="CreatorDomain">Домен создателя транспортного средства.</param>
/// <param name="CreatorType">Тип создателя транспортного средства.</param>
public sealed record AddVehicleCreatorCommandPayload(Guid CreatorId, string CreatorDomain, string CreatorType);
