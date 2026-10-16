using SpendWise.Domain.Enums;

namespace SpendWise.Application.Categories;

public record CreateCategoryRequest(string Name, TransactionType Type, string Color);
