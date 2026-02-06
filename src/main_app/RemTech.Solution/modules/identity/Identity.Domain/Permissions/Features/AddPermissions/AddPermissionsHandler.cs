using Identity.Domain.Contracts.Persistence;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Permissions.Features.AddPermissions;

/// <summary>
/// Обработчик команды добавления разрешений.
/// </summary>
/// <param name="repository">Репозиторий для работы с разрешениями.</param>
public sealed class AddPermissionsHandler(IPermissionsRepository repository)
	: ICommandHandler<AddPermissionsCommand, IEnumerable<Permission>>
{
	/// <summary>
	/// Выполняет команду добавления разрешений.
	/// </summary>
	/// <param name="command">Команда добавления разрешений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с коллекцией добавленных разрешений.</returns>
	public async Task<Result<IEnumerable<Permission>>> Execute(
		AddPermissionsCommand command,
		CancellationToken ct = default
	)
	{
		IEnumerable<Permission> permissions = CreatePermissions(command.Permissions);
		IEnumerable<PermissionSpecification> specifications = CreateSpecificationsForExistanceCheck(permissions);
		IEnumerable<Permission> existing = await GetExistingPermissions(specifications, ct);

		if (HasDuplicates(existing, out Error error))
		{
			return error;
		}

		await repository.Add(permissions, ct);
		return Result.Success(permissions);
	}

	private static bool HasDuplicates(IEnumerable<Permission> duplicates, out Error error)
	{
		error = Error.NoError();
		if (duplicates.Any())
		{
			string message =
				$"Найдены дублирующиеся разрешения: {string.Join(" ", duplicates.Select(e => e.Name.Value))}";
			error = Error.Conflict(message);
			return true;
		}

		return false;
	}

	private static IEnumerable<Permission> CreatePermissions(IEnumerable<AddPermissionCommandPayload> payloads)
	{
		return payloads.Select(p =>
			Permission.CreateNew(PermissionName.Create(p.Name), PermissionDescription.Create(p.Description))
		);
	}

	private static IEnumerable<PermissionSpecification> CreateSpecificationsForExistanceCheck(
		IEnumerable<Permission> permissions
	)
	{
		return permissions.Select(p => new PermissionSpecification().WithName(p.Name.Value));
	}

	private Task<IEnumerable<Permission>> GetExistingPermissions(
		IEnumerable<PermissionSpecification> specifications,
		CancellationToken ct
	)
	{
		return repository.GetMany(specifications, ct);
	}
}
