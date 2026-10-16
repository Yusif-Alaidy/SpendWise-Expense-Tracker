// SpendWise.Application/Common/Interfaces/IJwtTokenGenerator.cs
namespace SpendWise.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(Guid userId, string email, string displayName);
}