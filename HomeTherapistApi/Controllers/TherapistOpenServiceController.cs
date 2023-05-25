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
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (userId == null) return BadRequest();

      return await _context.TherapistOpenServices
          .Where(t => t.UserId == userId)
          .ToListAsync();
    }
    // {
    //   "UserId": "T5106",
    //   "ServiceId": "1"
    // }
    // POST: api/TherapistOpenService
    [HttpPost]
    public async Task<ActionResult<TherapistOpenService>> PostTherapistOpenService(TherapistOpenServiceDto therapistOpenServiceDto)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      var user = await _context.Users.SingleOrDefaultAsync(u => u.StaffId == userId);
      if (user == null) return BadRequest("使用者錯誤。");

      var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == therapistOpenServiceDto.ServiceId);
      if (service == null) return BadRequest("服務ID錯誤。");

      var therapistOpenService = new TherapistOpenService
      {
        UserId = userId,
        ServiceId = (ulong)therapistOpenServiceDto.ServiceId,
        User = user,
        Service = service
      };

      _context.TherapistOpenServices.Add(therapistOpenService);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTherapistOpenServices), new { id = therapistOpenService.Id }, therapistOpenService);
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
  }
}
