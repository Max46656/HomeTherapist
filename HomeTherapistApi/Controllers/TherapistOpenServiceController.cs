using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeTherapistApi.Models;
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeTherapistApi.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class TherapistOpenServiceController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public TherapistOpenServiceController(HometherapistContext context)
    {
      _context = context;
    }

    // GET: api/TherapistOpenService
    [HttpGet]
    public async Task<IActionResult> GetTherapistOpenServices()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無法取得使用者資訊" });

      var services = await _context.TherapistOpenServices
          .Where(t => t.UserId == userId)
          .ToListAsync();

      return Ok(new ApiResponse<List<TherapistOpenService>>
      {
        IsSuccess = true,
        Message = "取得治療師開放服務成功",
        Data = services
      });
    }

    [HttpPost]
    public async Task<IActionResult> AddTherapistOpenService(ulong serviceId)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無法取得使用者資訊" });

      var user = await _context.Users.SingleOrDefaultAsync(u => u.StaffId == userId);
      if (user == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "使用者不存在" });

      var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == serviceId);
      if (service == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "服務不存在" });

      var existingService = await _context.TherapistOpenServices
          .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.UserId == userId);
      if (existingService != null)
        return Conflict(new ApiResponse<object> { IsSuccess = false, Message = "已啟用該服務" });

      var therapistOpenService = new TherapistOpenService
      {
        UserId = userId,
        ServiceId = serviceId,
        User = user,
        Service = service,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      _context.TherapistOpenServices.Add(therapistOpenService);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTherapistOpenServices), new
      {
        id = therapistOpenService.Id
      }, new ApiResponse<TherapistOpenService>
      {
        IsSuccess = true,
        Message = "新增治療師開放服務成功",
        Data = therapistOpenService
      });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTherapistOpenService(ulong? serviceId)
    {
      if (serviceId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請提供欲刪除的服務ID" });

      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無法取得使用者資訊" });

      var therapistOpenService = await _context.TherapistOpenServices
          .FirstOrDefaultAsync(a => a.UserId == userId && a.ServiceId == serviceId);

      if (therapistOpenService == null)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不到指定的治療師開放服務" });

      _context.TherapistOpenServices.Remove(therapistOpenService);
      await _context.SaveChangesAsync();

      return Ok(new ApiResponse<TherapistOpenService>
      {
        IsSuccess = true,
        Message = "刪除治療師開放服務成功",
        Data = therapistOpenService
      });
    }
  }
  public class TherapistOpenServiceDto
  {
    public string UserId { get; set; } = null!;
    public ulong? ServiceId { get; set; }
    public User? User { get; set; } = null!;
    public Service? Service { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
  }
}
