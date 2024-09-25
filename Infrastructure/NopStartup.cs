using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.F.A.Q.Domain;
using Nop.Plugin.F.A.Q.Services;


namespace Nop.Plugin.F.A.Q.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => 3000;

    public void Configure(IApplicationBuilder application)
    {

    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRepository<FAQEntity>, EntityRepository<FAQEntity>>();
        services.AddScoped<IRepository<LocaleStringResource>, EntityRepository<LocaleStringResource>>();
        services.AddScoped<IFAQRepository,FAQRepository>();
    }
}
