namespace Nop.Plugin.F.A.Q.Models;
public class PaginatedList<T>
{
    public IList<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    public PaginatedList(IList<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }
    public bool ShowPages => TotalPages > 1;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}
