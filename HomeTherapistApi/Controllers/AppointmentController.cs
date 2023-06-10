using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using HomeTherapistApi.Services;
using HomeTherapistApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTherapistApi.Controllers
{
  // 面向消費者
  [ApiController]
  [Route("[controller]")]
  public class AppointmentController : ControllerBase
  {
    private readonly HometherapistContext _context;
    private readonly IAppointmentService _appointmentService;
    private readonly IEmailService _emailService;

    public AppointmentController(HometherapistContext context, IAppointmentService appointmentService, IEmailService emailService)
    {
      _appointmentService = appointmentService;
      _context = context;
      _emailService = emailService;
    }

    [HttpGet("{IdNumber}")]
    public async Task<ActionResult<ApiResponse<Appointment>>> Get(string IdNumber, string Phone)
    {
      var appointments = await _context.Appointments
          .Include(a => a.AppointmentDetails)
              .ThenInclude(ad => ad.Service)
          .Where(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone)
          .Select(a => new
          {
            a.Id,
            a.StartDt,
            a.CustomerId,
            a.CustomerPhone,
            a.CustomerAddress,
            a.Gender,
            a.AgeGroup,
            AppointmentDetails = a.AppointmentDetails.Select(ad => new
            {
              ad.ServiceId,
              ad.Price,
              ad.Note,
              Service = new
              {
                ad.Service.Id,
                ad.Service.Name,
                ad.Service.Price,
              }
            }).ToList(),
            User = new
            {
              a.User.UserName,
            },
          })
          .ToListAsync();

      if (appointments.Count == 0)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = $"查詢身分證字號為 '{IdNumber}' 且電話號碼為 '{Phone}' 的預約失敗。" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"查詢身分證字號為 '{IdNumber}' 且電話號碼為 '{Phone}' 的預約已完成。", Data = appointments });
    }


    [HttpPatch("{IdNumber}")]
    public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] JsonPatchDocument<AppointmentUpdateDto> patchDocument, string IdNumber, [FromQuery] string Phone, [FromQuery] DateTime? date)
    {
      var appointment = await _context.Appointments
          .Include(a => a.AppointmentDetails)
          .FirstOrDefaultAsync(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone && a.StartDt == date!.Value);

      if (appointment == null)
        return NotFound(new ApiResponse<string> { IsSuccess = false, Message = $"更新身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約失敗。" });

      var appointmentDto = new AppointmentUpdateDto
      {
        CustomerId = appointment.CustomerId,
        CustomerPhone = appointment.CustomerPhone,
        CustomerAddress = appointment.CustomerAddress,
        Gender = appointment.Gender,
        AgeGroup = appointment.AgeGroup,
        Note = appointment.AppointmentDetails.FirstOrDefault()?.Note
      };

      patchDocument.ApplyTo(appointmentDto, ModelState);

      if (!ModelState.IsValid)
      {
        var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = $"提供的資料無效，無法更新身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約。", Data = errors });
      }

      appointment.CustomerId = appointmentDto.CustomerId;
      appointment.CustomerPhone = appointmentDto.CustomerPhone;
      appointment.CustomerAddress = appointmentDto.CustomerAddress;
      appointment.Gender = appointmentDto.Gender;
      appointment.AgeGroup = appointmentDto.AgeGroup;
      appointment.AppointmentDetails.FirstOrDefault()!.Note = appointmentDto.Note;

      appointment.UpdatedAt = DateTime.UtcNow;

      await _context.SaveChangesAsync();

      return Ok(new ApiResponse<string> { IsSuccess = true, Message = $"更新身分證字號為 '{IdNumber}'、電話號碼為 '{Phone}' 且日期為 '{date}' 的預約已完成。" });
    }
    // [
    //   {
    //     "op": "replace",
    //     "path": "/customerId",
    //     "value": "新身分證字號"
    //   },
    //   {
    //     "op": "replace",
    //     "path": "/customerPhone",
    //     "value": "新電話號碼"
    //   },
    //   {
    //   "op": "replace",
    //     "path": "/customerAddress",
    //     "value": "新地址"
    //   },
    //   {
    //   "op": "replace",
    //     "path": "/gender",
    //     "value": "新性別"
    //   },
    //   {
    //   "op": "replace",
    //     "path": "/ageGroup",
    //     "value": "新年齡層"
    //   },
    //   {
    //   "op": "replace",
    //     "path": "/note",
    //     "value": "新備註"
    //   }
    // ]


    [HttpDelete("{IdNumber}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(string IdNumber, string Phone, DateTime? date)
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
    public class AppointmentUpdateDto
    {
      public string CustomerId { get; set; } = null!;
      public string CustomerPhone { get; set; } = null!;
      public string CustomerAddress { get; set; } = null!;
      public string? Gender { get; set; }
      public string? AgeGroup { get; set; }
      public string Note { get; set; }
    }


  }

}
