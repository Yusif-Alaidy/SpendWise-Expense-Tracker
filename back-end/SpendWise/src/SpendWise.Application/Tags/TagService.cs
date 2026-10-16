using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentValidation;
using SpendWise.Application.Common.Interfaces;

namespace SpendWise.Application.Tags;

public class TagService : ITagService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateTagRequest> _createValidator;
    private readonly IValidator<UpdateTagRequest> _updateValidator;
    private readonly ILogger<TagService> _logger;

    public TagService(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IValidator<CreateTagRequest> createValidator,
        IValidator<UpdateTagRequest> updateValidator,
        ILogger<TagService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TagListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var q = _db.Tags.AsNoTracking()
            .Select(t => new TagListItemDto(t.Id, t.Name, t.TransactionTags.Count()));

        return await q.OrderBy(t => t.Name).ToListAsync(cancellationToken);
    }

    public async Task<TagDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags.AsNoTracking().Where(t => t.Id == id).Select(t => new TagDto(t.Id, t.Name)).FirstOrDefaultAsync(cancellationToken);
        if (tag is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Tag not found.");
        return tag;
    }

    public async Task<TagDto> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var userId = _currentUser.UserId ?? throw new SpendWise.Application.Common.Exceptions.UnauthorizedException("User is not authenticated.");

        var name = request.Name.Trim();
        var exists = await _db.Tags.AnyAsync(t => t.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("A tag with this name already exists.");

        var entity = new Domain.Entities.Tag { UserId = userId, Name = name };
        _db.Tags.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tag created {TagId}", entity.Id);
        return new TagDto(entity.Id, entity.Name);
    }

    public async Task UpdateAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _db.Tags.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Tag not found.");

        var name = request.Name.Trim();
        var exists = await _db.Tags.AnyAsync(t => t.Id != id && t.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
            throw new SpendWise.Application.Common.Exceptions.ConflictException("A tag with this name already exists.");

        entity.Name = name;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tag updated {TagId}", entity.Id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Tags.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            throw new SpendWise.Application.Common.Exceptions.NotFoundException("Tag not found.");

        _db.Tags.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tag deleted {TagId}", entity.Id);
    }
}
