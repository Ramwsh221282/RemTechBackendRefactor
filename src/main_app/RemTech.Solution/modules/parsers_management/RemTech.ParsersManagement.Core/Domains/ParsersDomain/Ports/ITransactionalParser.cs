using RemTech.ParsersManagement.Core.Common.Database;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;

public interface ITransactionalParser : IParser, ITransactional { }
