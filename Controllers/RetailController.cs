using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;
using Nop.Plugin.F.A.Q.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;

namespace Nop.Plugin.F.A.Q.Controllers;
public class RetailController : Controller
{
    private readonly IFAQRepository _repo;
    private readonly IProductService _product;
    private readonly ICustomerService _customerService;
    private readonly ISettingService _settings;
    public RetailController(IFAQRepository repo, IProductService service, ISettingService setting, ICustomerService customerService)
    {
        _repo = repo;
        _product = service;
        _settings = setting;
        _customerService = customerService;
    }
  
    [HttpPost]
    public async Task<IActionResult> AddQuestion(string question , int productId)
    {
        if (string.IsNullOrEmpty(question) || productId == 0)
        {
            return BadRequest();
        }

        var faq = await BuildFAQ(question, productId);

        await _repo.CrudAsync(faq,Operation.Create);

        return Ok(new { message = "Question added successfully" });
    }

    private async Task<FAQEntity> BuildFAQ(string question, int productId) 
    {
        var settings = await _settings.LoadSettingAsync<FAQSettings>();
        var customer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();
        var product = await _product.GetProductByIdAsync(productId);

        return  new FAQEntity {
            Question = question,
            ProductId = product.Id,
            ProductName = product.Name,
            AskedDate = DateTime.Now,
            LastModified = DateTime.Now,
            Visibility = true,
            UserName = customer?.FirstName ?? "Anonymous",
            Upvotes = 0,
            AnsweredBy = settings.AnsweredBy ?? "Admin",
             };
    }
    // Does the same job as ProductViewComponent's Invoke method , the only reason for using it is for pagination ( which i am currently not sure if we can do with a ViewComponent? )
    [HttpPost]
    public async Task<IActionResult> FAQWidget(int productId, int page = 1, int size = 5)
    {

        var pageIndex = page - 1;
        var startIndex = size * pageIndex;

        var settings = await _settings.LoadSettingAsync<FAQSettings>();
        var faqCount = await _repo.GetCountAsync(FAQType.Answered, productId, visibility: Visibility.Visible);
        var faqs = await _repo.GetFAQAsync(FAQType.Answered, size, startIndex, SortExpression.LastModified, productId, visibility: Visibility.Visible);
        var customer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();

        var faqRetailViewList = Utilities.BuildRetailViewModel(faqs);
        var paginatedList = new PaginatedList<FAQRetail>(faqRetailViewList, faqCount, page, size);

        var retailViewModel = new FAQRetailViewModel()
        {
            PaginatedList = paginatedList,
            CurrentSettings = new()
            {
                AnsweredBy = settings.AnsweredBy,
                AllowAnoymourUsers = settings.AllowAnonymousUsersToAskFAQs,
                UserLoggedIn = !string.IsNullOrEmpty(customer.Username),
                ProductId = productId
            }
        };

        return View("~/Plugins/F.A.Q/Views/_FAQWidget.cshtml", retailViewModel);
    }

}




