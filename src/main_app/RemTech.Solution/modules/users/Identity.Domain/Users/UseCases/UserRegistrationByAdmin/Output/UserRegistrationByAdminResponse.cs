namespace Identity.Domain.Users.UseCases.UserRegistrationByAdmin.Output;

public sealed record UserRegistrationByAdminResponse(
    Guid Id,
    string Name,
    string Email,
    Guid RoleId
);
