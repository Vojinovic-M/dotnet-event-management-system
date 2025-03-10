using EMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        
        if (user == null)  {  return  NotFound( new { message = "User not found" });  }

        return Ok(new { email = user.Email });
    }

}
