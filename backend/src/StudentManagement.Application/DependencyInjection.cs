using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Application.Services;

namespace StudentManagement.Application;

public static class DependencyInjection
{
    #region Public Methods

    public static IServiceCollection AddApplicationServices(this IServiceCollection objServices)
    {
        Assembly objAssembly = Assembly.GetExecutingAssembly();

        objServices.AddAutoMapper(objAssembly);
        objServices.AddValidatorsFromAssembly(objAssembly);

        objServices.AddScoped<IStudentService, StudentService>();
        objServices.AddScoped<IAuthService, AuthService>();

        return objServices;
    }

    #endregion
}
