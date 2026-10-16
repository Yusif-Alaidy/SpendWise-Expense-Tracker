using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Defaults;

public static class DefaultCategories
{
    public static IReadOnlyList<Category> Create(Guid userId)
    {
        var items = new List<Category>
        {
            // Expense categories
            new Category { UserId = userId, Name = "Food", Type = TransactionType.Expense, Color = "#F59E0B" },
            new Category { UserId = userId, Name = "Transport", Type = TransactionType.Expense, Color = "#3B82F6" },
            new Category { UserId = userId, Name = "Rent", Type = TransactionType.Expense, Color = "#EF4444" },
            new Category { UserId = userId, Name = "Bills", Type = TransactionType.Expense, Color = "#8B5CF6" },
            new Category { UserId = userId, Name = "Shopping", Type = TransactionType.Expense, Color = "#EC4899" },
            new Category { UserId = userId, Name = "Health", Type = TransactionType.Expense, Color = "#10B981" },
            new Category { UserId = userId, Name = "Entertainment", Type = TransactionType.Expense, Color = "#F97316" },
            new Category { UserId = userId, Name = "Education", Type = TransactionType.Expense, Color = "#06B6D4" },
            new Category { UserId = userId, Name = "Other", Type = TransactionType.Expense, Color = "#6B7280" },

            // Income categories
            new Category { UserId = userId, Name = "Salary", Type = TransactionType.Income, Color = "#22C55E" },
            new Category { UserId = userId, Name = "Freelance", Type = TransactionType.Income, Color = "#14B8A6" },
            new Category { UserId = userId, Name = "Gift", Type = TransactionType.Income, Color = "#A855F7" },
            new Category { UserId = userId, Name = "Other", Type = TransactionType.Income, Color = "#6B7280" }
        };

        return items;
    }
}
