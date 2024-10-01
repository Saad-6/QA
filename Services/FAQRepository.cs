using Nop.Data;
using Nop.Data.DataProviders;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;

namespace Nop.Plugin.F.A.Q.Services;
public class FAQRepository : IFAQRepository
{
    private readonly MsSqlNopDataProvider _dataContext;
    private readonly IRepository<FAQEntity> _repository;
    public FAQRepository(IRepository<FAQEntity> repository,MsSqlNopDataProvider dataContext)
    {
        _dataContext = dataContext;
        _repository = repository;
    }
    public async Task <IList<FAQEntity>> GetFAQAsync(FAQType type = FAQType.All, int pageSize = 0, int startIndex = 0, SortExpression sortExpression = SortExpression.LastModified, int productId = 0, string productName = "",Visibility visibility = Visibility.Undefined)
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
        return await query.ToListAsync();
    }
    public async Task<FAQCounts> GetCountsAsync(int productId = 0, string productName = "", Visibility visibility = Visibility.Undefined)
    {
        var query = _dataContext.GetTable<FAQEntity>().AsQueryable();

        if (visibility != Visibility.Undefined)
        {
            query = visibility switch
            {
                Visibility.Visible => query.Where(m => m.Visibility == true),
                Visibility.Hidden => query.Where(m => m.Visibility == false),
                _ => query,
            };
        }

        if (productId > 0)
        {
            query = query.Where(m => m.ProductId == productId);
        }

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(m => m.ProductName.Contains(productName));
        }

        var counts = await query
            .GroupBy(f => new
            {
                IsAnswered = f.Answer != null,
                // If you need to include the visibility here, you can group by it as well
            })
            .Select(g => new
            {
                IsAnswered = g.Key.IsAnswered,
                Count = g.Count()
            })
            .ToListAsync();

        // Aggregate counts based on the grouping
        var answeredCount = counts.FirstOrDefault(c => c.IsAnswered)?.Count ?? 0;
        var unansweredCount = counts.FirstOrDefault(c => !c.IsAnswered)?.Count ?? 0;
        var totalCount = counts.Sum(c => c.Count);

        return new FAQCounts
        {
            TotalCount = totalCount,
            AnsweredCount = answeredCount,
            UnansweredCount = unansweredCount
        };
    }

    public async Task<bool> CrudAsync(FAQEntity entity, Operation operation)
    {
        if (entity == null || entity.ProductId == 0 || string.IsNullOrEmpty(entity.Question))
        {
            return false;
        }
        switch (operation)
        {
            case Operation.Create:
              await  _repository.InsertAsync(entity);
                break;
            case Operation.Update:
               await  _repository.UpdateAsync(entity);
                break;
            case Operation.Delete:
               await _repository.DeleteAsync(entity);
                break;
            default:
                return false;
        }
        return true;
    }
    public async  Task<int> GetCountAsync(FAQType type = FAQType.All, int productId = 0,string productName = "", Visibility visibility = Visibility.Undefined)
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
        return await query.CountAsync();
    }
    public async Task<FAQEntity> LoadByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
     
    }
    public async Task <IList<FAQEntity>> LoadForProductAsync(int id, bool visibility)
    {
        var query = _dataContext.GetTable<FAQEntity>().AsQueryable();
        query = query.Where(m => m.Answer != null);
        query = query.Where(m=>m.Visibility == visibility);
        return await query.Where(m => m.ProductId == id).ToListAsync<FAQEntity>();
    }
}
