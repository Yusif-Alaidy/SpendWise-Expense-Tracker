namespace SpendWise.Application.Auth;

public record AuthResponse(string Token, DateTime ExpiresAt, string DisplayName, string Email);
