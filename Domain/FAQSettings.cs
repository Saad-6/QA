using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;
using Nop.Core.Domain.Cms;
using Nop.Web.Areas.Admin.Models.Cms;
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
