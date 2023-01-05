using Microsoft.AspNetCore.Mvc;

namespace Inventory.Product.API.Controllers;

public class HomeController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return Redirect("~/swagger");
    }
}