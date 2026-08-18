using KadreeBank.Application.Accounts;
using KadreeBank.Application.Reports;
using Microsoft.Extensions.DependencyInjection;

namespace KadreeBank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}
