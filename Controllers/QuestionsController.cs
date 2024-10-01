using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Models;
using Nop.Plugin.F.A.Q.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.F.A.Q.Controllers
{
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin]
    public class QuestionsController : BasePluginController
    {
        private readonly IFAQRepository _repo;
        private readonly ISettingService _settings;

        public QuestionsController(IFAQRepository repo, ISettingService settings)
        {
            _repo = repo;
            _settings = settings;
        }
        public IActionResult Configure()
        {
            var settings = _settings.LoadSetting<FAQSettings>();
            ViewBag.WidgetZones = Utilities.GetAvailableWidgetZones();
            return View("~/Plugins/F.A.Q/Views/Configure.cshtml", settings);
        }

        [HttpPost, ActionName("Configure")]
        public IActionResult ChangeSettings(FAQSettings settings)
        {
            if (!ModelState.IsValid)
                return Configure();

            _settings.SaveSetting(settings);
            ViewBag.Success = true;
            return Configure();
        }

        public async Task<IActionResult> Index(int page = 1, int size = 10)
        {
            var settings = await _settings.LoadSettingAsync<FAQSettings>();
            var jointModel = await BuildJointQuestionModelAsync(page, size);
            jointModel.FAQSettings = settings;

            ViewBag.pageSize = size;
            ViewBag.page = page;
            return View("~/Plugins/F.A.Q/Views/AdminIndex.cshtml", jointModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAnswer(int faqId, string view, string answer)
        {
            var faq = await _repo.LoadByIdAsync(faqId);
            if (faq == null || string.IsNullOrEmpty(view) || string.IsNullOrEmpty(answer))
                return Json(new { success = false, message = "Invalid input." });

            faq.Answer = answer;
            faq.LastModified = DateTime.Now;
            await _repo.CrudAsync(faq, Operation.Update);
            return await ReturnPartialView(view);
        }

        public async Task<IActionResult> ToggleVisibility(int faqId, string view, bool visibility)
        {
            var faq = await _repo.LoadByIdAsync(faqId);
            if (faq == null)
                return Json(new { success = false, message = "No FAQ Found." });

            faq.Visibility = !visibility;
            await _repo.CrudAsync(faq, Operation.Update);
            return await ReturnPartialView(view);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string view)
        {
            var faq = await _repo.LoadByIdAsync(id);
            if (faq == null)
                return Json(new { success = false, message = "No FAQ Found." });

            await _repo.CrudAsync(faq, Operation.Delete);
            return await ReturnPartialView(view);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnPartialView(string view, int pageNumber = 1, int pageSize = 10, string productName = "")
        {
            var type = Utilities.GetType(view);
            var count = await _repo.GetCountAsync(type, productName: productName);

            // show all search results on a single page
            if (!string.IsNullOrEmpty(productName))
            {
                pageNumber = 1;
                pageSize = count;
            }

            var startIndex = (pageSize * (pageNumber - 1));
            pageSize = pageSize == 0 || pageSize > count ? count : pageSize;

            var faqs = await _repo.GetFAQAsync(type, pageSize, startIndex, SortExpression.LastModified, productName: productName);
            var list = new PaginatedList<FAQEntity>(faqs, count, pageNumber, pageSize);

            ViewBag.pageSize = pageSize;
            return PartialView($"~/Plugins/F.A.Q/Views/{view}.cshtml", list);
        }

        private async Task<JointQuestionModel<FAQEntity>> BuildJointQuestionModelAsync(int page, int size)
        {
            var counts = await _repo.GetCountsAsync();

            var answeredCount = counts.AnsweredCount;
            var unansweredCount = counts.UnansweredCount;

            var allFaqs = await _repo.GetFAQAsync();
            var answered = allFaqs
                .Where(m => m.Answer != null)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            var unAnswered = allFaqs
                .Where(m => m.Answer == null)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return new JointQuestionModel<FAQEntity>
            {
                Answered = new PaginatedList<FAQEntity>(answered, answeredCount, page, size),
                Unanswered = new PaginatedList<FAQEntity>(unAnswered, unansweredCount, page, size)
            };
        }
    }
}