using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;
using Spares.Domain.Contracts;
using Spares.Domain.Models;

namespace Spares.Domain.Features;

/// <summary>
/// Обработчик команды добавления запчастей.
/// </summary>
/// <param name="repository">Репозиторий для работы с запчастями.</param>
[TransactionalHandler]
public sealed class AddSparesHandler(ISparesRepository repository) : ICommandHandler<AddSparesCommand, (Guid, int)>
{
	/// <summary>
	/// Создает массив запчастей из предоставленной информации.
	/// </summary>
	/// <param name="spareInfo">Информация о запчастях для создания.</param>
	/// <returns>Массив созданных запчастей.</returns>
	public static Spare[] CreateSpares(IEnumerable<AddSpareCommandPayload> spareInfo) =>
		[
			.. spareInfo
				.Select(info =>
					SparesFactory.Create(
						containedItemId: info.ContainedItemId,
						source: info.Source,
						oem: info.Oem,
						title: info.Title,
						price: info.Price,
						isNds: info.IsNds,
						type: info.Type,
						address: info.Address,
						photoPaths: info.PhotoPaths
					)
				)
				.Where(r => r.IsSuccess)
				.Select(s => s.Value),
		];

	/// <summary>
	/// Выполняет команду добавления запчастей.
	/// </summary>
	/// <param name="command">Команда добавления запчастей.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды, содержащий идентификатор создателя и количество добавленных запчастей.</returns>
	public async Task<Result<(Guid, int)>> Execute(AddSparesCommand command, CancellationToken ct = default)
	{
		Guid creatorId = command.Creator.CreatorId;
		Result<Spare[]> spares = CreateSpares(command.Spares);
		int addedCount = await repository.AddMany(spares.Value, ct);
		return (creatorId, addedCount);
	}
}
