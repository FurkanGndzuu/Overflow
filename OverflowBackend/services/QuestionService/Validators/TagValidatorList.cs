using System.ComponentModel.DataAnnotations;

namespace QuestionService.Validators
{
    public class TagValidatorList(int min , int max) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not List<string> tags)
            {
                return new ValidationResult("Invalid tag list.");
            }
            if (tags.Count < min || tags.Count > max)
            {
                return new ValidationResult($"The number of tags must be between {min} and {max}.");
            }
            return ValidationResult.Success;
        }
    }
}
