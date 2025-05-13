using Microsoft.AspNetCore.Mvc;

namespace GerenciarProdutos.Controllers;

[ApiController]
[Route("home")]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public ActionResult Get()
    {
        return Ok();
    }
    
}