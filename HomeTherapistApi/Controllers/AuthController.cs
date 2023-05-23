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

namespace HomeTherapistApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, IPasswordHasher<User> passwordHasher)
    {
      _configuration = configuration;
      _userManager = userManager;
      _signInManager = signInManager;
      _passwordHasher = passwordHasher;

    }
    // 測試用Hash密碼生成
    // var passwordHasher = new PasswordHasher<User>();
    // var hashedPassword = passwordHasher.HashPassword(null, "123456");
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {

      // var user = await _userManager.FindByNameAsync(model.Name);
      var user = await _userManager.FindByEmailAsync(model.Email);

      if (user == null)
        return Unauthorized();

      var passwordHasher = new PasswordHasher<User>();
      var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

      if (verificationResult == PasswordVerificationResult.Failed)
        return Unauthorized();

      var token = GenerateJwtToken(user);

      return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
      var existingUser = await _userManager.FindByNameAsync(model.UserName);
      if (existingUser != null)
        return BadRequest("Username already exists.");
      existingUser = await _userManager.FindByEmailAsync(model.Email);
      if (existingUser != null)
        return BadRequest("Email already exists.");


      var newUser = new User
      {
        StaffId = model.StaffId,
        UserName = model.UserName,
        NormalizedUserName = model.UserName.ToUpper(),
        Email = model.Email,
        NormalizedEmail = model.Email.ToUpper(),
        EmailConfirmed = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };


      newUser.PasswordHash = _passwordHasher.HashPassword(newUser, model.Password);


      var result = await _userManager.CreateAsync(newUser);
      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }
      var token = GenerateJwtToken(newUser);
      return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("StaffId", user.StaffId)
            };

      var token = new JwtSecurityToken(
          _configuration["JwtSettings:Issuer"],
          _configuration["JwtSettings:Audience"],
          claims,
          expires: DateTime.UtcNow.AddDays(7),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }

  public class LoginDto
  {
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
  }
  public class RegisterDto
  {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "StaffId is required")]
    public string StaffId { get; set; }
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
  }
}
