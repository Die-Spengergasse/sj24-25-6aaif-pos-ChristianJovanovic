using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Commands;


public record NewPaymentCmd(
    int CashDeskNumber,
    string PaymentType,
    int EmployeeRegistrationNumber
);

