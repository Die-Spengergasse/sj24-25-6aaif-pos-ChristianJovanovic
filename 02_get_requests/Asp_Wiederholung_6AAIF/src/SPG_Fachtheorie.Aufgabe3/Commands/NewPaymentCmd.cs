using System.ComponentModel.DataAnnotations;
using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe3.Commands;

public record NewPaymentCmd(
    int cashDeskNumber,
    DateTime paymenDateTime,
    PaymentType paymentType,
    int employeeRegistrationNumber
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (paymenDateTime > (DateTime.Now.AddMinutes(1)))
        {
            yield return new ValidationResult(
                "Payment date is invalid",
                new[] { nameof(paymenDateTime) });
        }
    }
}

