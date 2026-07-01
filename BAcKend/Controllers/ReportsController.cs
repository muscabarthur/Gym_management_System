using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly PaymentData _paymentData;
    private readonly EquipmentData _equipmentData;
    private readonly MemberProgramData _memberProgramData;

    public ReportsController(
        PaymentData paymentData,
        EquipmentData equipmentData,
        MemberProgramData memberProgramData)
    {
        _paymentData = paymentData;
        _equipmentData = equipmentData;
        _memberProgramData = memberProgramData;
    }

    [HttpGet("payments-by-member")]
    public IActionResult PaymentsByMember()
    {
        // LINQ query syntax example.
        List<MemberPaymentReport> report =
            (from payment in _paymentData.GetAllPayments()
             group payment by new { payment.MemberID, payment.MemberName } into paymentGroup
             orderby paymentGroup.Sum(x => x.Amount) descending
             select new MemberPaymentReport
             {
                 MemberID = paymentGroup.Key.MemberID,
                 MemberName = paymentGroup.Key.MemberName,
                 PaymentCount = paymentGroup.Count(),
                 TotalPaid = paymentGroup.Sum(x => x.Amount)
             }).ToList();

        return Ok(report);
    }

    [HttpGet("equipment-by-status")]
    public IActionResult EquipmentByStatus()
    {
        // LINQ method syntax example.
        List<EquipmentStatusReport> report = _equipmentData.GetAllEquipment()
            .GroupBy(e => e.Status)
            .Select(group => new EquipmentStatusReport
            {
                Status = group.Key,
                EquipmentTypes = group.Count(),
                TotalQuantity = group.Sum(e => e.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .ToList();

        return Ok(report);
    }

    [HttpGet("program-enrollment")]
    public IActionResult ProgramEnrollment()
    {
        List<ProgramEnrollmentReport> report = _memberProgramData.GetAllMemberPrograms()
            .GroupBy(x => new { x.ProgramID, x.ProgramName })
            .Select(group => new ProgramEnrollmentReport
            {
                ProgramID = group.Key.ProgramID,
                ProgramName = group.Key.ProgramName,
                MembersCount = group.Select(x => x.MemberID).Distinct().Count()
            })
            .OrderByDescending(x => x.MembersCount)
            .ToList();

        return Ok(report);
    }
}
