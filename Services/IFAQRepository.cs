using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;

namespace Nop.Plugin.F.A.Q.Services;
public interface IFAQRepository
{
    IList<FAQEntity> GetFAQ(FAQType type = FAQType.All, int pageSize = 0, int startIndex = 0, SortExpression sortExpression = SortExpression.QuestionAsc, int productId = 0, string productName = "",Visibility visibility = Visibility.Undefined);
    IList<FAQEntity> LoadForProduct(int id,bool visibility);
    int GetCount(FAQType type = FAQType.All, int productId = 0,string productName = "", Visibility visibility = Visibility.Undefined);
    bool Crud(FAQEntity entity, Operation operation);
    FAQEntity LoadById(int id);
}
