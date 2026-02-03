using System.Net;
using Identity.Domain.Accounts.Features.Authenticate;
using Identity.Domain.Accounts.Features.ChangePassword;
using Identity.Domain.Accounts.Features.ConfirmPasswordReset;
using Identity.Domain.Accounts.Features.ConfirmTicket;
using Identity.Domain.Accounts.Features.GivePermissions;
using Identity.Domain.Accounts.Features.Logout;
using Identity.Domain.Accounts.Features.Refresh;
using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Features.ResetPassword;
using Identity.Domain.Accounts.Features.VerifyToken;
using Identity.Domain.Accounts.Models;
using Identity.Infrastructure.Accounts.Queries.GetUser;
using Identity.Infrastructure.Permissions.GetPermissions;
using Microsoft.AspNetCore.Mvc;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.ActionFilters.Common;
using WebHostApplication.Common.Envelope;
using WebHostApplication.Modules.identity.Requests;
using WebHostApplication.Modules.identity.Responses;

namespace WebHostApplication.Modules.identity;

/// <summary>
/// Контроллер для работы с идентификацией и управлением учетными записями.
/// </summary>
[ApiController]
[Route("api/identity")]
public sealed class IdentityController : ControllerBase
{
	private const string ACCESS_TOKEN_NAME = "access_token";
	private const string REFRESH_TOKEN_NAME = "refresh_token";

	private static readonly Func<HttpContext, string>[] _getAccessTokenMethods =
	[
		context => context.GetAccessTokenFromHeaderOrEmpty(),
		context => context.GetAccessTokenFromCookieOrEmpty(),
	];

	private static readonly Func<HttpContext, string>[] _getRefreshTokenMethods =
	[
		context => context.GetRefreshTokenFromHeaderOrEmpty(),
		context => context.GetRefreshTokenFromCookieOrEmpty(),
	];

	[HttpGet("permissions")]
	public async Task<Envelope> GetPermissions(
		[FromServices] IQueryHandler<GetPermissionsQuery, IReadOnlyList<PermissionResponse>> handler,
		CancellationToken ct
	)
	{
		GetPermissionsQuery query = GetPermissionsQuery.Create();
		IReadOnlyList<PermissionResponse> permissions = await handler.Handle(query, ct);
		return new Envelope((int)HttpStatusCode.OK, permissions, null);
	}

	/// <summary>
	/// Подтвердить сброс пароля.
	/// </summary>
	/// <param name="accountId">Идентификатор учетной записи.</param>
	/// <param name="ticketId">Идентификатор тикета сброса пароля.</param>
	/// <param name="request">Данные запроса для подтверждения сброса пароля.</param>
	/// <param name="handler">Обработчик команды подтверждения сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPost("accounts/{id:guid}/tickets/{ticketId:guid}/commit-password-reset")]
	public async Task<Envelope> CommitPasswordReset(
		[FromRoute(Name = "id")] Guid accountId,
		[FromRoute(Name = "ticketId")] Guid ticketId,
		[FromBody] CommitPasswordResetRequest request,
		[FromServices] ICommandHandler<ConfirmResetPasswordCommand, Unit> handler,
		CancellationToken ct
	)
	{
		ConfirmResetPasswordCommand command = new(accountId, ticketId, request.NewPassword);
		Result<Unit> result = await handler.Execute(command, ct);
		return result.IsFailure ? result.AsEnvelope() : Ok();
	}

	/// <summary>
	/// Сбросить пароль.
	/// </summary>
	/// <param name="request">Данные запроса для сброса пароля.</param>
	/// <param name="handler">Обработчик команды сброса пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPost("reset-password")]
	public async Task<Envelope> ResetPassword(
		[FromBody] ResetPasswordRequest request,
		[FromServices] ICommandHandler<ResetPasswordCommand, ResetPasswordResult> handler,
		CancellationToken ct
	)
	{
		ResetPasswordCommand command = new(request.Login, request.Email);
		Result<ResetPasswordResult> result = await handler.Execute(command, ct);
		return result.IsFailure ? result.AsEnvelope() : Ok();
	}

	/// <summary>
	/// Аутентификация пользователя.
	/// </summary>
	/// <param name="request">Данные запроса для аутентификации пользователя.</param>
	/// <param name="handler">Обработчик команды аутентификации пользователя.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPost("auth")]
	public async Task<Envelope> Authenticate(
		[FromBody] AuthenticateRequest request,
		[FromServices] ICommandHandler<AuthenticateCommand, AuthenticationResult> handler,
		CancellationToken ct
	)
	{
		AuthenticateCommand command = new(request.Login, request.Email, request.Password);
		Result<AuthenticationResult> result = await handler.Execute(command, ct);
		if (result.IsFailure)
		{
			return result.AsEnvelope();
		}

		SetAuthCookies(HttpContext, result.Value);
		SetAuthHeaders(HttpContext, result.Value);
		return Ok();
	}

	/// <summary>
	/// Изменить пароль пользователя.
	/// </summary>
	/// <param name="id">Идентификатор пользователя.</param>
	/// <param name="request">Данные запроса для изменения пароля.</param>
	/// <param name="handler">Обработчик команды изменения пароля.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[HttpPatch("{id:guid}/password")]
	public async Task<Envelope> ChangePassword(
		[FromRoute(Name = "id")] Guid id,
		[FromBody] ChangePasswordRequest request,
		[FromServices] ICommandHandler<ChangePasswordCommand, Unit> handler,
		CancellationToken ct
	)
	{
		(string accessToken, string refreshToken) = HttpContext.GetIdentityTokens(
			_getAccessTokenMethods,
			_getRefreshTokenMethods
		);
		ChangePasswordCommand command = new(
			accessToken,
			refreshToken,
			id,
			request.NewPassword,
			request.CurrentPassword
		);
		Result<Unit> result = await handler.Execute(command, ct);
		if (result.IsFailure)
		{
			return result.AsEnvelope();
		}

		ClearAuthHeaders(HttpContext);
		return Ok();
	}

	/// <summary>
	/// Выйти из системы.
	/// </summary>
	/// <param name="handler">Обработчик команды выхода из системы.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPost("logout")]
	public async Task<Envelope> Logout(
		[FromServices] ICommandHandler<LogoutCommand, Unit> handler,
		CancellationToken ct
	)
	{
		(string accessToken, string refreshToken) = HttpContext.GetIdentityTokens(
			_getAccessTokenMethods,
			_getRefreshTokenMethods
		);
		LogoutCommand command = new(accessToken, refreshToken);
		Result<Unit> result = await handler.Execute(command, ct);
		if (result.IsFailure)
		{
			return result.AsEnvelope();
		}

		ClearAuthHeaders(HttpContext);
		return Ok();
	}

	/// <summary>
	///  Получить информацию о пользователе по токену обновления.
	/// </summary>
	/// <param name="handler">Обработчик запроса получения информации о пользователе.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[HttpGet("account")]
	public async Task<Envelope> GetUserAccount(
		[FromServices] IQueryHandler<GetUserQuery, UserAccountResponse?> handler,
		CancellationToken ct
	)
	{
		(_, string refreshToken) = HttpContext.GetIdentityTokens([], _getRefreshTokenMethods);
		GetUserByRefreshTokenQuery query = new(refreshToken);
		UserAccountResponse? user = await handler.Handle(query, ct);
		return EnvelopeFactory.NotFoundOrOk(user, "Пользователь не найден.");
	}

	/// <summary>
	/// Подтвердить тикет.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="ticketId">Идентификатор тикета.</param>
	/// <param name="handler">Обработчик команды подтверждения тикета.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpGet("confirmation")]
	public async Task<Envelope> ConfirmTicket(
		[FromQuery(Name = "accountId")] Guid accountId,
		[FromQuery(Name = "ticketId")] Guid ticketId,
		[FromServices] ICommandHandler<ConfirmTicketCommand, Account> handler,
		CancellationToken ct
	)
	{
		ConfirmTicketCommand command = new(accountId, ticketId);
		Result<Account> result = await handler.Execute(command, ct);
		return result.IsFailure ? result.AsEnvelope() : Ok();
	}

	/// <summary>
	/// Выдать разрешения пользователю.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="request">Запрос с данными для выдачи разрешений.</param>
	/// <param name="handler">Обработчик команды выдачи разрешений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[IdentityManagementPermission]
	[HttpPatch("account/{id:guid}/permissions")]
	public async Task<Envelope> GivePermissions(
		[FromRoute(Name = "id")] Guid accountId,
		[FromBody] GivePermissionsRequest request,
		[FromServices] ICommandHandler<GivePermissionsCommand, Account> handler,
		CancellationToken ct
	)
	{
		IEnumerable<GivePermissionsPermissionsPayload> payloads = request.PermissionIds.Select(
			id => new GivePermissionsPermissionsPayload(id)
		);
		GivePermissionsCommand command = new(accountId, payloads);
		Result<Account> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(AccountResponse.ConvertFrom);
	}

	/// <summary>
	/// Обновить токен аутентификации.
	/// </summary>
	/// <param name="handler">Обработчик команды обновления токена.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPut("refresh")]
	public async Task<Envelope> RefreshToken(
		[FromServices] ICommandHandler<RefreshTokenCommand, AuthenticationResult> handler,
		CancellationToken ct
	)
	{
		(string accessToken, string refreshToken) = HttpContext.GetIdentityTokens(
			_getAccessTokenMethods,
			_getRefreshTokenMethods
		);
		RefreshTokenCommand command = new(accessToken, refreshToken);
		Result<AuthenticationResult> result = await handler.Execute(command, ct);

		if (result.IsFailure)
		{
			return result.AsEnvelope();
		}

		SetAuthCookies(HttpContext, result.Value);
		SetAuthHeaders(HttpContext, result.Value);
		return Ok();
	}

	/// <summary>
	/// Регистрация нового пользователя.
	/// </summary>
	/// <param name="request">Запрос с данными для регистрации.</param>
	/// <param name="handler">Обработчик команды регистрации.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPost("sign-up")]
	public async Task<Envelope> SignUp(
		[FromBody] RegisterAccountRequest request,
		[FromServices] ICommandHandler<RegisterAccountCommand, Unit> handler,
		CancellationToken ct
	)
	{
		RegisterAccountCommand command = new(request.Email, request.Login, request.Password);
		Result<Unit> result = await handler.Execute(command, ct);
		return result.IsFailure ? result.AsEnvelope() : Ok();
	}

	/// <summary>
	/// Проверить токен аутентификации.
	/// </summary>
	/// <param name="handler">Обработчик команды проверки токена.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[HttpPost("verify")]
	public async Task<Envelope> Verify(
		[FromServices] ICommandHandler<VerifyTokenCommand, Unit> handler,
		CancellationToken ct
	)
	{
		string accessToken = HttpContext.GetAccessToken(_getAccessTokenMethods);
		VerifyTokenCommand command = new(accessToken);
		Result<Unit> result = await handler.Execute(command, ct);
		return result.IsFailure ? result.AsEnvelope() : Ok();
	}

	private static new Envelope Ok()
	{
		return new((int)HttpStatusCode.OK, null, null);
	}

	private static void SetAuthHeaders(HttpContext context, AuthenticationResult result)
	{
		context.Response.Headers.Remove(ACCESS_TOKEN_NAME);
		context.Response.Headers.Remove(REFRESH_TOKEN_NAME);
		context.Response.Headers.Append(ACCESS_TOKEN_NAME, result.AccessToken);
		context.Response.Headers.Append(REFRESH_TOKEN_NAME, result.RefreshToken);
	}

	private static void ClearAuthHeaders(HttpContext context)
	{
		CookieOptions options = CreateCookieOptions(context);
		context.Response.Headers.Remove(ACCESS_TOKEN_NAME);
		context.Response.Headers.Remove(REFRESH_TOKEN_NAME);
		context.Response.Cookies.Delete(ACCESS_TOKEN_NAME, options);
		context.Response.Cookies.Delete(REFRESH_TOKEN_NAME, options);
	}

	private static void SetAuthCookies(HttpContext context, AuthenticationResult result)
	{
		CookieOptions options = CreateCookieOptions(context);
		context.Response.Cookies.Delete(ACCESS_TOKEN_NAME, options);
		context.Response.Cookies.Delete(REFRESH_TOKEN_NAME, options);
		context.Response.Cookies.Append(ACCESS_TOKEN_NAME, result.AccessToken, options);
		context.Response.Cookies.Append(REFRESH_TOKEN_NAME, result.RefreshToken, options);
	}

	private static CookieOptions CreateCookieOptions(HttpContext context)
	{
		bool isHttps = context.Request.IsHttps;
		return new()
		{
			HttpOnly = true,
			SameSite = !isHttps ? SameSiteMode.Lax : SameSiteMode.None,
			Secure = isHttps,
			Path = "/",
		};
	}
}
