using Nop.Plugin.F.A.Q.Domain;


namespace Nop.Plugin.F.A.Q.Models;
public class JointQuestionModel<T> where T : class
{
    public PaginatedList<T> All { get; set; }
    public PaginatedList<T> Answered { get; set; }
    public PaginatedList<T> Unanswered { get; set; }
    public FAQSettings FAQSettings { get; set; }
}
public class QuestionsViewModel
{
    public int Id { get; set; }
    public string Question { get; set; }
    public bool Visibility { get; set; }
    public string Answer { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public DateTime LastModified { get; set; }
}
