﻿using RemTech.Core.Shared.Database;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;

public interface ITransactionalParser : IParser, ITransactional { }
