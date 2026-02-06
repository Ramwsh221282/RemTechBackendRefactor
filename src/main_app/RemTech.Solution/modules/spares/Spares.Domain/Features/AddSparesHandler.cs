using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using Spares.Domain.Contracts;
using Spares.Domain.Models;
using Spares.Domain.Oems;
using Spares.Domain.Types;

namespace Spares.Domain.Features;

/// <summary>
/// Обработчик команды добавления запчастей.
/// </summary>
/// <param name="sparesRepository">Репозиторий для работы с запчастями.</param>
[TransactionalHandler]
public sealed class AddSparesHandler(
	ISparesRepository sparesRepository,
	ISpareOemsRepository oemsRepository,
	ISpareTypesRepository typesRepository
) : ICommandHandler<AddSparesCommand, (Guid, int)>
{
	/// <summary>
	/// Выполняет команду добавления запчастей.
	/// </summary>
	/// <param name="command">Команда добавления запчастей.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды, содержащий идентификатор создателя и количество добавленных запчастей.</returns>
	public async Task<Result<(Guid, int)>> Execute(AddSparesCommand command, CancellationToken ct = default)
	{
		Guid creatorId = command.Creator.CreatorId;
		IEnumerable<Spare> spares = await CreateSpares(command.Spares, ct);
		int addedCount = await sparesRepository.AddMany(spares, ct);
		return (creatorId, addedCount);
	}

	private async Task<IEnumerable<Spare>> CreateSpares(
		IEnumerable<AddSpareCommandPayload> spareInfo,
		CancellationToken ct = default
	)
	{
		List<AddSpareCommandPayload> validInfos = FilterFromInvalid(spareInfo);
		if (validInfos.Count == 0)
		{
			return [];
		}
        
		Dictionary<string, SpareType> types = [];
		Dictionary<string, SpareOem> oems = [];
		FillSparesAndOemsWithValidData(types, oems, validInfos);
		if (types.Count == 0 || oems.Count == 0)
		{
			return [];
		}

		Dictionary<string, SpareOem> resolvedOems = await GetOrAddOems(oems, ct);
		Dictionary<string, SpareType> resolvedTypes = await GetOrAddOems(types, ct);
		return ConstructValidSparesForSaving(validInfos, resolvedOems, resolvedTypes);
	}

	private async Task<Dictionary<string, SpareOem>> GetOrAddOems(
		Dictionary<string, SpareOem> oems,
		CancellationToken ct
	)
	{
		SpareOem[] originArray = [.. oems.Values];
		return await oemsRepository.SaveOrFindManySimilar(originArray, ct);
	}

	private async Task<Dictionary<string, SpareType>> GetOrAddOems(
		Dictionary<string, SpareType> types,
		CancellationToken ct
	)
	{
		SpareType[] originArray = [.. types.Values];
		return await typesRepository.SaveOrFindManySimilar(originArray, ct);
	}

	private static List<AddSpareCommandPayload> FilterFromInvalid(IEnumerable<AddSpareCommandPayload> spares)
	{
		List<AddSpareCommandPayload> results = [];
		foreach (AddSpareCommandPayload info in spares)
		{
			Result<SpareOem> oem = SparesFactory.CreateSpareOem(info.Oem);
			if (oem.IsFailure)
			{
				continue;
			}

			Result<SpareType> type = SparesFactory.CreateSpareType(info.Type);
			if (type.IsFailure)
			{
				continue;
			}
            
            results.Add(info);
		}

		return results;
	}

	private void FillSparesAndOemsWithValidData(
		Dictionary<string, SpareType> types,
		Dictionary<string, SpareOem> oems,
		IEnumerable<AddSpareCommandPayload> validInfos
	)
	{
		foreach (AddSpareCommandPayload info in validInfos)
		{
			Result<SpareOem> oem = SparesFactory.CreateSpareOem(info.Oem);
			Result<SpareType> type = SparesFactory.CreateSpareType(info.Type);

			if (oem.IsSuccess && !oems.ContainsKey(info.Oem))
			{
				oems[info.Oem] = oem.Value;
			}

			if (type.IsSuccess && !types.ContainsKey(info.Type))
			{
				types[info.Type] = type.Value;
			}
		}
	}

	private static List<Spare> ConstructValidSparesForSaving(
		List<AddSpareCommandPayload> validInfos,
		Dictionary<string, SpareOem> resolvedOems,
		Dictionary<string, SpareType> resolvedTypes
	)
	{
		List<Spare> results = [];
		foreach (AddSpareCommandPayload info in validInfos)
		{
			SpareOem oem = resolvedOems[info.Oem];
			SpareType type = resolvedTypes[info.Type];

			Result<Spare> spareResult = SparesFactory.Create(
				containedItemId: info.ContainedItemId,
				source: info.Source,
				oem: oem,
				title: info.Title,
				price: info.Price,
				isNds: info.IsNds,
				type: type,
				address: info.Address,
				photoPaths: info.PhotoPaths
			);

			if (spareResult.IsSuccess)
			{
				results.Add(spareResult.Value);
			}
		}

		return results;
	}
}
