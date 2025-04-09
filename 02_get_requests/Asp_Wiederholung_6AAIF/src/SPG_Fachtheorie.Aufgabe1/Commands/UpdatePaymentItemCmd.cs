using System;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Commands;

public record UpdatePaymentItemCmd(
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    int Id,
    [MinLength(1, ErrorMessage = "Name can´t be empty")]
    string ArticleName,
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    int Amount,
    [Range(1, 1000000, ErrorMessage = "Price must be greater than 0")]
    decimal Price,
    [Range(1, int.MaxValue, ErrorMessage = "PaymentId must be greater than 0")]
    int PaymentId,
    DateTime? LastUpdated
    );