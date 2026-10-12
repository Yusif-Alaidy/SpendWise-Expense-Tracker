// SpendWise.Infrastructure/Identity/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace SpendWise.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}