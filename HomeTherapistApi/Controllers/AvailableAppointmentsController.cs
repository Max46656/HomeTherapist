using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;


namespace HomeTherapistApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AvailableAppointmentsController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public AvailableAppointmentsController(HometherapistContext context)
    {
      _context = context;
    }
    [HttpGet]
    [Route("getAvailableDays")]
    public async Task<List<DateTime>> GetAvailableDays(double? latitude, double? longitude, ulong serviceId, DateTime? date)
    {
      if (latitude == null || longitude == null || !date.HasValue)
        throw new ArgumentNullException("經緯度、月份為必要資訊。");

      const double EarthRadiusInKm = 6371.0;

      var availableDays = new List<DateTime>();

      var therapistsInRange = await _context.Users
          .Where(user => user.Latitude != null && user.Longitude != null && user.Radius != null)
          .Where(user => (2 * EarthRadiusInKm * Math.Asin(Math.Sqrt(
              Math.Pow(Math.Sin(((double)user.Latitude! - (double)latitude) / 2), 2) +
              Math.Cos((double)user.Latitude!) * Math.Cos((double)latitude) * Math.Pow(Math.Sin(((double)user.Longitude! - (double)longitude) / 2), 2)
          ))) <= user.Radius!)
          .ToListAsync();

      var firstDayOfMonth = new DateTime(date.Value.Year, date.Value.Month, 1);
      var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

      foreach (var therapist in therapistsInRange)
      {
        var openDays = await _context.TherapistOpenTimes
            .Where(time => time.UserId == therapist.StaffId && time.StartDt.HasValue && time.StartDt.Value.Date >= firstDayOfMonth && time.StartDt.Value.Date <= lastDayOfMonth)
            .Select(time => time.StartDt.Value.Date)
            .Distinct()
            .ToListAsync();

        foreach (var openDay in openDays)
        {
          // 排除已被預約的日期
          var overlappingAppointments = await _context.Appointments
              .Where(a => a.UserId == therapist.StaffId && a.StartDt >= openDay && a.StartDt < openDay.AddDays(1))
              .ToListAsync();

          if (!overlappingAppointments.Any())
            availableDays.Add(openDay);
        }
      }
      return availableDays.Distinct().OrderBy(day => day).ToList();
    }

    [HttpGet]
    [Route("getAvailableAppointments")]
    public async Task<List<AvailableAppointment>> GetAvailableDatetime(double? latitude, double? longitude, ulong serviceId, DateTime? date)
    {
      if (latitude == null || longitude == null || !date.HasValue)
        throw new ArgumentNullException("經緯度、日期為必要資訊。");

      const double EarthRadiusInKm = 6371.0;

      var availableAppointments = new List<AvailableAppointment>();

      var therapistsInRange = await _context.Users
          .Where(user => user.Latitude != null && user.Longitude != null && user.Radius != null)
          .Where(user => (2 * EarthRadiusInKm * Math.Asin(Math.Sqrt(
              Math.Pow(Math.Sin(((double)user.Latitude! - (double)latitude) / 2), 2) +
              Math.Cos((double)user.Latitude!) * Math.Cos((double)latitude) * Math.Pow(Math.Sin(((double)user.Longitude! - (double)longitude) / 2), 2)
          ))) <= user.Radius!)
          .ToListAsync();


      var selectedDate = date.Value.Date;

      foreach (var therapist in therapistsInRange)
      {
        var openTimes = await _context.TherapistOpenTimes
            .Where(time => time.UserId == therapist.StaffId && time.StartDt.HasValue && time.StartDt.Value.Date == selectedDate)
            .OrderBy(time => time.StartDt)
            .ToListAsync();

        if (openTimes == null)
          continue;

        foreach (var openTime in openTimes)
        {
          // 排除已被預約的治療師
          var overlappingAppointments = await _context.Appointments
              .Where(a => a.UserId == therapist.StaffId && a.StartDt >= openTime.StartDt && a.StartDt < openTime.StartDt.Value.AddHours(3))
              .ToListAsync();

          var timeSlotStart = openTime.StartDt!.Value;
          while (timeSlotStart < openTime.StartDt.Value.AddHours(3))
          {
            if (!overlappingAppointments.Any(a => a.StartDt == timeSlotStart))
            {
              availableAppointments.Add(new AvailableAppointment
              {
                StartDt = timeSlotStart,
                EndDt = timeSlotStart.AddHours(1)
              });
            }

            timeSlotStart = timeSlotStart.AddHours(1);
          }
        }
      }
      return availableAppointments;
    }

  }
  public class AvailableAppointment
  {
    public DateTime StartDt { get; set; }
    public DateTime EndDt { get; set; }
    public TherapistOpenService? TherapistOpenService { get; set; }
  }
}