using Nop.Core;

namespace Nop.Plugin.F.A.Q.Domain;
public class FAQEntity : BaseEntity
{
    public string Question { get; set; }
    public string? Answer { get; set; }
    public bool Visibility { get; set; }
    public int? Upvotes { get; set; }
    public int ProductId { get; set; }
    public string ProductName {  get; set; }
    public DateTime AskedDate { get; set; }
    public DateTime LastModified {  get; set; }
    public string UserName { get; set; }
    public string AnsweredBy { get; set; }

    public FAQEntity()
    {
        Visibility = true;
    }
}
