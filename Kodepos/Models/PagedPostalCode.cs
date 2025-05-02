namespace Kodepos.Models;
public record PagedPostalCode
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public List<PostalCode>? Items { get; set; }
}