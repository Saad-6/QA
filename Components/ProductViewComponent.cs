using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;
using Nop.Plugin.F.A.Q.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.F.A.Q.Components;
public class ProductViewComponent : NopViewComponent
{
    private readonly IProductService _productService;
    private readonly IFAQRepository _repo;
    private readonly ISettingService _settings;
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    public ProductViewComponent(IProductService productService, IFAQRepository repo,ISettingService setting,ICustomerService customerService,IWorkContext workContext)
    {
        _productService = productService;
        _repo = repo;
        _settings = setting;
        _customerService = customerService;
        _workContext = workContext;
    }
    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData , int pageNumber = 1,int pageSize = 5)
    {
        if (additionalData == null)
            return Content("");

        var productId = GetProductId(additionalData);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return Content("");

        var retailViewModel = await BuildFAQRetailViewModelAsync(productId, pageNumber, pageSize);
        ViewBag.WidgetZone = widgetZone;
        return View("~/Plugins/F.A.Q/Views/_FAQWidget.cshtml", retailViewModel);
    }
    private int GetProductId(object data)
    {
        if(data is ProductReviewModel)
        {
            return ((ProductReviewsModel)data).ProductId;
        }
        if(data is ProductDetailsModel)
        {
        return ((ProductDetailsModel)data).Id;
        }
        if(data is int)
        {
            return (int)data;
        }
        return 0;
    }
    public async Task<FAQRetailViewModel> BuildFAQRetailViewModelAsync(int productId, int pageNumber, int pageSize)
    {
        var pageIndex = pageNumber - 1;
        var startIndex = pageSize * pageIndex;

        var settings = await _settings.LoadSettingAsync<FAQSettings>();
        var faqCount =  await _repo.GetCountAsync(FAQType.Answered, productId, visibility: Visibility.Visible);
        var faqs = await _repo.GetFAQAsync(FAQType.Answered, pageSize, startIndex, SortExpression.LastModified, productId, visibility: Visibility.Visible);
        var customer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();
        
        var faqRetailViewList = Utilities.BuildRetailViewModel(faqs);
        var paginatedList = new PaginatedList<FAQRetail>(faqRetailViewList, faqCount, pageNumber, pageSize);

        return new FAQRetailViewModel
        {
            PaginatedList = paginatedList,
            CurrentSettings = new ()
            {
                AnsweredBy = settings.AnsweredBy,
                AllowAnoymourUsers = settings.AllowAnonymousUsersToAskFAQs,
                UserLoggedIn = !string.IsNullOrEmpty(customer.Username),
                ProductId = productId
            }
        };
    }
}
