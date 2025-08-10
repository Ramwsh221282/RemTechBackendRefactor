namespace Parsing.Avito.Common.PaginationBar;

public interface IAvitoPaginationBarElement
{
    IEnumerable<string> Iterate(string originUrl);
}
