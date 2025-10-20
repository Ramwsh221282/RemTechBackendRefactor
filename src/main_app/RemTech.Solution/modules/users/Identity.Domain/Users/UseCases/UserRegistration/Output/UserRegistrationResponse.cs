using Identity.Domain.Tokens;

namespace Identity.Domain.Users.UseCases.UserRegistration.Output;

public sealed record UserRegistrationResponse(UserToken Token);
