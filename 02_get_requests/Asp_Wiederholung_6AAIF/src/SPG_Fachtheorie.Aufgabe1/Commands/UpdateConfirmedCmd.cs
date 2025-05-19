using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Commands;

public record UpdateConfirmedCmd(
    DateTime Confirmed
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //Client sendet im Normalfall keine Zeitzonenangaben, deswegen wird hier DateTime.Now anstatt DateTime.UtcNow verwendet
        if (Confirmed > DateTime.Now.AddMinutes(1))
        {
            yield return new ValidationResult(
                errorMessage: "Confirmed Date is invalid",
                //Welche Properties haben den Fehler verursacht
                new string[] { nameof(Confirmed) }
            );
        }
    }
}