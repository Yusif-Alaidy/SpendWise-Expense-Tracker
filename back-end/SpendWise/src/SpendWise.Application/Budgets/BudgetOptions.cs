namespace SpendWise.Application.Budgets;

using System.ComponentModel.DataAnnotations;

public class BudgetOptions
{
    [Required]
    public string TimeZoneId { get; set; } = "Africa/Cairo";
}

