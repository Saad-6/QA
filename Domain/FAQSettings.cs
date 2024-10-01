using Nop.Core.Configuration;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.F.A.Q.Domain;
public class FAQSettings : ISettings
{

    public bool AllowAnonymousUsersToAskFAQs { get; set; }
    public string AnsweredBy { get; set; }
    public string ActiveWidgetZone { get; set; }
    public FAQSettings()
    {
        AllowAnonymousUsersToAskFAQs = true;
        ActiveWidgetZone = PublicWidgetZones.ProductDetailsBottom;
    }
}
