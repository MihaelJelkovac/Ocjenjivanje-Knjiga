using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Lab5.Models;

public class IsbnAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var s = value as string;

        // Let [Required] handle empty values
        if (string.IsNullOrWhiteSpace(s))
            return ValidationResult.Success;

        // Remove non-digit characters
        var digits = new string(s.Where(char.IsDigit).ToArray());

        // ISBN-10: 10 digits
        if (digits.Length == 10)
            return ValidationResult.Success;

        // ISBN-13: 13 digits and usually starts with 978 or 979
        if (digits.Length == 13 && (digits.StartsWith("978") || digits.StartsWith("979")))
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage ?? "ISBN nije u ispravnom formatu");
    }
}

