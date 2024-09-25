using Nop.Data;
using Nop.Data.DataProviders;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;

namespace Nop.Plugin.F.A.Q.Services;
public class FAQRepository : IFAQRepository
{
    private readonly MsSqlNopDataProvider _dataContext;
    private readonly IRepository<FAQEntity> _repository;
    public FAQRepository(IRepository<FAQEntity> repository)
    {
        _dataContext = new MsSqlNopDataProvider();
        _repository = repository;
    }
    public IList<FAQEntity> GetFAQ(FAQType type = FAQType.All, int pageSize = 0, int startIndex = 0, SortExpression sortExpression = SortExpression.LastModified, int productId = 0, string productName = "",Visibility visibility = Visibility.Undefined)
    {
        var query = _dataContext.GetTable<FAQEntity>().AsQueryable();
        if (productId > 0)
        {
            query = query.Where(f => f.ProductId == productId);
        }
        query = visibility switch
        {
            Visibility.Visible => query.Where(m => m.Visibility == true),
            Visibility.Hidden => query.Where(m => m.Visibility == false),
            Visibility.Undefined =>query
        };
        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(f => f.ProductName.Contains(productName));
        }
        query = sortExpression switch 
        {
            SortExpression.LastModified=>query.OrderByDescending(f=>f.LastModified),
            SortExpression.QuestionAsc => query.OrderBy(f => f.Question),
            SortExpression.QuestionDesc => query.OrderByDescending(f => f.Question),
            SortExpression.UpvotesAsc => query.OrderBy(f => f.Upvotes),
            SortExpression.UpvotesDesc => query.OrderByDescending(f => f.Upvotes),
            _ => query
        };
        query = type switch
        {
            FAQType.All => query,
            FAQType.Unanswered => query.Where(f => string.IsNullOrEmpty(f.Answer)),
            FAQType.Answered => query.Where(f => !string.IsNullOrEmpty(f.Answer)),
            _ => query
        };
        if (pageSize > 0)
        {
            query = query.Skip(startIndex).Take(pageSize);
        }
        return query.ToList();
    }

    public bool Crud(FAQEntity entity, Operation operation)
    {
        if (entity == null || entity.ProductId == 0 || string.IsNullOrEmpty(entity.Question))
        {
            return false;
        }
        switch (operation)
        {
            case Operation.Create:
                _repository.Insert(entity);
                break;
            case Operation.Update:
                _repository.Update(entity);
                break;
            case Operation.Delete:
                _repository.Delete(entity);
                break;
            default:
                return false;
        }
        return true;
    }
    public int GetCount(FAQType type = FAQType.All, int productId = 0,string productName = "", Visibility visibility = Visibility.Undefined)
    {
        var query = _dataContext.GetTable<FAQEntity>().AsQueryable();
        if (type == FAQType.Answered)
        {
            query = query.Where(f => f.Answer != null);
        }
        else if (type == FAQType.Unanswered)
        {
            query = query.Where(f => f.Answer == null);
        }
        query = visibility switch
        {
            Visibility.Visible => query.Where(m => m.Visibility == true),
            Visibility.Hidden => query.Where(m => m.Visibility == false),
            Visibility.Undefined=>query
        };
        if (productId > 0)
        {
            query = query.Where(m => m.ProductId == productId);
        }
        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(m=>m.ProductName.Contains(productName));
        }
        return query.Count();
    }
    public FAQEntity LoadById(int id)
    {
        var query = _dataContext.GetTable<FAQEntity>().AsQueryable();
        var faq = query.Where(m => m.Id == id).FirstOrDefault();
        return faq;
    }
    public IList<FAQEntity> LoadForProduct(int id, bool visibility)
    {
        var query = _dataContext.GetTable<FAQEntity>().AsQueryable();
        query = query.Where(m => m.Answer != null);
        query = query.Where(m=>m.Visibility == visibility);
        return query.Where(m => m.ProductId == id).ToList<FAQEntity>();
    }
}
