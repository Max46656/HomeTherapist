using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeTherapistApi.Models;
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
    public async Task<ActionResult<IEnumerable<TherapistOpenTime>>> GetTherapistOpenTimes()
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null) return BadRequest();

      return await _context.TherapistOpenTimes
          .Where(t => t.UserId == userId)
          .ToListAsync();
    }
    // {
    //   "UserId": "T5106",
    //   "StartDt": "2023-01-14T09:00:00"
    // }
    // POST: api/TherapistOpenTime
    [HttpPost]
    public async Task<ActionResult<TherapistOpenTime>> PostTherapistOpenTime(TherapistOpenTimeDto therapistOpenTimeDto)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      // 根據 UserId 查詢相應的 User 物件
      var user = await _context.Users.SingleOrDefaultAsync(u => u.StaffId == userId);
      if (user == null) return BadRequest("使用者錯誤。");

      // 根據 StartDt 查詢相應的 Calendar 物件
      var calendar = await _context.Calendars.SingleOrDefaultAsync(c => c.Dt == therapistOpenTimeDto.StartDt);
      if (calendar == null) return BadRequest("可設定時間以10分鐘為刻度。");

      // 建立 TherapistOpenTime 物件並設定屬性值
      var therapistOpenTime = new TherapistOpenTime
      {
        UserId = userId,
        StartDt = therapistOpenTimeDto.StartDt,
        User = user,
        Calendar = calendar
      };

      _context.TherapistOpenTimes.Add(therapistOpenTime);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTherapistOpenTimes), new { id = therapistOpenTime.Id }, therapistOpenTime);
    }
    // DELETE: api/TherapistOpenTime/5
    [HttpDelete]
    public async Task<IActionResult> DeleteTherapistOpenTime(DateTime? date)
    {
      if (date == null)
        return BadRequest("需要輸入欲刪除日期。");

      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      var therapistOpenTime = await _context.TherapistOpenTimes
      .FirstOrDefaultAsync(a => a.UserId == userId && a.StartDt == date!.Value);

      if (therapistOpenTime == null)
        return NotFound();

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
  }
}