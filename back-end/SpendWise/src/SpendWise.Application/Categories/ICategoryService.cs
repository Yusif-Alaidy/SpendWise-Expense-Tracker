using SpendWise.Domain.Enums;

namespace SpendWise.Application.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(TransactionType? type = null, CancellationToken cancellationToken = default);
    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
