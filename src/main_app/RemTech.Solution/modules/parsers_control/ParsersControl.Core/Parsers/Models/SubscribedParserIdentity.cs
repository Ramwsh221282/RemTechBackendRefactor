using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

/// <summary>
/// Идентификатор подписанного парсера.
/// </summary>
public sealed record SubscribedParserIdentity
{
	/// <summary>
	/// Максимальная длина идентификатора домена сервиса парсера.
	/// </summary>
	public const int MAX_DOMAIN_NAME_LENGTH = 128;

	/// <summary>
	/// Максимальная длина типа обрабатываемых данных парсером.
	/// </summary>
	public const int MAX_SERVICE_TYPE_LENGTH = 128;

	private SubscribedParserIdentity(string domainName, string serviceType)
	{
		DomainName = domainName;
		ServiceType = serviceType;
	}

	/// <summary>
	///  Идентификатор домена сервиса парсера.
	/// </summary>
	public string DomainName { get; private init; }

	/// <summary>
	/// Тип обрабатываемых данных парсером.
	/// </summary>
	public string ServiceType { get; private init; }

	/// <summary>
	/// Создаёт идентификатор подписанного парсера.
	/// </summary>
	/// <param name="domainName">Название домена сервиса.</param>
	/// <param name="serviceType">Тип объявлений (техника или запчасти) сервиса.</param>
	/// <returns>Результат создания идентификатора подписанного парсера.</returns>
	public static Result<SubscribedParserIdentity> Create(string domainName, string serviceType)
	{
		List<string> errors = [];
		if (string.IsNullOrWhiteSpace(domainName))
			errors.Add(Error.NotSet("Идентификатор домена сервиса парсера"));
		if (domainName.Length > MAX_DOMAIN_NAME_LENGTH)
			errors.Add(Error.GreaterThan("Идентификатор домена сервиса парсера", MAX_DOMAIN_NAME_LENGTH));
		if (string.IsNullOrWhiteSpace(serviceType))
			errors.Add(Error.NotSet("Тип обрабатываемых данных парсером"));
		if (serviceType.Length > MAX_SERVICE_TYPE_LENGTH)
			errors.Add(Error.GreaterThan("Тип обрабатываемых данных парсером", MAX_SERVICE_TYPE_LENGTH));
		return errors.Count > 0 ? Error.Validation(errors) : new SubscribedParserIdentity(domainName, serviceType);
	}
}
