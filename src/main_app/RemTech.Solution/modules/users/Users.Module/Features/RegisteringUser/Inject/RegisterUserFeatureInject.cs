using Mailing.Module.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Users.Module.CommonAbstractions;
using Users.Module.Features.RegisteringUser.Exceptions;
using Users.Module.Features.RegisteringUser.Storage;
using Users.Module.Features.RegisteringUser.Validating;

namespace Users.Module.Features.RegisteringUser.Inject;

internal static class RegisterUserFeatureInject
{
    public static void Inject(RouteGroupBuilder builder) => builder.MapPost(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource source,
        [FromServices] Serilog.ILogger logger,
        [FromServices] MailingBusPublisher publisher,
        [FromServices] StringHash hash,
        [FromBody] RegisterUserRequest request,
        CancellationToken ct
    ) =>
        await new HttpApiUserRegistration(
            new UserToRegister(request.Name, request.Email, request.Password),
            new LoggingUsersStorage(
                logger,
                new MailPublishingUsersStorage(
                    publisher,
                    new ValidatingUsersStorage(
                        new ComposableValidation()
                            .With(
                                new NotEmptyStringValidation(
                                    request.Name,
                                    new UserRegistrationValidationFailedException(
                                        "Псевдоним был пустым."
                                    )
                                )
                            )
                            .With(
                                new NotEmptyStringValidation(
                                    request.Password,
                                    new UserRegistrationValidationFailedException(
                                        "Пароль был пустым."
                                    )
                                )
                            )
                            .With(
                                new NotEmptyStringValidation(
                                    request.Email,
                                    new UserRegistrationValidationFailedException(
                                        "Почта была пустой."
                                    )
                                )
                            )
                            .With(new EmailFormatValidation(request.Email))
                            .With(
                                new PasswordLengthValidation(
                                    request.Password,
                                    10,
                                    new UserRegistrationValidationFailedException(
                                        "Пароль должен быть больше 10 символов"
                                    )
                                )
                            ),
                        new PasswordHashingUsersStorage(
                            new StringHash(),
                            new PgUsersStorage(source)
                        )
                    )
                )
            ),
            logger
        ).Process(request, ct);
}
