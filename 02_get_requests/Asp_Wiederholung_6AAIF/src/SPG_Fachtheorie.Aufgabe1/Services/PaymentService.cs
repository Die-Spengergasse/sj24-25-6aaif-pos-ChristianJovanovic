using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Services;

namespace SPG_Fachtheorie.Aufgabe1.Services
{

    public class PaymentService
    {
        private readonly AppointmentContext _db;

        public PaymentService(AppointmentContext db)
        {
            _db = db;
        }

        public IQueryable<PaymentItem> PaymentItems => _db.PaymentItems.AsQueryable();
        public IQueryable<Payment> Payments => _db.Payments.AsQueryable();


        public Payment CreatePayment(NewPaymentCmd cmd)
        {
            var cashDesk = _db.CashDesks.FirstOrDefault(a => a.Number == cmd.CashDeskNumber);
            var employee = _db.Employees.FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            var isPayment = Enum.TryParse<PaymentType>(cmd.PaymentType, true, out PaymentType paymentType);

            if (cashDesk is null)
            {
                throw new PaymentServiceException("CashDesk not found");
            }

            if (employee is null)
            {
                throw new PaymentServiceException("Employee not found");
            }

            if (!isPayment)
            {
                throw new PaymentServiceException("PaymentType not valid");
            }

            var openPayment = _db.Payments.Any(p => p.CashDesk.Number == cmd.CashDeskNumber && p.Confirmed == null);
            if (openPayment)
            {
                throw new PaymentServiceException("Open payment for cashdesk");
            }

            if (cmd.PaymentType == "CreditCard" && employee.Type != "Manager")
            {
                throw new PaymentServiceException("Insufficient rights to create a credit card payment.");
            }

            var payment = new Payment(cashDesk, DateTime.Now, employee, Enum.Parse<PaymentType>(cmd.PaymentType), DateTime.UtcNow);
            _db.Payments.Add(payment);
            SaveOrThrow();
            return payment;
        }

        public void ConfirmPayment(int paymentId)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null)
            {
                //return Problem("Payment not found", statusCode: 404);
                throw new PaymentServiceException("Payment not found") { NotFoundException = true };
            }

            if (payment.Confirmed is not null)
            {
                //return Problem("Payment already confirmed", statusCode: 400);
                throw new PaymentServiceException("Payment already confirmed");
            }

            payment.Confirmed = DateTime.UtcNow;
            SaveOrThrow();
        }

        public void AddPaymentItem(NewPaymentItemCommand cmd)
        {
            var payment = Payments.FirstOrDefault(p => p.Id == cmd.PaymentId);
            if (payment is null)
            {
                throw new PaymentServiceException("Payment not found") { NotFoundException = true };
            }

            if (payment.Confirmed is not null)
            {
                throw new PaymentServiceException("Payment already confirmed");
            }

            PaymentItem paymentItem = new PaymentItem(cmd.ArticleName, cmd.Amount, cmd.Price, payment);
            _db.PaymentItems.Add(paymentItem);
            SaveOrThrow();
        }

        public void DeletePayment(int paymentId, bool deleteItems)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null)
            {
                throw new PaymentServiceException("Payment not found") { NotFoundException = true };
            }

            if (deleteItems is false)
            {
                throw new PaymentServiceException("Payment has items");
            }
            var paymentItems = _db.PaymentItems.Where(p => p.Payment.Id == paymentId).ToList();
            _db.RemoveRange(paymentItems);
            _db.Remove(payment);
            SaveOrThrow(); 
        }

        private void SaveOrThrow()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e);
                throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}