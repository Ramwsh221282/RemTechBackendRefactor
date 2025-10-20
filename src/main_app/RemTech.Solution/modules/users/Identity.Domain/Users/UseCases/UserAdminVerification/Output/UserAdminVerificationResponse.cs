using Identity.Domain.Tokens;

namespace Identity.Domain.Users.UseCases.UserAdminVerification.Output;

public sealed record UserAdminVerificationResponse(UserToken Token);
