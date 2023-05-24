using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
      ValidateInput(latitude, longitude, date, "經緯度、月份為必要資訊。");

      var therapistsInRange = await GetTherapistsInRange(latitude, longitude);
      var availableDays = await GetAvailableDaysForTherapists(therapistsInRange, date.Value);

      return availableDays.Distinct().OrderBy(day => day).ToList();
    }

    [HttpGet]
    [Route("availabledDatetimes")]
    public async Task<List<AvailableAppointment>> GetAvailableDatetime(double? latitude, double? longitude, ulong serviceId, DateTime? date)
    {
      ValidateInput(latitude, longitude, date, "經緯度、日期為必要資訊。");

      var therapistsInRange = await GetTherapistsInRange(latitude, longitude);
      var availabledDatetimes = await GetAvailableAppointmentsForTherapists(therapistsInRange, date.Value);

      return availabledDatetimes;
    }

    private static void ValidateInput(double? latitude, double? longitude, DateTime? date, string errorMessage)
    {
      if (latitude == null || longitude == null || !date.HasValue)
        throw new ArgumentNullException(errorMessage);
    }

    private async Task<List<User>> GetTherapistsInRange(double? latitude, double? longitude)
    {
      const double EarthRadiusInKm = 6371.0;
      //以半正矢公式計算兩個經緯度之間的距離。
      var therapistsInRange = await _context.Users
          .Where(user => user.Latitude != null && user.Longitude != null && user.Radius != null)
          .Where(user => (2 * EarthRadiusInKm * Math.Asin(Math.Sqrt(
              Math.Pow(Math.Sin(((double)user.Latitude! - (double)latitude) / 2), 2) +
              Math.Cos((double)user.Latitude!) * Math.Cos((double)latitude) * Math.Pow(Math.Sin(((double)user.Longitude! - (double)longitude) / 2), 2)
          ))) <= user.Radius!)
          .ToListAsync();

      return therapistsInRange;
    }

    private async Task<List<DateTime>> GetAvailableDaysForTherapists(List<User> therapistsInRange, DateTime date)
    {
      var availableDays = new List<DateTime>();

      var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
      var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

      foreach (var therapist in therapistsInRange)
      {
        var openDays = await GetOpenDaysForTherapist(therapist, firstDayOfMonth, lastDayOfMonth);
        availableDays.AddRange(openDays);
      }
      return availableDays;
    }

    private async Task<List<DateTime>> GetOpenDaysForTherapist(User therapist, DateTime firstDayOfMonth, DateTime lastDayOfMonth)
    {
      var openDays = await _context.TherapistOpenTimes
          .Where(time => time.UserId == therapist.StaffId && time.StartDt.HasValue && time.StartDt.Value.Date >= firstDayOfMonth && time.StartDt.Value.Date <= lastDayOfMonth)
          .Select(time => time.StartDt.Value.Date)
          .Distinct()
          .ToListAsync();

      var availableDays = new List<DateTime>();
      foreach (var openDay in openDays)
      {
        // 排除已被預約的日期
        var overlappingAppointments = await _context.Appointments
            .Where(a => a.UserId == therapist.StaffId && a.StartDt >= openDay && a.StartDt < openDay.AddDays(1))
            .ToListAsync();

        if (!overlappingAppointments.Any())
          availableDays.Add(openDay);
      }

      return availableDays;
    }

    private async Task<List<AvailableAppointment>> GetAvailableAppointmentsForTherapists(List<User> therapistsInRange, DateTime date)
    {
      var availableAppointments = new List<AvailableAppointment>();

      foreach (var therapist in therapistsInRange)
      {
        var appointmentsForTherapist = await GetAvailableAppointmentsForTherapist(therapist, date);
        availableAppointments.AddRange(appointmentsForTherapist);
      }

      return availableAppointments;
    }

    private async Task<List<AvailableAppointment>> GetAvailableAppointmentsForTherapist(User therapist, DateTime date)
    {
      var openTimes = await _context.TherapistOpenTimes
          .Where(time => time.UserId == therapist.StaffId && time.StartDt.HasValue && time.StartDt.Value.Date == date.Date)
          .OrderBy(time => time.StartDt)
          .ToListAsync();

      var availableAppointments = new List<AvailableAppointment>();

      if (openTimes == null)
        return availableAppointments;

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

      return availableAppointments;
    }
  }


}
public class AvailableAppointment
{
  public DateTime StartDt { get; set; }
  public DateTime EndDt { get; set; }
  public TherapistOpenService? TherapistOpenService { get; set; }
}
