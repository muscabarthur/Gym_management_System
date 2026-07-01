using Microsoft.AspNetCore.Mvc;
using GymManagement.Api.Data;

namespace GymManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebsiteController : ControllerBase
{
    private readonly WebsiteData _data;
    public WebsiteController(WebsiteData data) => _data = data;

    [HttpGet("home")]
    public IActionResult GetHomePage() => Ok(_data.GetHomePage());

    [HttpGet("about")]
    public IActionResult GetAboutPage() => Ok(_data.GetAboutPage());
}
