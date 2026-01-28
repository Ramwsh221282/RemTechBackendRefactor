namespace Spares.Domain.Features;

/// <summary>
/// Создатель команды добавления запчастей.
/// </summary>
/// <param name="CreatorId">Идентификатор создателя команды.</param>
/// <param name="CreatorDomain">Домен создателя команды.</param>
/// <param name="CreatorType">Тип создателя команды.</param>
public sealed record AddSparesCreatorPayload(Guid CreatorId, string CreatorDomain, string CreatorType);
