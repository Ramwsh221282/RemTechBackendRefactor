using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed record SubscribedParserIdentity
{
	public const int MaxDomainNameLength = 128;
	public const int MaxServiceTypeLength = 128;

	public string DomainName { get; private init; }
	public string ServiceType { get; private init; }

	private SubscribedParserIdentity(string domainName, string serviceType)
	{
		DomainName = domainName;
		ServiceType = serviceType;
	}

	public static Result<SubscribedParserIdentity> Create(string domainName, string serviceType)
	{
		List<string> errors = [];
		if (string.IsNullOrWhiteSpace(domainName))
			errors.Add(Error.NotSet("Идентификатор домена сервиса парсера"));
		if (domainName.Length > MaxDomainNameLength)
			errors.Add(Error.GreaterThan("Идентификатор домена сервиса парсера", MaxDomainNameLength));
		if (string.IsNullOrWhiteSpace(serviceType))
			errors.Add(Error.NotSet("Тип обрабатываемых данных парсером"));
		if (serviceType.Length > MaxServiceTypeLength)
			errors.Add(Error.GreaterThan("Тип обрабатываемых данных парсером", MaxServiceTypeLength));
		if (errors.Count > 0)
			return Error.Validation(errors);
		return new SubscribedParserIdentity(domainName, serviceType);
	}
}
