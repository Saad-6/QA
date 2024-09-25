using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;
using Nop.Plugin.F.A.Q.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
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
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData , int pageNumber = 1,int pageSize = 5)
    {
        int productId = 0;
        if (additionalData == null)
            return Content("");
        if(additionalData is ProductReviewsModel)
        {
            productId = ((ProductReviewsModel)additionalData).ProductId;
        }else if( additionalData is ProductDetailsModel)
        {
         productId = ((ProductDetailsModel)additionalData).Id;
        }
        var product = _productService.GetProductByIdAsync(productId);
        if (product == null || product.IsFaulted)
            return Content("");
        var pageIndex = pageNumber - 1;
        var startIndex = (pageSize * pageIndex);
        var settings = _settings.LoadSetting<FAQSettings>();
        var customer = EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();
        var count = _repo.GetCount(FAQType.Answered,productId,visibility:Visibility.Visible);
        var faqs = _repo.GetFAQ(FAQType.Answered, pageSize, startIndex, SortExpression.LastModified, productId, visibility: Visibility.Visible);
        var faqRetailViewList = Utilities.MapToViewModel(faqs);
        var paginatedList = new PaginatedList<FAQRetail>(faqRetailViewList, count, pageNumber, pageSize);
        var retailViewModel = new FAQRetailViewModel();
        retailViewModel.PaginatedList = paginatedList;
        retailViewModel.CurrentSettings.AnsweredBy = settings.AnsweredBy;
        retailViewModel.CurrentSettings.AllowAnoymourUsers = settings.AllowAnonymousUsersToAskFAQs;
        retailViewModel.CurrentSettings.UserLoggedIn = customer.Result.Username != null;
        retailViewModel.CurrentSettings.ProductId = productId;
        ViewBag.ProductName = product.Result.Name;

        return View("~/Plugins/F.A.Q/Views/_FAQWidget.cshtml", retailViewModel);

    }
}
