using System.ComponentModel.DataAnnotations;
using LunarLensBackend.Entities.Enums;

namespace LunarLensBackend.Utility;

public class EnumValidationAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumValidationAttribute(Type enumType)
    {
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || !Enum.IsDefined(_enumType, value))
        {
            return new ValidationResult($"Invalid value for {validationContext.DisplayName}.");
        }

        return ValidationResult.Success;
    }
}