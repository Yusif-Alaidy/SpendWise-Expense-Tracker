namespace SpendWise.Application.Tags;

public interface ITagService
{
    Task<IReadOnlyList<TagListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TagDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TagDto> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
