using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.DataProviders;
using Nop.Plugin.F.A.Q.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Infrastructure;


namespace Nop.Plugin.F.A.Q.Domain;
public static class Utilities
{
    public static FAQType GetType(string view)
    {
        FAQType type = view switch
        {
            "_AnsweredQuestions" => FAQType.Answered,
            "_UnansweredQuestions" => FAQType.Unanswered,
            _ => FAQType.All
        };
        return type;
    }
    // No mapping required as of now since we need every attribute in the FAQ Entity in our views
    public static IList<FAQRetail> MapToViewModel(IList<FAQEntity> list)
    {
        var questionsViewModels = new List<FAQRetail>();
        foreach (var item in list)
        {
            var questionModel = new FAQRetail();
            questionModel.Question = item.Question;
            questionModel.Answer = item.Answer;
            questionModel.ProductName = item.ProductName;
            questionModel.AnsweredBy = item.AnsweredBy;
            questionModel.AskedBy = item.UserName;
            questionModel.AskedTime = TimeAgo(item.AskedDate);
            questionsViewModels.Add(questionModel);
        
        }

        return questionsViewModels;
    }
    // These will be displayed in configuration ,user can choose where to display the widget from this list
    public static IDictionary<string, string> GetAvailableWidgetZones()
    {
        var availableWidgetZones = new Dictionary<string, string>();

        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsTop, "Product Details - Top");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsBottom, "Product Details - Bottom");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsEssentialTop, "Product Details Essential - Top");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsEssentialBottom, "Product Details Essential - Bottom");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsOverviewTop, "Product Details Overview - Top");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsOverviewBottom, "Product Details Overview - Bottom");
        availableWidgetZones.Add(PublicWidgetZones.ProductReviewsPageTop, "Product Reviews Page - Top");
        availableWidgetZones.Add(PublicWidgetZones.ProductReviewsPageBottom, "Product Reviews Page - Bottom");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsAfterBreadcrumb, "Product Details - After Breadcrumb");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsAfterPictures, "Product Details - After Pictures");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsAfterVideos, "Product Details - After Videos");
        availableWidgetZones.Add(PublicWidgetZones.ProductDetailsBeforeCollateral, "Product Details - Before Collateral");

        return availableWidgetZones;
    }
    public static string TimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.Now.Subtract(dateTime);

        if (timeSpan.TotalDays < 1)
        {
            return "Today";
        }
        else if (timeSpan.TotalDays < 2)
        {
            return "Yesterday";
        }
        else if (timeSpan.TotalDays < 7)
        {
            return $"{timeSpan.Days} days ago";
        }
        else if (timeSpan.TotalDays < 30)
        {
            var weeks = (int)(timeSpan.TotalDays / 7);
            return $"{weeks} {(weeks > 1 ? "weeks" : "week")} ago";
        }
        else if (timeSpan.TotalDays < 365)
        {
            var months = (int)(timeSpan.TotalDays / 30);
            return $"{months} {(months > 1 ? "months" : "month")} ago";
        }
        else
        {
            var years = (int)(timeSpan.TotalDays / 365);
            return $"{years} {(years > 1 ? "years" : "year")} ago";
        }
    }

}
