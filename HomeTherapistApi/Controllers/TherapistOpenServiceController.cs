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
  public class TherapistOpenServiceController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public TherapistOpenServiceController(HometherapistContext context)
    {
      _context = context;
    }

    // GET: api/TherapistOpenService
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TherapistOpenService>>> GetTherapistOpenServices()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      return await _context.TherapistOpenServices
          .Where(t => t.UserId == userId)
          .ToListAsync();
    }
    // POST: api/TherapistOpenService
    [HttpPost]
    public async Task<ActionResult<TherapistOpenService>> AddTherapistOpenService(ulong ServiceId)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      var user = await _context.Users.SingleOrDefaultAsync(u => u.StaffId == userId);
      if (user == null) return BadRequest("使用者錯誤。");

      var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == ServiceId);
      if (service == null) return BadRequest("服務ID錯誤。");

      var existingService = await _context.TherapistOpenServices
         .FirstOrDefaultAsync(s => s.ServiceId == ServiceId && s.UserId == userId);
      if (existingService != null)
        return Conflict("已啟用該服務。");

      var therapistOpenService = new TherapistOpenService
      {
        UserId = userId,
        ServiceId = ServiceId,
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
      }, therapistOpenService);
    }

    // DELETE: api/TherapistOpenService/5
    [HttpDelete]
    public async Task<IActionResult> DeleteTherapistOpenService(ulong? serviceId)
    {
      if (serviceId == null)
        return BadRequest("需要輸入欲刪除服務ID。");

      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      var therapistOpenService = await _context.TherapistOpenServices
      .FirstOrDefaultAsync(a => a.UserId == userId && a.ServiceId == serviceId);

      if (therapistOpenService == null)
        return NotFound();

      _context.TherapistOpenServices.Remove(therapistOpenService);
      await _context.SaveChangesAsync();

      return NoContent();
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
