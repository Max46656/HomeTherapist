using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using HomeTherapistApi.Services;

namespace HomeTherapistApi.Controllers
{
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
    // [HttpPost]
    // public ActionResult<Appointment> Create(AppointmentDto appointmentDto) => _appointmentService.CreateAppointmentWithDetail(appointmentDto);
    [HttpGet("{IdNumber}")]
    public async Task<IActionResult> Get(string IdNumber, string Phone)
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
        return NotFound();

      return Ok(appointments);
    }
    [HttpPatch("{IdNumber}")]
    public async Task<IActionResult> Update([FromBody] JsonPatchDocument<Appointment> patchDocument, string IdNumber, [FromQuery] string Phone, [FromQuery] DateTime? date)
    {
      var appointment = await _context.Appointments
     .Include(a => a.AppointmentDetails)
     .FirstOrDefaultAsync(a => a.CustomerId == IdNumber
                               && a.CustomerPhone == Phone
                               && a.StartDt == date!.Value);

      if (appointment == null)
        return NotFound();

      patchDocument.ApplyTo(appointment, ModelState);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      await _context.SaveChangesAsync();

      return NoContent();
    }
    [HttpDelete("{IdNumber}")]
    public async Task<IActionResult> Delete(string IdNumber, string Phone, DateTime? date)
    {
      var appointment = await _context.Appointments
          .Include(a => a.AppointmentDetails)
          .FirstOrDefaultAsync(a => a.CustomerId == IdNumber
                                    && a.CustomerPhone == Phone
                                    && a.StartDt == date!.Value);

      if (appointment == null)
        return NotFound();

      _context.AppointmentDetails.RemoveRange(appointment.AppointmentDetails);
      _context.Appointments.Remove(appointment);
      await _context.SaveChangesAsync();

      return NoContent();
    }

  }

}
