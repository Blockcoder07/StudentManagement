using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Infrastructure.Configurations;

namespace StudentManagement.API.Extensions;

public static class AuthenticationServiceExtensions
{
    #region Public Methods

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection objServices,
        IConfiguration objConfiguration)
    {
        JwtSettings objJwt = objConfiguration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt section is missing from configuration.");

        byte[] lstKeyBytes = ResolveKey(objJwt.Key);

        objServices.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.RequireHttpsMetadata = false;
            opts.SaveToken = true;
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrWhiteSpace(objJwt.Issuer),
                ValidateAudience = !string.IsNullOrWhiteSpace(objJwt.Audience),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = objJwt.Issuer,
                ValidAudience = objJwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(lstKeyBytes),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

        objServices.AddAuthorization();

        return objServices;
    }

    #endregion

    #region Private Methods

    private static byte[] ResolveKey(string stKey)
    {
        if (string.IsNullOrWhiteSpace(stKey))
        {
            throw new InvalidOperationException("JWT signing key has not been configured.");
        }

        try
        {
            byte[] lstBytes = Convert.FromBase64String(stKey);
            if (lstBytes.Length >= 32)
            {
                return lstBytes;
            }
        }
        catch (FormatException)
        {
        }

        byte[] lstRaw = Encoding.UTF8.GetBytes(stKey);
        if (lstRaw.Length < 32)
        {
            Array.Resize(ref lstRaw, 32);
        }

        return lstRaw;
    }

    #endregion
}
