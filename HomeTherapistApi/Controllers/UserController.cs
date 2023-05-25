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
[Authorize]
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

    if (userId == null)
      return BadRequest();

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      return NotFound();

    return Ok(user);
  }

  // [HttpPut("{id}")]
  // public async Task<IActionResult> Update(ulong id, User updatedUser)
  // {
  //   if (id != updatedUser.Id)
  //     return BadRequest();

  //   var existingUser = await _context.Users.FindAsync(id);
  //   if (existingUser == null)
  //     return NotFound();

  //   existingUser.StaffId = updatedUser.StaffId;
  //   existingUser.CertificateNumber = updatedUser.CertificateNumber;
  //   existingUser.Address = updatedUser.Address;
  //   existingUser.Latitude = updatedUser.Latitude;
  //   existingUser.Longitude = updatedUser.Longitude;
  //   existingUser.Radius = updatedUser.Radius;
  //   existingUser.Username = updatedUser.Username;
  //   existingUser.NormalizedUsername = updatedUser.Username?.ToUpperInvariant();
  //   existingUser.Email = updatedUser.Email;
  //   existingUser.NormalizedEmail = updatedUser.Email?.ToUpperInvariant();
  //   existingUser.EmailConfirmed = updatedUser.EmailConfirmed;
  //   existingUser.PasswordHash = updatedUser.PasswordHash;
  //   existingUser.Password = updatedUser.Password;
  //   existingUser.SecurityStamp = updatedUser.SecurityStamp;
  //   existingUser.ConcurrencyStamp = updatedUser.ConcurrencyStamp;
  //   existingUser.PhoneNumber = updatedUser.PhoneNumber;
  //   existingUser.PhoneNumberConfirmed = updatedUser.PhoneNumberConfirmed;
  //   existingUser.TwoFactorEnabled = updatedUser.TwoFactorEnabled;
  //   existingUser.LockoutEnd = updatedUser.LockoutEnd;
  //   existingUser.LockoutEnabled = updatedUser.LockoutEnabled;
  //   existingUser.AccessFailedCount = updatedUser.AccessFailedCount;
  //   existingUser.RememberToken = updatedUser.RememberToken;
  //   existingUser.CreatedAt = updatedUser.CreatedAt;
  //   existingUser.UpdatedAt = updatedUser.UpdatedAt;

  //   await _context.SaveChangesAsync();
  //   return NoContent();
  // }
  [HttpGet("GetAppointmentsByUser")]
  public IActionResult GetAppointmentsByUser()
  {
    var userId = User.FindFirst("StaffId")?.Value;
    var appointments = _context.Users
                                .Where(u => u.StaffId == userId)
                                .SelectMany(u => u.Appointments)
                                .ToList();

    return Ok(appointments);
  }


  [HttpGet("GetOrdersByUser")]
  public async Task<IActionResult> GetOrdersByUser()
  {
    var staffId = User.FindFirst("StaffId")?.Value;

    var orders = await _context.Orders
                               .Where(o => o.UserId == staffId)
                               .ToListAsync();

    return Ok(orders);
  }
  [HttpGet("GetFeedbacksByUser")]
  public IActionResult GetFeedbacksByUser()
  {
    var staffId = User.FindFirst("StaffId")?.Value;
    var feedbacks = _context.Users
                              .Where(u => u.StaffId == staffId)
                              .SelectMany(u => u.Feedbacks)
                              .ToList();

    return Ok(feedbacks);
  }
  [HttpGet("GetArticlesByUser")]
  public IActionResult GetArticlesByUser()
  {
    var staffId = User.FindFirst("StaffId")?.Value;
    var articles = _context.Users
                           .Where(u => u.StaffId == staffId)
                           .SelectMany(u => u.Articles)
                           .ToList();

    return Ok(articles);
  }
}
