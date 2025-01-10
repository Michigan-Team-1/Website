using FluentValidation;
using System.Reflection;

namespace Team1.Web.Common;

public static class FluentValidationHelpers
{
    public static void SetupDisplayNameResolver()
    {
        ValidatorOptions.Global.DisplayNameResolver = (_, memberInfo, _) => memberInfo?.GetCustomAttributes<System.ComponentModel.DataAnnotations.DisplayAttribute>(true)?.FirstOrDefault()?.GetName() ?? memberInfo?.Name ?? "Unknown";
    }
}
