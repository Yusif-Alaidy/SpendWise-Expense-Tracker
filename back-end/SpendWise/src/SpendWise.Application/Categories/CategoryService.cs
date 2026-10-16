using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpendWise.Application.Common.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using FluentValidation;

namespace SpendWise.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCategoryRequest> _createValidator;
    private readonly IValidator<UpdateCategoryRequest> _updateValidator;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IValidator<CreateCategoryRequest> createValidator,
        IValidator<UpdateCategoryRequest> updateValidator,
        ILogger<CategoryService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(TransactionType? type = null, CancellationToken cancellationToken = default)
    {
        var q = _db.Categories.AsNoTracking().AsQueryable();
        if (type is not null)
            q = q.Where(c => c.Type == type.Value);

        var result = await q.OrderBy(c => c.Type).ThenBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Type, c.Color))
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var c = await _db.Categories.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Type, c.Color))
            .FirstOrDefaultAsync(cancellationToken);

        if (c is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Category not found.");

        return c;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = _currentUser.UserId ?? throw new SpendWise.Application.Common.Exceptions.UnauthorizedException("User is not authenticated.");

        var name = request.Name.Trim();

        var exists = await _db.Categories.AnyAsync(c => c.Type == request.Type && c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("A category with this name and type already exists.");

        var entity = new Category
        {
            UserId = userId,
            Name = name,
            Type = request.Type,
            Color = request.Color
        };

        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category created {CategoryId}", entity.Id);

        return new CategoryDto(entity.Id, entity.Name, entity.Type, entity.Color);
    }

    public async Task UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Category not found.");

        var name = request.Name.Trim();
        var exists = await _db.Categories.AnyAsync(c => c.Id != id && c.Type == entity.Type && c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("A category with this name and type already exists.");

        entity.Name = name;
        entity.Color = request.Color;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category updated {CategoryId}", entity.Id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Category not found.");

        var inUse = await _db.Transactions.AnyAsync(t => t.CategoryId == id, cancellationToken);
        if (inUse)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("Category is in use and cannot be deleted.");

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category deleted {CategoryId}", entity.Id);
    }
}
