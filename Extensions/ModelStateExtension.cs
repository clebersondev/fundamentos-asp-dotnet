using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogEF3.Extensions;

public static class ModelStateExtension
{
    public static List<string> GetErrors(this ModelStateDictionary modelState)
    {
        var result = new List<string>();

        foreach (var item in modelState.Values)
        {
            foreach (var error in item.Errors) // result.AddRange(item.Errors.Select(error => error.ErrorMessage));
            {
                result.Add(error.ErrorMessage);
            }
        }

        return result;
    }
}