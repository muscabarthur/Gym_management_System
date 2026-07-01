using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly DashboardData _data;
    public DashboardController(DashboardData data) => _data = data;

    [HttpGet]
    public IActionResult GetDashboard() => Ok(_data.GetSummary());
}
