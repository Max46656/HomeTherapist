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
  public class TherapistOpenTimeController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public TherapistOpenTimeController(HometherapistContext context)
    {
      _context = context;
    }

    // GET: api/TherapistOpenTime
    [HttpGet]
    public async Task<IActionResult> GetTherapistOpenTimes()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無法取得使用者資訊" });

      var openTimes = await _context.TherapistOpenTimes
          .Where(t => t.UserId == userId)
          .ToListAsync();

      return Ok(new ApiResponse<List<TherapistOpenTime>>
      {
        IsSuccess = true,
        Message = "取得治療師開放時間成功",
        Data = openTimes
      });
    }
    // {
    //   "UserId": "T5106",
    //   "StartDt": "2023-01-14T09:00:00"
    // }
    // POST: api/TherapistOpenTime
    [HttpPost]
    public async Task<IActionResult> PostTherapistOpenTime(DateTime? startDt)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無法取得使用者資訊" });

      var user = await _context.Users.SingleOrDefaultAsync(u => u.StaffId == userId);
      if (user == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "使用者不存在" });

      var calendar = await _context.Calendars.SingleOrDefaultAsync(c => c.Dt == startDt);
      if (calendar == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "可設定時間以10分鐘為刻度" });

      var existingOpenTime = await _context.TherapistOpenTimes
          .FirstOrDefaultAsync(s => s.StartDt == startDt && s.UserId == userId);
      if (existingOpenTime != null)
        return Conflict(new ApiResponse<object> { IsSuccess = false, Message = "已啟用該時間" });

      var therapistOpenTime = new TherapistOpenTime
      {
        UserId = userId,
        StartDt = startDt,
        User = user,
        Calendar = calendar,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      _context.TherapistOpenTimes.Add(therapistOpenTime);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTherapistOpenTimes), new { id = therapistOpenTime.Id }, new ApiResponse<TherapistOpenTime>
      {
        IsSuccess = true,
        Message = "新增治療師開放時間成功",
        Data = therapistOpenTime
      });
    }
    // DELETE: api/TherapistOpenTime/5
    [HttpDelete]
    public async Task<IActionResult> DeleteTherapistOpenTime(DateTime? startDt)
    {
      if (startDt == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請提供欲刪除的日期與時間" });

      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無法取得使用者資訊" });

      var therapistOpenTime = await _context.TherapistOpenTimes
          .FirstOrDefaultAsync(a => a.UserId == userId && a.StartDt == startDt!.Value);

      if (therapistOpenTime == null)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不到指定的治療師開放時間" });

      _context.TherapistOpenTimes.Remove(therapistOpenTime);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
  public class TherapistOpenTimeDto
  {
    public string UserId { get; set; } = null!;
    public DateTime? StartDt { get; set; }
    public User? User { get; set; } = null!;
    public Calendar? Calendar { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
  }
}