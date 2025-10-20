using Identity.Domain.Tokens;

namespace Identity.Domain.Users.UseCases.UserTokenVerification.Output;

public sealed record UserTokenVerificationResponse(UserToken Token);
