using System;
using Microsoft.Extensions.Options;

namespace SpendWise.Application.Budgets;

public class BudgetPeriodResolver : IBudgetPeriodResolver
{
    private readonly TimeProvider _timeProvider;
    private readonly TimeZoneInfo _timeZone;

    public BudgetPeriodResolver(TimeProvider timeProvider, IOptions<BudgetOptions> options)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        var tzId = options?.Value?.TimeZoneId ?? "Africa/Cairo";

        try
        {
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            }
            catch (TimeZoneNotFoundException ex)
            {
                throw new InvalidOperationException($"Neither configured timezone '{tzId}' nor fallback 'Egypt Standard Time' could be found on this system.", ex);
            }
        }
    }

    public (int Year, int Month) Resolve(int? year, int? month)
    {
        if (year.HasValue ^ month.HasValue)
        {
            var missing = year.HasValue ? "Month" : "Year";
            throw new FluentValidation.ValidationException(new[] { new FluentValidation.Results.ValidationFailure(missing, "Year and month must be provided together.") });
        }

        if (year.HasValue && month.HasValue)
            return (year.Value, month.Value);

        var utc = _timeProvider.GetUtcNow();
        var utcDateTime = utc.UtcDateTime;
        var tzTime = TimeZoneInfo.ConvertTime(utcDateTime, TimeZoneInfo.Utc, _timeZone);
        return (tzTime.Year, tzTime.Month);
    }

    public (DateOnly Start, DateOnly EndExclusive) GetRange(int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1);
        return (start, end);
    }
}
