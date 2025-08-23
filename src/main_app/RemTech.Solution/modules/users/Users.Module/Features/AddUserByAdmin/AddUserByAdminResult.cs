namespace Users.Module.Features.AddUserByAdmin;

internal sealed record AddUserByAdminResult(
    Guid Id,
    string Password,
    string Name,
    string Email,
    string Role
);
