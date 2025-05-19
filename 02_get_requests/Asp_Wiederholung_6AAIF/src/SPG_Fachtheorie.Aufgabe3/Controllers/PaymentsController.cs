
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Services;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers;


[Route("api/[controller]")]
[ApiController]

public class PaymentsController : ControllerBase
{
    
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public ActionResult<List<PaymentDto>> GetPayments([FromQuery] int? cashDesk, [FromQuery] DateTime? dateFrom)
    {
        /*
         * .Where(p => cashDesk.HasValue ? p.CashDesk.Number == cashDesk.Value : true)
         * .Where(p => dateFrom.HasValue ? p.PaymentDateTime >= dateFrom.Value : true)
         */
        return Ok(_paymentService.Payments
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
        var payment = _paymentService.Payments
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
    public ActionResult<NewPaymentCmd> CreatePayment([FromBody] NewPaymentCmd paymentCmd)
    {
        try
        {
            var payment = _paymentService.CreatePayment(paymentCmd);
            return CreatedAtAction(nameof(CreatePayment), paymentCmd);
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 400);
        }
    }

    [HttpPost("/api/paymentItems/")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult AddPaymentItem(NewPaymentItemCommand cmd)
    {
        try
        {
            _paymentService.AddPaymentItem(cmd);
            return CreatedAtAction(nameof(AddPaymentItem), new {Id = cmd.PaymentId});
        }
        catch (PaymentServiceException e) when (e.NotFoundException)
        {
            return Problem(e.Message, statusCode: 404);
        }
        catch (PaymentServiceException e)
        {
            return Problem(e.Message, statusCode: 400);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType((StatusCodes.Status400BadRequest))]
    public ActionResult DeletePayment(int id, [FromQuery] bool? deleteItems)
    {
        try
        {
            _paymentService.DeletePayment(id, deleteItems ?? false);
            return NoContent();
        }
        catch (PaymentServiceException e) when (e.NotFoundException)
        {
            return Problem(e.Message, statusCode: 404);
        }
        catch (PaymentServiceException e)
        {
            return Problem(e.Message, statusCode: 400);
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
        var paymentItemDb = _paymentService.PaymentItems.FirstOrDefault(p => p.Id == id);
        var payment = _paymentService.Payments.FirstOrDefault(p => p.Id == paymentItem.PaymentId);
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
        return NoContent();
       
        //Wird gesendet wenn der Client bereits alle Informationen hat und nur eine Rückmeldung braucht ob die Aktion erfolgreich war
        return NoContent();
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType((StatusCodes.Status204NoContent))]

    public ActionResult UpdatePayment(int id)
    {
        try
        {
            _paymentService.ConfirmPayment(id);
            return NoContent();
        }
        catch (PaymentServiceException e) when (e.NotFoundException)
        {
            return Problem(e.Message, statusCode: 404);
        }
        catch (PaymentServiceException e)
        {
            return Problem(e.Message, statusCode: 400);
        }
    }
}