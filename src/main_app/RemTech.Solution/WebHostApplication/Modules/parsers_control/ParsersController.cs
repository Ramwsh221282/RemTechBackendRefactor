using System.Net;
using Microsoft.AspNetCore.Mvc;
using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.Features.ChangeLinkActivity;
using ParsersControl.Core.Features.ChangeWaitDays;
using ParsersControl.Core.Features.DeleteLinkFromParser;
using ParsersControl.Core.Features.DisableParser;
using ParsersControl.Core.Features.PermantlyDisableManyParsing;
using ParsersControl.Core.Features.PermantlyDisableParsing;
using ParsersControl.Core.Features.PermantlyStartManyParsing;
using ParsersControl.Core.Features.PermantlyStartParsing;
using ParsersControl.Core.Features.StartParserWork;
using ParsersControl.Core.Features.UpdateParserLink;
using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.Queries.GetParser;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Web;
using WebHostApplication.ActionFilters.Attributes;
using WebHostApplication.Common.Envelope;

namespace WebHostApplication.Modules.parsers_control;

/// <summary>
/// Контроллер для управления парсерами.
/// </summary>
[ApiController]
[Route("api/parsers")]
public sealed class ParsersController : ControllerBase
{
	/// <summary>
	/// Запустить парсер.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="handler">Обработчик команды запуска парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPost("{id:guid}/start")]
	public async Task<Envelope> StartParser(
		[FromRoute(Name = "id")] Guid id,
		[FromServices] ICommandHandler<StartParserCommand, SubscribedParser> handler,
		CancellationToken ct
	)
	{
		StartParserCommand command = new(Id: id);
		Result<SubscribedParser> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserResponse.Create);
	}

	/// <summary>
	/// Удалить ссылку из парсера.
	/// </summary>
	/// <param name="parserId">Идентификатор парсера.</param>
	/// <param name="linkId">Идентификатор ссылки парсера.</param>
	/// <param name="handler">Обработчик команды удаления ссылки из парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpDelete("{id:guid}/links/{linkId:guid}")]
	public async Task<Envelope> RemoveLinkFromParser(
		[FromRoute(Name = "id")] Guid parserId,
		[FromRoute(Name = "linkId")] Guid linkId,
		[FromServices] ICommandHandler<DeleteLinkFromParserCommand, SubscribedParserLink> handler,
		CancellationToken ct
	)
	{
		DeleteLinkFromParserCommand command = new(parserId, linkId);
		Result<SubscribedParserLink> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserLinkResponse.Create);
	}

	/// <summary>
	/// Изменить количество дней ожидания парсера.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="value">Новое значение количества дней ожидания.</param>
	/// <param name="handler">Обработчик команды изменения количества дней ожидания.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPatch("{id:guid}/wait-days")]
	public async Task<Envelope> ChangeWaitDays(
		[FromRoute(Name = "id")] Guid id,
		[FromQuery(Name = "value")] int value,
		[FromServices] ICommandHandler<ChangeWaitDaysCommand, SubscribedParser> handler,
		CancellationToken ct
	)
	{
		ChangeWaitDaysCommand command = new(id, value);
		Result<SubscribedParser> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserResponse.Create);
	}

	/// <summary>
	/// Обновить ссылку парсера.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="linkId">Идентификатор ссылки парсера.</param>
	/// <param name="name">Новое название ссылки парсера.</param>
	/// <param name="url">Новый URL ссылки парсера.</param>
	/// <param name="handler">Обработчик команды обновления ссылки парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPut("{id:guid}/links/{linkId:guid}")]
	public async Task<Envelope> UpdateParserLink(
		[FromRoute(Name = "id")] Guid id,
		[FromRoute(Name = "linkId")] Guid linkId,
		[FromQuery(Name = "name")] string? name,
		[FromQuery(Name = "url")] string? url,
		[FromServices] ICommandHandler<UpdateParserLinkCommand, SubscribedParserLink> handler,
		CancellationToken ct
	)
	{
		UpdateParserLinkCommand command = new(id, linkId, name, url);
		Result<SubscribedParserLink> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserLinkResponse.Create);
	}

	/// <summary>
	/// Получить список парсеров.
	/// </summary>
	/// <param name="handler">Обработчик запроса получения списка парсеров.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpGet]
	public async Task<Envelope> GetParsers(
		[FromServices] IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>> handler,
		CancellationToken ct
	)
	{
		GetParsersQuery query = new();
		IEnumerable<ParserResponse> parsers = await handler.Handle(query, ct);
		return new Envelope((int)HttpStatusCode.OK, parsers, null);
	}

	/// <summary>
	/// Получить информацию о парсере.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="handler">Обработчик запроса получения информации о парсере.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpGet("{id:guid}")]
	public async Task<Envelope> GetParser(
		[FromRoute(Name = "id")] Guid id,
		[FromServices] IQueryHandler<GetParserQuery, ParserResponse?> handler,
		CancellationToken ct
	)
	{
		GetParserQuery query = new(Id: id);
		ParserResponse? parser = await handler.Handle(query, ct);
		return parser.NotFoundOrOk($"Парсер с id: {id} не найден.");
	}

	/// <summary>
	/// Перманентно запустить парсер.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="handler">Обработчик команды перманентного запуска парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPatch("{id:guid}/permantly-start")]
	public async Task<Envelope> PermantlyStartParser(
		[FromRoute(Name = "id")] Guid id,
		[FromServices] ICommandHandler<PermantlyStartParsingCommand, SubscribedParser> handler,
		CancellationToken ct
	)
	{
		PermantlyStartParsingCommand command = new(Id: id);
		Result<SubscribedParser> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserResponse.Create);
	}

	/// <summary>
	/// Обновить ссылки парсера.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="request">Запрос с информацией для обновления ссылок парсера.</param>
	/// <param name="handler">Обработчик команды обновления ссылок парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPut("{id:guid}/links")]
	public async Task<Envelope> UpdateParserLinks(
		[FromRoute(Name = "id")] Guid id,
		[FromBody] UpdateParserLinksRequest request,
		[FromServices] ICommandHandler<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>> handler,
		CancellationToken ct
	)
	{
		IEnumerable<UpdateParserLinksCommandInfo> updateInfos = request.Links.Select(
			l => new UpdateParserLinksCommandInfo(l.LinkId, l.Activity, l.Name, l.Url)
		);
		UpdateParserLinksCommand command = new(id, updateInfos);
		Result<IEnumerable<SubscribedParserLink>> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(r => r.Select(ParserLinkResponse.Create));
	}

	/// <summary>
	/// Перманентно запустить множество парсеров.
	/// </summary>
	/// <param name="ids">Идентификаторы парсеров.</param>
	/// <param name="handler">Обработчик команды перманентного запуска множества парсеров.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPatch("permantly-start")]
	public async Task<Envelope> PermantlyStartManyParsers(
		[FromQuery(Name = "ids")] IEnumerable<Guid> ids,
		[FromServices] ICommandHandler<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>> handler,
		CancellationToken ct
	)
	{
		PermantlyStartManyParsingCommand command = new(Identifiers: ids);
		Result<IEnumerable<SubscribedParser>> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(r => r.Select(ParserResponse.Create));
	}

	/// <summary>
	/// Перманентно отключить множество парсеров.
	/// </summary>
	/// <param name="ids">Идентификаторы парсеров.</param>
	/// <param name="handler">Обработчик команды перманентного отключения множества парсеров.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPatch("permantly-disable")]
	public async Task<Envelope> PermantlyDisableManyParsers(
		[FromQuery(Name = "ids")] IEnumerable<Guid> ids,
		[FromServices] ICommandHandler<PermantlyDisableManyParsingCommand, IEnumerable<SubscribedParser>> handler,
		CancellationToken ct
	)
	{
		PermantlyDisableManyParsingCommand command = new(Identifiers: ids);
		Result<IEnumerable<SubscribedParser>> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(r => r.Select(ParserResponse.Create));
	}

	/// <summary>
	/// Перманентно отключить парсер.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="handler">Обработчик команды перманентного отключения парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPatch("{id:guid}/permantly-disable")]
	public async Task<Envelope> PermantlyDisableParser(
		[FromRoute(Name = "id")] Guid id,
		[FromServices] ICommandHandler<PermantlyDisableParsingCommand, SubscribedParser> handler,
		CancellationToken ct
	)
	{
		PermantlyDisableParsingCommand command = new(Id: id);
		Result<SubscribedParser> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserResponse.Create);
	}

	/// <summary>
	/// Отключить парсер.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="handler">Обработчик команды отключения парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPost("{id:guid}/disabled")]
	public async Task<Envelope> DisableParser(
		[FromRoute(Name = "id")] Guid id,
		[FromServices] ICommandHandler<DisableParserCommand, SubscribedParser> handler,
		CancellationToken ct
	)
	{
		DisableParserCommand command = new(Id: id);
		Result<SubscribedParser> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserResponse.Create);
	}

	/// <summary>
	/// Изменить активность ссылки парсера.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="linkId">Идентификатор ссылки парсера.</param>
	/// <param name="value">Новое значение активности.</param>
	/// <param name="handler">Обработчик команды изменения активности ссылки парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPatch("{id:guid}/links/{linkId:guid}/activity")]
	public async Task<Envelope> ChangeParserActivity(
		[FromRoute(Name = "id")] Guid id,
		[FromRoute(Name = "linkId")] Guid linkId,
		[FromQuery(Name = "value")] bool value,
		[FromServices] ICommandHandler<ChangeLinkActivityCommand, SubscribedParserLink> handler,
		CancellationToken ct
	)
	{
		ChangeLinkActivityCommand command = new(id, linkId, value);
		Result<SubscribedParserLink> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(ParserLinkResponse.Create);
	}

	/// <summary>
	/// Добавить ссылки к парсеру.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <param name="request">Запрос с данными для добавления ссылок.</param>
	/// <param name="handler">Обработчик команды добавления ссылок к парсеру.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Обертка с результатом операции.</returns>
	[VerifyToken]
	[ParserManagementPermission]
	[HttpPost("{id:guid}/links")]
	public async Task<Envelope> AddLinksToParser(
		[FromRoute(Name = "id")] Guid id,
		[FromBody] AddLinksToParserRequest request,
		[FromServices] ICommandHandler<AddParserLinkCommand, IEnumerable<SubscribedParserLink>> handler,
		CancellationToken ct
	)
	{
		AddParserLinkCommand command = new(id, request.Links.Select(l => new AddParserLinkCommandArg(l.Url, l.Name)));
		Result<IEnumerable<SubscribedParserLink>> result = await handler.Execute(command, ct);
		return result.AsTypedEnvelope(r => r.Select(ParserLinkResponse.Create));
	}
}
