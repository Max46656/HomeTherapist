using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace HomeTherapistApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AppointmentController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public AppointmentController(HometherapistContext context)
    {
      _context = context;
    }

    [HttpPost]
    public IActionResult Create([FromBody] AppointmentDto appointmentDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var appointment = new Appointment
      {
        UserId = appointmentDto.UserId,
        StartDt = appointmentDto.StartDt,
        CustomerId = appointmentDto.CustomerId,
        CustomerPhone = appointmentDto.CustomerPhone,
        CustomerAddress = appointmentDto.CustomerAddress,
        Latitude = appointmentDto.Latitude,
        Longitude = appointmentDto.Longitude,

        IsComplete = false,
        CreatedAt = DateTime.Now
      };

      _context.Appointments.Add(appointment);
      _context.SaveChanges();

      return Ok(appointment);
    }


    [HttpGet("{IdNumber}")]
    public async Task<IActionResult> Get(string IdNumber, string Phone)
    {
      var appointments = await _context.Appointments.Where(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone).ToListAsync();
      if (appointments.Count == 0)
        return NotFound();

      return Ok(appointments);
    }
    [HttpPut("{IdNumber}")]
    public async Task<IActionResult> Update(string IdNumber, string Phone, Appointment updatedAppointment)
    {
      if (IdNumber != updatedAppointment.CustomerId)
        return BadRequest();

      var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.CustomerId == IdNumber && a.CustomerPhone == Phone);
      if (appointment == null)
        return NotFound();

      appointment.CustomerId = updatedAppointment.CustomerId;
      appointment.CustomerPhone = updatedAppointment.CustomerPhone;
      appointment.CustomerAddress = updatedAppointment.CustomerAddress;
      appointment.Longitude = updatedAppointment.Longitude;
      appointment.Latitude = updatedAppointment.Latitude;
      appointment.StartDt = updatedAppointment.StartDt;

      await _context.SaveChangesAsync();
      return NoContent();
    }
    [HttpGet("{id}/Appointmentdetails")]
    public async Task<IActionResult> GetAppointmentDetails(ulong id)
    {
      var AppointmentDetails = await _context.AppointmentDetails
          .Where(od => od.AppointmentId == (ulong)id)
          .ToListAsync();

      return Ok(AppointmentDetails);
    }
  }
  public class AppointmentDto
  {
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public DateTime StartDt { get; set; }
    [Required]
    public string? CustomerId { get; set; }

    [Required]
    public string? CustomerPhone { get; set; }

    [Required]
    public string? CustomerAddress { get; set; }

    [Required]
    public decimal Latitude { get; set; }

    [Required]
    public decimal Longitude { get; set; }
  }

}
