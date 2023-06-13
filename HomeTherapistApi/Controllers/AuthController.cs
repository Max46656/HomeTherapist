using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Models;
using HomeTherapistApi.Utilities;
using MimeKit;
using System.Configuration;
using MailKit.Net.Smtp;

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
    public async Task<ActionResult<ApiResponse<object>>> Login(LoginDto model)
    {
      // var span = _tracer.BuildSpan("Home_Controller_Get_Action").Start();
      if (model.Email == null || model.Password == null)
        return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "Email和密碼不能為空" });

      var user = await _userManager.FindByEmailAsync(model.Email);

      if (user == null || user.PasswordHash == null)
        return Unauthorized(new ApiResponse<string> { IsSuccess = false, Message = "未授權，請檢查帳號密碼" });

      var passwordHasher = new PasswordHasher<User>();
      var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

      if (verificationResult == PasswordVerificationResult.Failed)
        return Unauthorized(new ApiResponse<string> { IsSuccess = false, Message = "未授權，請檢查帳號密碼" });

      var token = GenerateJwtToken(user);
      // span.Finish();
      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "登入成功", Data = new { token } });
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<string>>> Logout()
    {
      await _signInManager.SignOutAsync();
      return Ok(new ApiResponse<string> { IsSuccess = true, Message = "登出成功" });
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<object>>> Register(RegisterDto model)
    {
      if (model.UserName == null || model.Email == null || model.StaffId == null || model.Password == null)
        return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "請填寫所有必填欄位" });

      var existingUser = await _userManager.FindByEmailAsync(model.Email);
      if (existingUser != null)
        return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "該Email已被註冊" });

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
        return BadRequest(new ApiResponse<object>
        { IsSuccess = false, Message = "註冊失敗", Data = result.Errors });

      var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
      // token = Encoding.UTF8.GetString(Base64Utils.Base64UrlDecode(token));
      var callbackUrl = Url.Link("ConfirmEmail", new { userId = newUser.Id, token });

      var emailMessage = new MimeMessage();
      emailMessage.From.Add(new MailboxAddress("寄件人姓名", "寄件人郵箱地址"));
      emailMessage.To.Add(new MailboxAddress("收件人姓名", newUser.Email));
      emailMessage.Subject = "請確認您的電子郵件地址";
      emailMessage.Body = new TextPart("html")
      {
        ContentTransferEncoding = ContentEncoding.Base64,
        Text = $"<a href=\"{callbackUrl}\">請點擊此處確認您的電子郵件地址</a>"
      };
      var emailSettings = _configuration.GetSection("EmailSettings");
      var smtpServer = emailSettings["SmtpServer"];
      var smtpPort = int.Parse(emailSettings["SmtpPort"]);
      var smtpUsername = emailSettings["SmtpUsername"];
      var smtpPassword = emailSettings["SmtpPassword"];
      using (var client = new SmtpClient())
      {
        client.Connect(smtpServer, smtpPort, false);
        client.Authenticate(smtpUsername, smtpPassword);
        client.Send(emailMessage);
        client.Disconnect(true);
      }

      // var token = GenerateJwtToken(newUser);
      return Ok(new ApiResponse<object>
      { IsSuccess = true, Message = "註冊成功", Data = new { callbackUrl, token } });
    }
    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        return BadRequest();

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        return NotFound();

      var result = await _userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
        return Ok("您的電子郵件已成功驗證");
      else
        return BadRequest("驗證電子郵件時發生錯誤");
    }
    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword(PasswordChangeDto passwordChange)
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object>
        { IsSuccess = false, Message = "無效的使用者 ID" });

      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
        return NotFound(new ApiResponse<object>
        { IsSuccess = false, Message = "找不到使用者" });

      var result = await _userManager.ChangePasswordAsync(user, passwordChange.CurrentPassword, passwordChange.NewPassword);
      if (!result.Succeeded)
        return BadRequest(new ApiResponse<object>
        { IsSuccess = false, Message = "變更密碼失敗", Data = result.Errors });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "變更密碼成功" });
    }
    private string GenerateJwtToken(User user)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException()));

      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email?? throw new InvalidOperationException()),
                new Claim("StaffId", user.StaffId)
            };

      var token = new JwtSecurityToken(
          _configuration["JwtSettings:Issuer"],
          _configuration["JwtSettings:Audience"],
          claims,
          expires: DateTime.UtcNow.AddHours(3),
          signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
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
      public string? Email { get; set; }

      [Required(ErrorMessage = "StaffId is required")]
      public string? StaffId { get; set; }
      [Required(ErrorMessage = "Username is required")]
      public string? UserName { get; set; }

      [Required(ErrorMessage = "Password is required")]
      [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
      public string? Password { get; set; }

      [Compare("Password", ErrorMessage = "Passwords do not match")]
      public string? ConfirmPassword { get; set; }
    }
    public class PasswordChangeDto
    {
      [Required]
      public string CurrentPassword { get; set; }

      [Required]
      [MinLength(6)]
      public string NewPassword { get; set; }
    }
  }


}
