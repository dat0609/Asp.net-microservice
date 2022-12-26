using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Identity;

namespace OcelotApiGw.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Post()
    {
        var token = _tokenService.GetToken(new TokenRequest());
        return Ok(token);
    }
}