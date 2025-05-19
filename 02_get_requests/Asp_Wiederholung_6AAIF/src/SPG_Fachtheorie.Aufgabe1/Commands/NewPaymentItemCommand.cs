using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Commands;

public record NewPaymentItemCommand
(
    int PaymentId,
    [MinLength(1), MaxLength(255)]
    string ArticleName,
    [Range(1, int.MaxValue)]
    int Amount, 
    [Range(0, int.MaxValue)]
    decimal Price
    );