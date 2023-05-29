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
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.JsonPatch;

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
  //[
  //   {
  //     "op": "replace",
  //     "path": "/certificateNumber",
  //     "value": 12345
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/address",
  //     "value": "新地址"
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/latitude",
  //     "value": 1.23456
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/longitude",
  //     "value": 2.34567
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/radius",
  //     "value": 100
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/userName",
  //     "value": "新用户名"
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/email",
  //     "value": "newemail@example.com"
  //   },
  //   {
  //   "op": "replace",
  //     "path": "/phoneNumber",
  //     "value": "1234567890"
  //   }
  // ]

  [HttpPatch]
  public async Task<IActionResult> UpdateUser([FromBody] JsonPatchDocument<UserUpdateDto> patchDocument)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userId == null)
      return BadRequest();

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      return NotFound();

    var userDto = new UserUpdateDto
    {
      CertificateNumber = user.CertificateNumber,
      Address = user.Address,
      Latitude = user.Latitude,
      Longitude = user.Longitude,
      Radius = user.Radius,
      UserName = user.UserName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber
    };

    patchDocument.ApplyTo(userDto, ModelState);
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    user.CertificateNumber = userDto.CertificateNumber;
    user.Address = userDto.Address;
    user.Latitude = userDto.Latitude;
    user.Longitude = userDto.Longitude;
    user.Radius = userDto.Radius;
    user.UserName = userDto.UserName;
    user.Email = userDto.Email;
    user.PhoneNumber = userDto.PhoneNumber;

    user.NormalizedUserName = _userManager.NormalizeName(userDto.UserName);
    user.NormalizedEmail = _userManager.NormalizeEmail(userDto.Email);

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
      return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "更新使用者資料失敗", Data = result.Errors });

    return Ok(new ApiResponse<object> { IsSuccess = true, Message = "更新使用者資料成功", Data = user });
  }



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
public class UserUpdateDto
{
  public uint? CertificateNumber { get; set; }
  public string? Address { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public uint? Radius { get; set; }
  public string? UserName { get; set; }
  public string? Email { get; set; }
  public string? PhoneNumber { get; set; }
}

