using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using HomeTherapistApi.Services;
using HomeTherapistApi.Utilities;

namespace HomeTherapistApi.Controllers
{
  // 面向消費者
  [ApiController]
  [Route("[controller]")]
  public class AppointmentController : ControllerBase
  {
    private readonly HometherapistContext _context;
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(HometherapistContext context, IAppointmentService appointmentService)
    {
      _appointmentService = appointmentService;
      _context = context;
    }

    [HttpGet("{IdNumber}")]
    public async Task<ActionResult<ApiResponse<object>>> Get(string IdNumber, string Phone)
    {
      var appointments = await _context.Appointments
          .Include(a => a.AppointmentDetails)
              .ThenInclude(ad => ad.Service)
          .Where(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone)
          .Select(a => new
          {
            Appointment = a,
            AppointmentDetails = a.AppointmentDetails.Select(ad => new
            {
              AppointmentDetail = ad,
              Service = ad.Service
            }).ToList(),
            User = a.User,
            Calendar = a.Calendar
          })
          .ToListAsync();

      if (appointments.Count == 0)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = $"查詢身分證字號為 '{IdNumber}' 且電話號碼為 '{Phone}' 的預約失敗。" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"查詢身分證字號為 '{IdNumber}' 且電話號碼為 '{Phone}' 的預約已完成。", Data = appointments });

    }

    [HttpPatch("{IdNumber}")]
    public async Task<IActionResult> Update([FromBody] JsonPatchDocument<Appointment> patchDocument, string IdNumber, [FromQuery] string Phone, [FromQuery] DateTime? date)
    {
      var appointment = await _context.Appointments
          .Include(a => a.AppointmentDetails)
          .FirstOrDefaultAsync(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone && a.StartDt == date!.Value);

      if (appointment == null)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = $"更新身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約失敗。" });

      patchDocument.ApplyTo(appointment, ModelState);

      if (!ModelState.IsValid)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = $"提供的資料無效，無法更新身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約。" });

      await _context.SaveChangesAsync();

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"更新身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約已完成。" });
    }

    [HttpDelete("{IdNumber}")]
    public async Task<IActionResult> Delete(string IdNumber, string Phone, DateTime? date)
    {
      var appointment = await _context.Appointments
          .Include(a => a.AppointmentDetails)
          .FirstOrDefaultAsync(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone && a.StartDt == date!.Value);

      if (appointment == null)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = $"刪除身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約失敗。" });

      _context.AppointmentDetails.RemoveRange(appointment.AppointmentDetails);
      _context.Appointments.Remove(appointment);
      await _context.SaveChangesAsync();

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"刪除身分證字號為為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約已完成。" });
    }
  }

}
