namespace StudentManagement.Infrastructure.Configurations;

public class JwtSettings
{
    #region Constants

    public const string SectionName = "Jwt";

    #endregion

    #region Properties

    public string Key { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public int AccessTokenExpiryMinutes { get; init; } = 60;

    #endregion
}
