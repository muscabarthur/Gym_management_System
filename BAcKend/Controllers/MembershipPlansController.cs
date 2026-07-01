using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;
using GymManagement.Api.Models;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembershipPlansController : ControllerBase
{
    private readonly MembershipPlanData _data;
    public MembershipPlansController(MembershipPlanData data) => _data = data;

    [HttpGet]
    public IActionResult GetAll(string? search = null, decimal? maxPrice = null)
    {
        List<MembershipPlan> plans = _data.GetAllPlans();
        if (!string.IsNullOrWhiteSpace(search))
            plans = plans.Where(p => p.PlanName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        if (maxPrice.HasValue)
            plans = plans.Where(p => p.Price <= maxPrice.Value).ToList();
        return Ok(plans);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        MembershipPlan? plan = _data.GetPlanById(id);
        return plan == null ? NotFound(new { message = "Membership plan not found." }) : Ok(plan);
    }

    [HttpPost]
    public IActionResult Create(MembershipPlan plan)
    {
        plan.PlanName = plan.PlanName.Trim();
        int id = _data.AddPlan(plan);
        plan.PlanID = id;
        return CreatedAtAction(nameof(GetById), new { id }, plan);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, MembershipPlan plan)
    {
        if (_data.GetPlanById(id) == null)
            return NotFound(new { message = "Membership plan not found." });

        plan.PlanID = id;
        plan.PlanName = plan.PlanName.Trim();
        _data.UpdatePlan(plan);
        return Ok(new { message = "Membership plan updated successfully." });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _data.DeletePlan(id)
            ? Ok(new { message = "Membership plan deleted successfully." })
            : NotFound(new { message = "Membership plan not found." });
    }
}
