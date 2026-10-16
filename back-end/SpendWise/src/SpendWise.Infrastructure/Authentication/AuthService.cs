using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SpendWise.Application.Auth;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Infrastructure.Identity;
using SpendWise.Infrastructure.Persistence;
using SpendWise.Domain.Defaults;
using FluentValidation.Results;

namespace SpendWise.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly AppDbContext _db;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwt,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        AppDbContext db)
    {
        _userManager = userManager;
        _jwt = jwt;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _db = db;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictException("Email is already registered.");

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            DisplayName = request.DisplayName
        };

        // Begin a DB transaction so user creation and default categories are atomic
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var failures = result.Errors.Select(e =>
            {
                var prop = string.Empty;
                if (!string.IsNullOrEmpty(e.Code) && e.Code.StartsWith("Password"))
                    prop = nameof(RegisterRequest.Password);
                else if (!string.IsNullOrEmpty(e.Code) && (e.Code.StartsWith("DuplicateEmail") || e.Code.StartsWith("InvalidEmail")))
                    prop = "Email";

                return new ValidationFailure(prop, e.Description);
            });

            throw new ValidationException(failures);
        }

        // Create default categories for the new user
        _db.Categories.AddRange(DefaultCategories.Create(user.Id));
        await _db.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        var (token, expires) = _jwt.Generate(user.Id, request.Email, user.DisplayName);
        return new AuthResponse(token, expires, user.DisplayName, request.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            throw new UnauthorizedException("Invalid credentials.");

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid)
            throw new UnauthorizedException("Invalid credentials.");

        var (token, expires) = _jwt.Generate(user.Id, request.Email, user.DisplayName);
        return new AuthResponse(token, expires, user.DisplayName, request.Email);
    }
}
