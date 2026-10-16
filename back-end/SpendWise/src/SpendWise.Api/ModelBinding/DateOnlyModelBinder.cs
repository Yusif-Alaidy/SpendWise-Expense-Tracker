using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SpendWise.Api.ModelBinding;

public class DateOnlyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
            return Task.CompletedTask;

        if (DateOnly.TryParse(value, out var date))
        {
            bindingContext.Result = ModelBindingResult.Success(date);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date format.");
        }

        return Task.CompletedTask;
    }
}
