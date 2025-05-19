using System;

namespace SPG_Fachtheorie.Aufgabe1.Services;

public class PaymentServiceException : Exception
{
    public PaymentServiceException() : base() { }
    public PaymentServiceException(string message) : base(message) { }
    public PaymentServiceException(string message, Exception innerException) : base(message, innerException) { }
    public bool NotFoundException { get; set; }
}