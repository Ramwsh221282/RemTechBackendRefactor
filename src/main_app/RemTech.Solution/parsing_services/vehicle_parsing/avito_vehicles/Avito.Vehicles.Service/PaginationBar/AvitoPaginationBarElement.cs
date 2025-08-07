using System.Text;

namespace Avito.Vehicles.Service.PaginationBar;

public sealed class AvitoPaginationBarElement
{
    private readonly int[] _pages;

    public AvitoPaginationBarElement(int[] pages) => _pages = pages;

    public AvitoPaginationBarElement(AvitoPaginationBarElement origin, int page)
        : this([.. origin._pages, page]) { }

    public int Amount() => _pages.Length;

    public IEnumerable<string> Iterate(string originUrl)
    {
        int target = _pages.Min();
        int maxPage = _pages.Max();
        while (target <= maxPage)
        {
            StringBuilder sb = new(originUrl);
            sb.Append($"&p={target}");
            target++;
            yield return sb.ToString();
        }
    }
}
