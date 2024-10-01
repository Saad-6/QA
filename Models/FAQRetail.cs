namespace Nop.Plugin.F.A.Q.Models;
public class FAQRetail
{
    public string ProductName { get; set; }
    public int ProductId { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public string AskedBy { get; set; }
    public string AskedTime { get; set; }
    public string AnsweredBy {get; set; }

}
public class FAQRetailViewModel
{
    public CurrentSettings CurrentSettings { get; set; }
    public PaginatedList<FAQRetail> PaginatedList { get; set; }
    public FAQRetailViewModel()
    {
        CurrentSettings = new CurrentSettings()
        {
           AnsweredBy = "NopTeam",
           AllowAnoymourUsers = true,

        };

    }
}
public class CurrentSettings
{
    public bool AllowAnoymourUsers { get; set; }
    public bool UserLoggedIn { get; set; }
    public string AnsweredBy { get; set; }
    public int ProductId { get; set; }
}