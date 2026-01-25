using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Features.UpdateParserLinks;

public sealed class ParserLinkUpdater
{
	public SubscribedParserLinkId Id { get; }
	public bool? Activity { get; }
	public string? Name { get; }
	public string? Url { get; }

	private ParserLinkUpdater(SubscribedParserLinkId id, bool? activity, string? name, string? url)
	{
		Id = id;
		Activity = activity;
		Name = name;
		Url = url;
	}

	public bool UpdateBelongsTo(SubscribedParserLink link) => Id == link.Id;

	public Result Update(SubscribedParserLink link)
	{
		Result<SubscribedParserLink> editing = link.Edit(Name, Url);
		if (editing.IsFailure)
			return Result.Failure(editing.Error);
		if (Activity.HasValue)
		{
			if (Activity.Value)
				link.Enable();
			else
				link.Disable();
		}

		return editing;
	}

	public static Result<ParserLinkUpdater> Create(Guid id, bool? activity, string? name, string? url)
	{
		Result<SubscribedParserLinkId> idResult = SubscribedParserLinkId.From(id);
		return idResult.IsFailure ? idResult.Error : new ParserLinkUpdater(idResult.Value, activity, name, url);
	}
}
