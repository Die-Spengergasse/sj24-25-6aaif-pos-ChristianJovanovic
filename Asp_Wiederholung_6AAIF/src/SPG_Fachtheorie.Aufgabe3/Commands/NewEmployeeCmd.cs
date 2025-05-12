using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands;

public record NewEmployeeCmd(
    [Range(1, 999999, ErrorMessage = "Invalid registration number")]
    int RegistrationNumber,
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid firstname")]
    string FirstName,
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid firstname")]
    string LastName,
    AdressCmd? Adress
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FirstName.Length + LastName.Length < 3)
            yield return new ValidationResult(
                "Invalid name",
                new[] { nameof(FirstName), nameof(LastName) }
            );
    }
}
    
    public record AdressCmd(
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid firstname")]
        string Street,
        [RegularExpression(@"^[0-9]{4,5}$", ErrorMessage = "Invalid zip")]
        string Zip,
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid firstname")]
        string City
        );