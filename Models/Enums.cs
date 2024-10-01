namespace Nop.Plugin.F.A.Q.Models;

public enum FAQType
{
    All,
    Unanswered,
    Answered

}
public enum SortExpression
{
    LastModified,
    CreatedDate,
    QuestionAsc,
    QuestionDesc,
    UpvotesAsc,
    UpvotesDesc
}
public enum Operation
{
    Create,
    Update,
    Delete,
}
public enum Visibility
{
    Visible,
    Hidden,
    Undefined
}