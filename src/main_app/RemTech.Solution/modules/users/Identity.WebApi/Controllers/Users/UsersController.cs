using System.Net;
using Identity.Adapter.Auth.Middleware;
using Identity.Domain.Sessions;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.UseCases.Authenticate;
using Identity.Domain.Users.UseCases.Common;
using Identity.Domain.Users.UseCases.ConfirmEmailTicket;
using Identity.Domain.Users.UseCases.ConfirmPasswordReset;
using Identity.Domain.Users.UseCases.CreateEmailConfirmationTicket;
using Identity.Domain.Users.UseCases.CreatePasswordResetTicket;
using Identity.Domain.Users.UseCases.CreateRoot;
using Identity.Domain.Users.UseCases.ReadUserInfo;
using Identity.Domain.Users.UseCases.ReadUsers;
using Identity.Domain.Users.UseCases.UserRegistration;
using Identity.WebApi.Filters;
using Identity.WebApi.Requests;
using Identity.WebApi.Responses;
using Microsoft.AspNetCore.Mvc;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.WebApi;

namespace Identity.WebApi.Controllers.Users;

/// <summary>
/// Контроллер для управления пользователями: аутентификация, регистрация, подтверждение email,
/// сброс пароля и получение информации о пользователе.
/// </summary>
[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    /// <summary>
    /// Аутентифицирует пользователя по логину/email и паролю.
    /// </summary>
    /// <param name="request">Данные для аутентификации: логин/email и пароль.</param>
    /// <param name="handler">Обработчик команды <see cref="AuthenticateCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="AuthorizedUserSessionResult"/> при успешной аутентификации (содержит сессию).
    /// - <see cref="HttpEnvelope"/> с ошибкой, если аутентификация не удалась.
    /// </returns>
    [HttpPost("auth")]
    public async Task<IResult> Auth(
        [FromBody] AuthRequest request,
        [FromServices] ICommandHandler<AuthenticateCommand, Status<UserSession>> handler,
        CancellationToken ct = default
    )
    {
        var command = new AuthenticateCommand(request.Login, request.Email, request.Password);
        var session = await handler.Handle(command, ct);
        return session.IsFailure
            ? new HttpEnvelope(session)
            : new AuthorizedUserSessionResult(session);
    }

    /// <summary>
    /// Регистрирует нового пользователя.
    /// </summary>
    /// <param name="request">Данные для регистрации: логин, email и пароль.</param>
    /// <param name="handler">Обработчик команды <see cref="UserRegistrationCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope.NoContent()"/> при успешной регистрации.
    /// - <see cref="HttpEnvelope"/> с ошибкой в случае неудачи (например, дублирующийся email).
    /// </returns>
    [HttpPost("sign-up")]
    public async Task<IResult> SignUp(
        [FromBody] UserRegistrationRequest request,
        ICommandHandler<UserRegistrationCommand, Status<User>> handler,
        CancellationToken ct = default
    )
    {
        var command = new UserRegistrationCommand(request.Login, request.Email, request.Password);
        var status = await handler.Handle(command, ct);
        return status.IsFailure ? new HttpEnvelope(status) : HttpEnvelope.NoContent();
    }

    /// <summary>
    /// Создаёт первого (root) пользователя в системе.
    /// Доступ ограничен фильтром <see cref="FirstRootFilter"/>.
    /// </summary>
    /// <param name="request">Данные для создания root-пользователя.</param>
    /// <param name="handler">Обработчик команды <see cref="CreateRootUserCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope.NoContent()"/> при успешном создании.
    /// - <see cref="HttpEnvelope"/> с ошибкой в случае неудачи.
    /// </returns>
    [HttpPost("root")]
    [TypeFilter<FirstRootFilter>]
    public async Task<IResult> CreateRoot(
        [FromBody] CreateRootRequest request,
        ICommandHandler<CreateRootUserCommand, Status<User>> handler,
        CancellationToken ct = default
    )
    {
        var command = new CreateRootUserCommand(request.Email, request.Login, request.Password);
        var status = await handler.Handle(command, ct);
        return status.IsFailure ? new HttpEnvelope(status) : HttpEnvelope.NoContent();
    }

    /// <summary>
    /// Создаёт тикет подтверждения email для указанного пользователя.
    /// Требуется указать пароль в заголовке запроса.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="password">Пароль пользователя (передаётся в заголовке "password").</param>
    /// <param name="handler">Обработчик команды <see cref="CreateEmailConfirmationTicketCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope.Ok()"/> при успешном создании тикета.
    /// - <see cref="HttpEnvelope"/> с ошибкой, если пользователь не найден или пароль неверен.
    /// </returns>
    [HttpPost("{id:guid}/email/ticket")]
    public async Task<IResult> CreateConfirmationTicket(
        [FromRoute] Guid id,
        [FromHeader(Name = "password")] string password,
        ICommandHandler<CreateEmailConfirmationTicketCommand, Status<User>> handler,
        CancellationToken ct = default
    )
    {
        var command = new CreateEmailConfirmationTicketCommand(id, password);
        var status = await handler.Handle(command, ct);
        return status.IsFailure ? new HttpEnvelope(status) : HttpEnvelope.Ok();
    }

    /// <summary>
    /// Подтверждает email пользователя по тикету.
    /// </summary>
    /// <param name="id">Идентификатор тикета подтверждения email.</param>
    /// <param name="handler">Обработчик команды <see cref="ConfirmEmailTicketCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope{UserDto}"/> с данными подтверждённого пользователя.
    /// - <see cref="HttpEnvelope"/> с ошибкой, если тикет недействителен или просрочен.
    /// </returns>
    [HttpGet("email/ticket/{id:guid}")]
    public async Task<IResult> ConfirmEmail(
        [FromRoute] Guid id,
        ICommandHandler<ConfirmEmailTicketCommand, Status<User>> handler,
        CancellationToken ct = default
    )
    {
        var command = new ConfirmEmailTicketCommand(id);
        var status = await handler.Handle(command, ct);
        return status.IsFailure
            ? new HttpEnvelope(status)
            : new HttpEnvelope<UserDto>(new UserDto(status), HttpStatusCode.OK);
    }

    /// <summary>
    /// Создаёт тикет сброса пароля для указанного пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="handler">Обработчик команды <see cref="CreatePasswordResetCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope.Ok()"/> при успешном создании тикета.
    /// - <see cref="HttpEnvelope"/> с ошибкой, если пользователь не найден.
    /// </returns>
    [HttpPost("{id:guid}/password/ticket")]
    public async Task<IResult> CreatePasswordTicket(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<CreatePasswordResetCommand, Status<User>> handler,
        CancellationToken ct
    )
    {
        var command = new CreatePasswordResetCommand(IssuerId: id);
        var status = await handler.Handle(command, ct);
        return status.IsFailure ? new HttpEnvelope(status) : HttpEnvelope.Ok();
    }

    /// <summary>
    /// Возвращает информацию о пользователе по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="handler">Обработчик команды <see cref="ReadUserInfoCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope{UserDto}"/> с данными пользователя.
    /// - <see cref="HttpEnvelope"/> с ошибкой, если пользователь не найден.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<IResult> ReadUserById(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<ReadUserInfoCommand, Status<User>> handler,
        CancellationToken ct = default
    )
    {
        var command = new ReadUserInfoCommand(id);
        var status = await handler.Handle(command, ct);
        return status.IsFailure ? new HttpEnvelope(status) : new HttpEnvelope<UserDto>(status);
    }

    /// <summary>
    /// Проверяет существование пользователя по email или логину.
    /// Используется, например, для проверки доступности логина/email при регистрации.
    /// </summary>
    /// <param name="email">Email пользователя (опционально).</param>
    /// <param name="login">Логин пользователя (опционально).</param>
    /// <param name="byEmail">Обработчик запроса по email.</param>
    /// <param name="byLogin">Обработчик запроса по логину.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope.NoContent()"/> если пользователь найден.
    /// - <see cref="HttpEnvelope"/> с ошибкой <c>404 Not Found</c>, если пользователь не найден или не указаны параметры.
    /// </returns>
    [HttpGet("user")]
    public async Task<IResult> Login(
        [FromQuery(Name = "email")] string? email,
        [FromQuery(Name = "login")] string? login,
        [FromServices] IGetUserByEmailHandle byEmail,
        [FromServices] IGetUserByLoginHandle byLogin,
        CancellationToken ct
    )
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            var user = await byEmail.Handle(email, ct);
            return user.IsFailure ? new HttpEnvelope(user) : HttpEnvelope.NoContent();
        }

        if (!string.IsNullOrWhiteSpace(login))
        {
            var user = await byLogin.Handle(login, ct);
            return user.IsFailure ? new HttpEnvelope(user) : HttpEnvelope.NoContent();
        }

        var notFound = Status.NotFound("Пользователь не найден.");
        return new HttpEnvelope(notFound);
    }

    /// <summary>
    /// Подтверждает сброс пароля по тикету и устанавливает новый пароль.
    /// Новый пароль передаётся в заголовке "password".
    /// </summary>
    /// <param name="id">Идентификатор тикета сброса пароля.</param>
    /// <param name="password">Новый пароль (передаётся в заголовке "password").</param>
    /// <param name="handler">Обработчик команды <see cref="ConfirmPasswordResetCommand"/>.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// - <see cref="HttpEnvelope.Ok()"/> при успешном сбросе пароля.
    /// - <see cref="HttpEnvelope"/> с ошибкой, если тикет недействителен или пароль не соответствует требованиям.
    /// </returns>
    [HttpGet("password/ticket/{id:guid}")]
    public async Task<IResult> ConfirmPasswordTicket(
        [FromRoute] Guid id,
        [FromHeader(Name = "password")] string password,
        [FromServices] ICommandHandler<ConfirmPasswordResetCommand, Status<User>> handler,
        CancellationToken ct
    )
    {
        var command = new ConfirmPasswordResetCommand(id, password);
        var status = await handler.Handle(command, ct);
        return status.IsFailure ? new HttpEnvelope(status) : HttpEnvelope.Ok();
    }

    [HttpGet]
    public async Task<IResult> GetUsers(
        [FromQuery(Name = "email")] string? email,
        [FromQuery(Name = "login")] string? login,
        [FromQuery(Name = "roles")] IEnumerable<string>? roles,
        [FromQuery(Name = "ordering")] IEnumerable<string>? ordering,
        [FromQuery(Name = "order")] string? order,
        [FromQuery(Name = "page")] int? page,
        [FromQuery(Name = "pageSize")] int? pageSize,
        [FromQuery(Name = "verifiedOnly")] bool? verifiedOnly,
        [FromServices] ICommandHandler<ReadUsersCommand, IEnumerable<User>> handler,
        CancellationToken ct
    )
    {
        string actualOrder = string.IsNullOrWhiteSpace(order) ? "ASC" : order;
        order = actualOrder switch
        {
            "ASC" => "ASC",
            "DESC" => "DESC",
            _ => "ASC",
        };

        page ??= 1;
        pageSize ??= 20;

        var speicifaction = new UsersSpecification(
            login,
            email,
            roles,
            ordering,
            verifiedOnly,
            order,
            page.Value,
            pageSize.Value
        );

        var command = new ReadUsersCommand(speicifaction);
        var result = await handler.Handle(command, ct);
        var dtos = result.Select(r => new UserDto(r));
        return HttpEnvelope<IEnumerable<UserDto>>.Ok(dtos);
    }
}
