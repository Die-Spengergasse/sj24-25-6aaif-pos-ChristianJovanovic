
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers;


[Route("api/payments/[controller]")]
[ApiController]

public class PaymentsController : ControllerBase
{
    private readonly AppointmentContext _db;

    public PaymentsController(AppointmentContext db)
    {
        _db = db;
    }

    [HttpGet]
    public ActionResult<List<PaymentDto>> GetPayments([FromQuery] int? cashDesk, [FromQuery] DateTime? dateFrom)
    {
        /*
         * .Where(p => cashDesk.HasValue ? p.CashDesk.Number == cashDesk.Value : true)
         * .Where(p => dateFrom.HasValue ? p.PaymentDateTime >= dateFrom.Value : true)
         */
        return Ok(_db.Payments
            .Where(p => cashDesk.HasValue ? p.CashDesk.Number == cashDesk.Value : true)
            .Where(p => dateFrom.HasValue ? p.PaymentDateTime >= dateFrom.Value : true)
            .Select(e => new PaymentDto(
            e.Id,
            e.Employee.FirstName,
            e.Employee.LastName,
            e.CashDesk.Number,
            e.PaymentType.ToString(),
            e.PaymentItems.Select(s => s.Price).Sum()
        )).ToList());
    }

    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PaymentDto> GetPaymentById([FromQuery] int id)
    {
        var payment = _db.Payments
            .Where(e => e.Id == id)
            .Select(e => new PaymentDetailDto(
                e.Id,
                e.Employee.FirstName,
                e.Employee.LastName,
                e.CashDesk.Number,
                e.PaymentType.ToString(),
                e.PaymentItems.Select(p => new PaymentItemDto(
                    p.ArticleName,
                    p.Amount,
                    p.Price
                )).ToList()
            ))
            .AsNoTracking()
            .FirstOrDefault();
        if (payment is null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<int> CreatePayment([FromBody] NewPaymentCmd paymentCmd)
    {
        CashDesk cashDesk = _db.CashDesks.FirstOrDefault(a => a.Number == paymentCmd.cashDeskNumber);
        Employee employee =
            _db.Employees.FirstOrDefault(e => e.RegistrationNumber == paymentCmd.employeeRegistrationNumber);
        var isPayment =  Enum.TryParse<PaymentType>(paymentCmd.paymentType, true, out PaymentType paymentType);
        if (cashDesk is null)
        {
            return BadRequest("Cash desk not found");
        }

        if (employee is null)
        {
            return BadRequest("Employee not found");
        }

        if (!isPayment)
        {
            return BadRequest("Invalid payment type");
        }
        
        var payment = new Payment(cashDesk, DateTime.Now, employee, Enum.Parse<PaymentType>(paymentCmd.paymentType));
        try
        {
            _db.Payments.Add(payment);
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        return CreatedAtAction(nameof(CreatePayment), new { id = payment.Id }, payment.Id);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    public ActionResult<int> DeletePayment(int id, [FromQuery] bool? deleteItems)
    {
        try
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null)
            {
                return NotFound();
            }

            if (deleteItems is true)
            {
                var paymentItems = _db.PaymentItems.Where(p => p.Payment.Id == id).ToList();
                _db.RemoveRange(paymentItems);
                _db.Remove(payment);
                _db.SaveChanges();
                return NoContent();
            }
            else
            {
                return BadRequest("Payment has payment items!");
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    //Wichtig: auf die Route achten!
    [HttpPut("/api/paymentItems/{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType((StatusCodes.Status204NoContent))]

    public ActionResult UpdatePaymentItem(int id, [FromBody] UpdatePaymentItemCmd paymentItem)
    {
        if (id != paymentItem.Id)
        {
            return Problem("Invalid payment item ID", statusCode: 400);
        }
        var paymentItemDb = _db.PaymentItems.FirstOrDefault(p => p.Id == id);
        var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentItem.PaymentId);
        if (paymentItemDb is null)
        {
            return Problem("Payment Item not found", statusCode: 404);
        }

        if (paymentItem.LastUpdated != paymentItemDb.LastUpdated)
        {
            return Problem("Payment item has changed", statusCode: 400);
        }

        if (payment is null)
        {
            return Problem("Invalid payment ID", statusCode: 400);
        }
        paymentItemDb.ArticleName = paymentItem.ArticleName;
        paymentItemDb.Amount = paymentItem.Amount;
        paymentItemDb.LastUpdated = DateTime.UtcNow;
        paymentItemDb.Price = paymentItem.Price;
        return TrySave(new NoContentResult());
       
        //Wird gesendet wenn der Client bereits alle Informationen hat und nur eine Rückmeldung braucht ob die Aktion erfolgreich war
        return NoContent();
    }

    private ActionResult TrySave(ActionResult successResult)
    {
        try
        {
            _db.SaveChanges();
            return successResult;
        }
        catch (Exception e)
        {
            return Problem(e.InnerException?.Message ?? e.Message);
        }
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType((StatusCodes.Status204NoContent))]

    public ActionResult UpdatePayment(int id, [FromBody] UpdateConfirmedCmd updateConfirmed)
    {
        var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
        if (payment is null)
        {
            return Problem("Payment not found", statusCode: 404);
        }

        if (payment.Confirmed is not null)
        {
            return Problem("Payment already confirmed", statusCode: 400);
        }
        payment.Confirmed = updateConfirmed.Confirmed;
        return TrySave(new NoContentResult());
    }
}