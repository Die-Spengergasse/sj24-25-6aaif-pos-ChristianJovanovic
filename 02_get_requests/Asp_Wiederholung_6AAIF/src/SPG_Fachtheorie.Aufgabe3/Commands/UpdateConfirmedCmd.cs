using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands;

public record UpdateConfirmedCmd(
    string Confirmed
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateTime.Parse(Confirmed) > DateTime.Now.AddMinutes(1))
        {
            yield return new ValidationResult(
                errorMessage: "Confirmed Date is invalid"
            );
        }
    }
}