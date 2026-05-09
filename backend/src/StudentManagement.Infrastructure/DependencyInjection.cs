using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Application.Interfaces.Authentication;
using StudentManagement.Application.Interfaces.Repositories;
using StudentManagement.Infrastructure.Authentication;
using StudentManagement.Infrastructure.Configurations;
using StudentManagement.Infrastructure.Persistence;
using StudentManagement.Infrastructure.Persistence.Repositories;
using StudentManagement.Infrastructure.Persistence.Seed;

namespace StudentManagement.Infrastructure;

public static class DependencyInjection
{
    #region Public Methods

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection objServices,
        IConfiguration objConfiguration)
    {
        objServices.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                objConfiguration.GetConnectionString("Default"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        objServices.AddScoped<IStudentRepository, StudentRepository>();
        objServices.AddScoped<IUserRepository, UserRepository>();

        objServices.Configure<JwtSettings>(objConfiguration.GetSection(JwtSettings.SectionName));

        objServices.AddSingleton<IPasswordHasher, PasswordHasher>();
        objServices.AddScoped<IJwtTokenService, JwtTokenService>();

        objServices.AddScoped<DatabaseSeeder>();

        return objServices;
    }

    #endregion
}
