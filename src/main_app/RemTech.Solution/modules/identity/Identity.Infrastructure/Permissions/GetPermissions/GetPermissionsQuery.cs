using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Permissions.GetPermissions;

public sealed class GetPermissionsQuery : IQuery
{
	private GetPermissionsQuery() { }

	public static GetPermissionsQuery Create()
	{
		return new GetPermissionsQuery();
	}
}
