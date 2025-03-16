
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
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
}