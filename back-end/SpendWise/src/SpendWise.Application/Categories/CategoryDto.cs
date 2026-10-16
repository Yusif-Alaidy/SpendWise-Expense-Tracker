using SpendWise.Domain.Enums;

namespace SpendWise.Application.Categories;

public record CategoryDto(int Id, string Name, TransactionType Type, string Color);
