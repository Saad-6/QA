using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;

namespace Nop.Plugin.F.A.Q.Services;
public interface IFAQRepository
{
    Task<IList<FAQEntity>> GetFAQAsync(FAQType type = FAQType.All, int pageSize = 0, int startIndex = 0, SortExpression sortExpression = SortExpression.QuestionAsc, int productId = 0, string productName = "",Visibility visibility = Visibility.Undefined);
    Task<IList<FAQEntity>> LoadForProductAsync(int id,bool visibility);
    Task<int> GetCountAsync(FAQType type = FAQType.All, int productId = 0,string productName = "", Visibility visibility = Visibility.Undefined);
    Task<bool> CrudAsync(FAQEntity entity, Operation operation);
    Task<FAQCounts> GetCountsAsync(int productId = 0, string productName = "", Visibility visibility = Visibility.Undefined);
    Task<FAQEntity> LoadByIdAsync(int id);
}
