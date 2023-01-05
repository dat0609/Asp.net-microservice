using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

public class HomeController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}