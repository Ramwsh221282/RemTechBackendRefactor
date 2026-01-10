using System.Net;
using Identity.Domain.Accounts.Features.Authenticate;
using Identity.Domain.Accounts.Features.ConfirmTicket;
using Identity.Domain.Accounts.Features.GivePermissions;
using Identity.Domain.Accounts.Features.Logout;
using Identity.Domain.Accounts.Features.Refresh;
using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Features.VerifyToken;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Tokens;
using Identity.Infrastructure.Accounts.Queries.GetUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.ActionFilters.Common;
using WebHostApplication.Common.Envelope;
using WebHostApplication.Modules.identity.Requests;
using WebHostApplication.Modules.identity.Responses;

namespace WebHostApplication.Modules.identity;

[ApiController]
[Route("api/identity")]
public sealed class IdentityController : Controller
{
    private static readonly Func<HttpContext, string>[] GetAccessTokenMethods =
    [
        context => context.GetAccessTokenFromHeaderOrEmpty(),
        context => context.GetAccessTokenFromCookieOrEmpty()
    ];
    
    private static readonly Func<HttpContext, string>[] GetRefreshTokenMethods =
    [
        context => context.GetRefreshTokenFromHeaderOrEmpty(),
        context => context.GetRefreshTokenFromCookieOrEmpty()
    ];
    
    [HttpPost("auth")]
    public async Task<Envelope> Authenticate(
        [FromBody] AuthenticateRequest request,
        [FromServices] ICommandHandler<AuthenticateCommand, AuthenticationResult> handler,
        CancellationToken ct)
    {
        AuthenticateCommand command = new(request.Login, request.Email, request.Password);
        Result<AuthenticationResult> result = await handler.Execute(command, ct);
        if (result.IsFailure) 
            return result.AsEnvelope();
        SetAuthCookies(HttpContext, result.Value);
        SetAuthHeaders(HttpContext, result.Value);
        return Ok();
    }

    [HttpPost("logout")]
    public async Task<Envelope> Logout(
        [FromServices] ICommandHandler<LogoutCommand, Unit> handler,
        CancellationToken ct)
    {
        (string accessToken, string refreshToken) = HttpContext.GetIdentityTokens(GetAccessTokenMethods, GetRefreshTokenMethods);
        LogoutCommand command = new(accessToken, refreshToken);
        Result<Unit> result = await handler.Execute(command, ct);
        if (result.IsFailure) return result.AsEnvelope();
        ClearAuthHeaders(HttpContext);
        return Ok();
    }
    
    [VerifyToken]
    [HttpGet("account")]
    public async Task<Envelope> GetUserAccount(
        [FromServices] IJwtTokenManager jwtManager,
        [FromServices] IQueryHandler<GetUserQuery, UserAccountResponse?> handler,
        CancellationToken ct)
    {
        string accessToken = HttpContext.GetAccessToken(GetAccessTokenMethods);
        AccessToken token = jwtManager.ReadToken(accessToken);
        GetUserQuery query = new(token.UserId);
        UserAccountResponse? user = await handler.Handle(query, ct);
        return EnvelopeExtensions.NotFoundOrOk(user, "Пользователь не найден.");
    }
    
    
    [HttpGet("confirmation")]
    public async Task<Envelope> ConfirmTicket(
        [FromRoute(Name = "account-id")] Guid accountId,
        [FromRoute(Name = "ticket-id")] Guid ticketId,
        [FromServices] ICommandHandler<ConfirmTicketCommand, Account> handler,
        CancellationToken ct
        )
    {
        ConfirmTicketCommand command = new(accountId, ticketId);
        Result<Account> result = await handler.Execute(command, ct);
        return result.IsFailure ? result.AsEnvelope() : Ok();
    }
    
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
        IEnumerable<GivePermissionsPermissionsPayload> payloads = request.PermissionIds.Select(id => new GivePermissionsPermissionsPayload(id));
        GivePermissionsCommand command = new(accountId, payloads);
        Result<Account> result = await handler.Execute(command, ct);
        return result.AsTypedEnvelope(AccountResponse.ConvertFrom);
    }

    [HttpPut("refresh")]
    public async Task<Envelope> RefreshToken(
        [FromServices] ICommandHandler<RefreshTokenCommand, AuthenticationResult> handler,
        CancellationToken ct)
    {
        (string accessToken, string refreshToken) = HttpContext.GetIdentityTokens(GetAccessTokenMethods, GetRefreshTokenMethods);
        RefreshTokenCommand command = new(accessToken, refreshToken);
        Result<AuthenticationResult> result = await handler.Execute(command, ct);
        
        if (result.IsFailure) 
            return result.AsEnvelope();
        
        SetAuthCookies(HttpContext, result.Value);
        SetAuthHeaders(HttpContext, result.Value);
        return Ok();
    }

    [HttpPost("sign-up")]
    public async Task<Envelope> SignUp(
        [FromBody] RegisterAccountRequest request,
        [FromServices] ICommandHandler<RegisterAccountCommand, Unit> handler,
        CancellationToken ct)
    {
        RegisterAccountCommand command = new(request.Email, request.Login, request.Password);
        Result<Unit> result = await handler.Execute(command, ct);
        return result.IsFailure ? result.AsEnvelope() : Ok();
    }

    [HttpPost("verify")]
    public async Task<Envelope> Verify(
        [FromServices] ICommandHandler<VerifyTokenCommand, Unit> handler,
        CancellationToken ct
        )
    {
        string accessToken = HttpContext.GetAccessToken(GetAccessTokenMethods);
        VerifyTokenCommand command = new(accessToken);
        Result<Unit> result = await handler.Execute(command, ct);
        return result.IsFailure ? result.AsEnvelope() : Ok();
    }
    
    private static new Envelope Ok() => new((int)HttpStatusCode.OK, null, null);

    private static void SetAuthHeaders(HttpContext context, AuthenticationResult result)
    {
        context.Response.Headers.Remove("access_token");
        context.Response.Headers.Remove("refresh_token");
        
        context.Response.Headers.Append("access_token", result.AccessToken);
        context.Response.Headers.Append("refresh_token", result.RefreshToken);
    }
    
    private static void ClearAuthHeaders(HttpContext context)
    {
        context.Response.Headers.Remove("access_token");
        context.Response.Headers.Remove("refresh_token");
        context.Response.Cookies.Delete("access_token");
        context.Response.Cookies.Delete("refresh_token");
    }
    
    private static void SetAuthCookies(HttpContext context, AuthenticationResult result)
    {
        context.Response.Cookies.Delete("access_token");
        context.Response.Cookies.Delete("refresh_token");
        
        context.Response.Cookies.Append("access_token", result.AccessToken);
        context.Response.Cookies.Append("refresh_token", result.RefreshToken);
    }
}