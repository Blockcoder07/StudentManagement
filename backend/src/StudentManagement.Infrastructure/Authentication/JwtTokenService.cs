using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Application.Interfaces.Authentication;
using StudentManagement.Domain.Entities;
using StudentManagement.Infrastructure.Configurations;

namespace StudentManagement.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    #region Fields

    private readonly JwtSettings _objSettings;

    #endregion

    #region Constructor

    public JwtTokenService(IOptions<JwtSettings> objSettings)
    {
        _objSettings = objSettings.Value;
    }

    #endregion

    #region Public Methods

    public (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser objUser)
    {
        SymmetricSecurityKey objKey = new(Convert.FromBase64String(NormaliseKey(_objSettings.Key)));
        SigningCredentials objCredentials = new(objKey, SecurityAlgorithms.HmacSha256);
        DateTime dtExpiresAt = DateTime.UtcNow.AddMinutes(_objSettings.AccessTokenExpiryMinutes);

        List<Claim> lstClaims = new()
        {
            new(JwtRegisteredClaimNames.Sub, objUser.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, objUser.Username),
            new(JwtRegisteredClaimNames.Email, objUser.Email),
            new(ClaimTypes.NameIdentifier, objUser.Id.ToString()),
            new(ClaimTypes.Name, objUser.Username),
            new(ClaimTypes.Email, objUser.Email),
            new(ClaimTypes.Role, objUser.Role.ToString())
        };

        JwtSecurityToken objToken = new(
            issuer: _objSettings.Issuer,
            audience: _objSettings.Audience,
            claims: lstClaims,
            notBefore: DateTime.UtcNow,
            expires: dtExpiresAt,
            signingCredentials: objCredentials);

        string stTokenString = new JwtSecurityTokenHandler().WriteToken(objToken);
        return (stTokenString, dtExpiresAt);
    }

    #endregion

    #region Private Methods

    private static string NormaliseKey(string stKey)
    {
        if (string.IsNullOrWhiteSpace(stKey))
        {
            throw new InvalidOperationException("JWT signing key has not been configured.");
        }

        try
        {
            byte[] lstRaw = Convert.FromBase64String(stKey);
            if (lstRaw.Length >= 32)
            {
                return stKey;
            }
        }
        catch (FormatException)
        {
        }

        byte[] lstBytes = Encoding.UTF8.GetBytes(stKey);
        if (lstBytes.Length < 32)
        {
            Array.Resize(ref lstBytes, 32);
        }

        return Convert.ToBase64String(lstBytes);
    }

    #endregion
}
