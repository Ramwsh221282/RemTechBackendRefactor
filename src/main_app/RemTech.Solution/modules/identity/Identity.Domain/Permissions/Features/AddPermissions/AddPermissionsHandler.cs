using Identity.Domain.Contracts.Persistence;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Permissions.Features.AddPermissions;

public sealed class AddPermissionsHandler(IPermissionsRepository repository)
	: ICommandHandler<AddPermissionsCommand, IEnumerable<Permission>>
{
	public async Task<Result<IEnumerable<Permission>>> Execute(
		AddPermissionsCommand command,
		CancellationToken ct = default
	)
	{
		IEnumerable<Permission> permissions = CreatePermissions(command.Permissions);
		IEnumerable<PermissionSpecification> specifications = CreateSpecificationsForExistanceCheck(permissions);
		IEnumerable<Permission> existing = await GetExistingPermissions(specifications, ct);

		if (HasDuplicates(existing, out Error error))
			return error;
		await repository.Add(permissions, ct);
		return Result.Success(permissions);
	}

	private bool HasDuplicates(IEnumerable<Permission> duplicates, out Error error)
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

	private IEnumerable<Permission> CreatePermissions(IEnumerable<AddPermissionCommandPayload> payloads) =>
		payloads.Select(p =>
			Permission.CreateNew(PermissionName.Create(p.Name), PermissionDescription.Create(p.Description))
		);

	private IEnumerable<PermissionSpecification> CreateSpecificationsForExistanceCheck(
		IEnumerable<Permission> permissions
	) => permissions.Select(p => new PermissionSpecification().WithName(p.Name.Value));

	private async Task<IEnumerable<Permission>> GetExistingPermissions(
		IEnumerable<PermissionSpecification> specifications,
		CancellationToken ct
	) => await repository.GetMany(specifications, ct);
}
