// SpendWise.Infrastructure/Authentication/JwtSettings.cs
using System.ComponentModel.DataAnnotations;

namespace SpendWise.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    [MinLength(32)]
    public string Key { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiryMinutes { get; init; } = 60;
}
