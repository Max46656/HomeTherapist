using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  private readonly HometherapistContext _context;
  private readonly UserManager<User> _userManager;

  public UserController(UserManager<User> userManager, HometherapistContext context)
  {
    _userManager = userManager;
    _context = context;
  }
  [HttpGet]
  public async Task<IActionResult> Get()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return NotFound();
    }

    return Ok(user);
  }

  [HttpPut]
  public async Task<IActionResult> Update(User updatedUser)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return NotFound();
    }

    // Update user fields here...
    user.Email = updatedUser.Email;
    user.PhoneNumber = updatedUser.PhoneNumber;
    // ...

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    return NoContent();
  }

  [HttpGet("GetOrdersByUser")]
  public async Task<IActionResult> GetOrdersByUser()
  {
    // Get the current user's ID.
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return Ok(userId);
    // Get the orders that belong to the current user.
    var orders = await _context.Orders
                               .Where(o => o.UserId == userId)
                               .ToListAsync();

    return Ok(orders);
  }
}
