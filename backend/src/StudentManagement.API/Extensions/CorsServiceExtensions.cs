namespace StudentManagement.API.Extensions;

public static class CorsServiceExtensions
{
    #region Constants

    public const string StAngularClientPolicy = "AngularClientPolicy";

    #endregion

    #region Public Methods

    public static IServiceCollection AddAngularCors(this IServiceCollection objServices, IConfiguration objConfiguration)
    {
        string[] lstOrigins = objConfiguration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                              ?? new[] { "http://localhost:4200" };

        objServices.AddCors(opts =>
        {
            opts.AddPolicy(StAngularClientPolicy, policy => policy
                .WithOrigins(lstOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });

        return objServices;
    }

    #endregion
}
