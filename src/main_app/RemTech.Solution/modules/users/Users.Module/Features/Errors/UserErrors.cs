using RemTech.Core.Shared.Result;

namespace Users.Module.Features.Errors;

public static class UserErrors
{
    public static Error EmailNotFound(string email) =>
        Error.NotFound($"Пользователь с почтой {email} не найден.");

    public static Error AuthenticationPasswordFailed() => Error.PasswordIncorrect();

    public static Error PassowrdNotProvided() =>
        Error.Validation("Пароль для авторизации не предоставлен.");

    public static Error UserWithNameNotFound(string name) =>
        Error.NotFound($"Пользователь с псевдонимом {name} не найден.");

    public static Error ConfirmationEmailExpired() =>
        Error.Conflict(
            "Срок подтверждения почты закончился. Необходимо повторить процедуру регистрации."
        );

    public static Error EmailDuplicate() =>
        Error.Conflict("Пользователь с таким email уже существует.");

    public static Error EmailEmpty() => Error.Conflict("Строка почты была пустой.");

    public static Error EmailAlreadyConfirmed() => Error.Conflict("Почта уже подтверждена.");

    public static Error Forbidden() => Error.Forbidden();

    public static Error InvalidEmailFormat() => Error.Validation("Формат почты некорректен");

    public static Error NameDuplicate() =>
        Error.Conflict("Пользователь с таким псевдонимом уже существует.");

    public static Error NameExceesLength() => Error.Validation("Имя превышает длину 50 символов.");

    public static Error OnlyRootCanPromoteRoot() =>
        Error.Conflict("Только пользователь с Root правами может сделать Root пользователя.");

    public static Error PasswordDoesNotContainLowerCharacter() =>
        Error.Validation("Пароль должен содержать одну строчную букву");

    public static Error PasswordDoesNotContainSpecialSymbol() =>
        Error.Validation("Пароль должен содержать специальный символ");

    public static Error PasswordDoesNotContainUpperCharacter() =>
        Error.Validation("Пароль должен содержать заглавную букву");

    public static Error PasswordInvalid() => Error.Forbidden();

    public static Error PasswordLengthNotSatisfied() =>
        Error.Validation("Длина пароля меньше 8 символов.");

    public static Error PasswordShouldContainDigit() =>
        Error.Validation("Пароль должен содержать спец. символ.");

    public static Error RecoveryPasswordKeyNotFound() =>
        Error.NotFound("Не найден ключ для сброса пароля.");

    public static Error RoleNotFound(string role) => Error.NotFound($"Роль {role} не найдена.");

    public static Error RootUserCanBeAddedOnlyByRoot() =>
        Error.Conflict(
            "Пользователь с правами ROOT может быть добавлен только ROOT пользователем."
        );

    public static Error SendersAreNotAvailableError() =>
        Error.Conflict("Требуется наличие отправитилей писем в приложении.");

    public static Error TokensExpiredError() => Error.TokensExpired();

    public static Error UnableToDetermineHowToResetPassword() =>
        Error.Validation(
            "Не удается определить ни логин, ни почту пользователя для сброса пароля."
        );

    public static Error UnableToGetUserJwtValue() =>
        new("Unable to find user jwt session.", ErrorCodes.Unauthorized);

    public static Error UnableToResolveAuthentication() =>
        Error.Validation("Не удается определить метод атворизации.");

    public static Error UserNotFound() => Error.NotFound("Пользователь не найден.");

    public static Error JwtTokenDifferentComparison() =>
        new("Not origin jwt token.", ErrorCodes.Unauthorized);

    public static Error UserRecoveringPasswordInvalidException() =>
        new("Почта для операции восстановления некорректна.", ErrorCodes.Conflict);

    public static Error UserRecoveringPasswordByLoginEmpty() =>
        new("Не удается распознать псевдоним пользователя.", ErrorCodes.Conflict);

    public static Error UserRecoveringPasswordAlreadyExistsException() =>
        new("Пользователь уже создал заявку на сброс пароля.", ErrorCodes.Conflict);

    public static Error UserRecoveryPasswordTicketEmpty() =>
        new("Ключ подтверждения сброса пароля был пустым.", ErrorCodes.Conflict);

    public static Error UserRecoveryPasswordTicketNotValid() =>
        new("Ключ подтверждения сброса пароля некорректный.", ErrorCodes.Conflict);

    public static Error UserRegistrationRequiresPassword() =>
        new("Для создания учетной записи нужно указать почту", ErrorCodes.Validation);

    public static Error UserRegistrationRequiresNameError() =>
        new(
            "Для создания учетной записи нужно указать название учетной записи.",
            ErrorCodes.Validation
        );

    public static Error UserRegistrationRequiresPasswordError() =>
        new("Для создания учетной записи нужен пароль.", ErrorCodes.Validation);

    public static Error UserRecoverNotFoundException() =>
        new("Учетной записи пользователя с такими данными не существует.", ErrorCodes.NotFound);
}
