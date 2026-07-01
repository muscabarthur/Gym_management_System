using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentData _data;
    private readonly MemberData _memberData;

    public PaymentsController(PaymentData data, MemberData memberData)
    {
        _data = data;
        _memberData = memberData;
    }

    [HttpGet]
    public IActionResult GetAll(string? search = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        List<PaymentDetail> payments = _data.GetAllPayments();
        if (!string.IsNullOrWhiteSpace(search))
            payments = payments.Where(p => p.MemberName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        if (fromDate.HasValue)
            payments = payments.Where(p => p.PaymentDate.Date >= fromDate.Value.Date).ToList();
        if (toDate.HasValue)
            payments = payments.Where(p => p.PaymentDate.Date <= toDate.Value.Date).ToList();
        return Ok(payments);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        PaymentDetail? payment = _data.GetPaymentById(id);
        return payment == null ? NotFound(new { message = "Payment not found." }) : Ok(payment);
    }

    [HttpPost]
    public IActionResult Create(Payment payment)
    {
        if (_memberData.GetMemberById(payment.MemberID) == null)
            return BadRequest(new { message = "The selected member does not exist." });

        int id = _data.AddPayment(payment);
        payment.PaymentID = id;
        return CreatedAtAction(nameof(GetById), new { id }, payment);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Payment payment)
    {
        if (_data.GetPaymentById(id) == null)
            return NotFound(new { message = "Payment not found." });
        if (_memberData.GetMemberById(payment.MemberID) == null)
            return BadRequest(new { message = "The selected member does not exist." });

        payment.PaymentID = id;
        _data.UpdatePayment(payment);
        return Ok(new { message = "Payment updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _data.DeletePayment(id)
            ? Ok(new { message = "Payment deleted successfully." })
            : NotFound(new { message = "Payment not found." });
    }
}
