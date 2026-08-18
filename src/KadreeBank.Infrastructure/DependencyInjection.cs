using KadreeBank.Application.Abstractions;
using KadreeBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KadreeBank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("KadreeBank")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:KadreeBank.");

        services.AddDbContext<KadreeBankDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IReportQuery, ReportQuery>();
        return services;
    }
}
