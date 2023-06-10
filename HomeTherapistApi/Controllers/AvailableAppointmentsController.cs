using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using HomeTherapistApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HomeTherapistApi.Utilities;

namespace HomeTherapistApi.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class AvailableAppointmentsController : ControllerBase
  {
    private readonly HometherapistContext _context;
    private readonly IAppointmentService _appointmentService;
    private readonly IEmailService _emailService;
    private List<User>? _therapistsInRange;

    public AvailableAppointmentsController(HometherapistContext context, IAppointmentService appointmentService, IEmailService emailService)
    {
      _context = context;
      _appointmentService = appointmentService;
      _emailService = emailService;

    }

    [HttpGet]
    [Route("getAvailableDays")]
    public async Task<IActionResult> GetAvailableDays(double? latitude, double? longitude, ulong serviceId, DateTime? date)
    {
      try
      {
        ValidateInput(latitude, longitude, date, "經緯度、月份為必要資訊。");

        var therapistsInRange = await GetTherapistsInRange(latitude, longitude);
        var availableDays = await GetAvailableDaysForTherapists(therapistsInRange, date!.Value);

        return Ok(new ApiResponse<List<DateTime>> { IsSuccess = true, Data = availableDays.Distinct().OrderBy(day => day).ToList() });
      }
      catch (Exception ex)
      {
        return Ok(new ApiResponse<string> { IsSuccess = false, Message = ex.Message });
      }
    }

    [HttpGet]
    [Route("getAvailableDatetime")]
    public async Task<IActionResult> GetAvailableDatetime(double? latitude, double? longitude, ulong serviceId, DateTime? date)
    {
      try
      {
        ValidateInput(latitude, longitude, date, "經緯度、日期為必要資訊。");

        var therapistsInRange = await GetTherapistsInRange(latitude, longitude);
        var availableDatetimes = await GetAvailableDatetimesForTherapists(therapistsInRange, date!.Value);

        return Ok(new ApiResponse<List<DateTime>> { IsSuccess = true, Data = availableDatetimes });
      }
      catch (Exception ex)
      {
        return Ok(new ApiResponse<string> { IsSuccess = false, Message = ex.Message });
      }
    }
    [HttpPost]
    [Route("createAppointment")]
    public async Task<IActionResult> CreateAppointmentAsync([FromBody] AppointmentInputDto appointmentInputDto)
    {
      try
      {
        var validGenders = new List<string> { "男", "女", "其他" };
        if (!validGenders.Contains(appointmentInputDto.Gender))
          return Ok(new ApiResponse<string> { IsSuccess = false, Message = appointmentInputDto.Gender + "是無效的性別值。" });
        var validAgeGroups = new List<string> { "小於18", "18-25", "26-35", "36-45", "46-55", "56-65", "66-75", "大於75" };
        if (!validAgeGroups.Contains(appointmentInputDto.AgeGroup))
          return Ok(new ApiResponse<string> { IsSuccess = false, Message = appointmentInputDto.AgeGroup + "是無效的年齡組別值。" });

        var UserId = await GetRandomTherapistId(appointmentInputDto.SelectedDate, appointmentInputDto.Latitude, appointmentInputDto.Longitude);
        var appointmentDto = new AppointmentDto
        {
          ServiceId = appointmentInputDto.ServiceId,
          CustomerId = appointmentInputDto.CustomerId,
          CustomerPhone = appointmentInputDto.CustomerPhone,
          CustomerAddress = appointmentInputDto.CustomerAddress,
          Note = appointmentInputDto.Note,
          UserId = UserId,
          Price = (double)(_context.Services.FirstOrDefault(a => a.Id == appointmentInputDto.ServiceId)?.Price!),
          StartDt = appointmentInputDto.SelectedDate,
          Latitude = (decimal)appointmentInputDto.Latitude!,
          Longitude = (decimal)appointmentInputDto.Longitude!,
          Gender = appointmentInputDto.Gender,
          AgeGroup = appointmentInputDto.AgeGroup
        };

        var appointment = _appointmentService.CreateAppointmentWithDetail(appointmentDto);
        var therapistEmail = _context.Users.First(u => u.StaffId == UserId).Email!.ToString();
        var isEmailSend = _emailService.SendAppointmentEmail(appointmentDto, therapistEmail);

        return Ok(new ApiResponse<Appointment> { IsSuccess = true, Data = appointment });
      }
      catch (ArgumentException ex)
      {
        return Ok(new ApiResponse<string> { IsSuccess = false, Message = ex.Message });
      }
    }

    private async Task<List<DateTime>> GetAvailableDatetimesForTherapists(List<User> therapistsInRange, DateTime date)
    {
      var availableDatetimes = new List<DateTime>();

      foreach (var therapist in therapistsInRange)
      {
        var openDatetimes = await GetAvailableDatetimesForTherapist(therapist, date);
        availableDatetimes.AddRange(openDatetimes);
      }

      return availableDatetimes;
    }

    private async Task<List<DateTime>> GetAvailableDatetimesForTherapist(User therapist, DateTime date)
    {
      var openTimes = await _context.TherapistOpenTimes
          .Where(time => time.UserId == therapist.StaffId && time.StartDt.HasValue && time.StartDt.Value.Date == date.Date)
          .OrderBy(time => time.StartDt)
          .ToListAsync();

      var availableDatetimes = new List<DateTime>();

      if (openTimes == null)
        return availableDatetimes;

      foreach (var openTime in openTimes)
      {
        // 排除已被預約的時間
        var overlappingAppointments = await _context.Appointments
            .Where(a => a.UserId == therapist.StaffId && a.StartDt >= openTime.StartDt && a.StartDt < openTime.StartDt.Value.AddHours(3))
            .ToListAsync();

        var timeSlotStart = openTime.StartDt!.Value;
        while (timeSlotStart < openTime.StartDt.Value.AddHours(3))
        {
          if (!overlappingAppointments.Any(a => a.StartDt == timeSlotStart))
          {
            availableDatetimes.Add(timeSlotStart);
          }

          timeSlotStart = timeSlotStart.AddHours(1);
        }
      }

      return availableDatetimes;
    }
    private async Task<string> GetRandomTherapistId(DateTime selectedDate, double? latitude, double? longitude)
    {
      var therapistsInRange = await _GetTherapistsInRange(latitude, longitude);
      var availableAppointments = await GetAvailableAppointmentsForTherapists(therapistsInRange, selectedDate);

      if (availableAppointments.Count == 0)
        throw new Exception("該時間段沒有該放預約的治療師。");

      var random = new Random();
      var randomAppointment = availableAppointments[random.Next(availableAppointments.Count)];

      return randomAppointment.Therapist.StaffId;
    }
    private static void ValidateInput(double? latitude, double? longitude, DateTime? date, string errorMessage)
    {
      if (latitude == null || longitude == null || !date.HasValue)
        throw new ArgumentNullException(errorMessage);
    }
    private async Task<List<User>> _GetTherapistsInRange(double? latitude, double? longitude)
    {
      if (_therapistsInRange == null)
        _therapistsInRange = await GetTherapistsInRange(latitude, longitude);

      return _therapistsInRange;
    }

    // private async Task<List<User>> GetTherapistsInRange(double? latitude, double? longitude)
    // {
    //   const double EarthRadiusInKm = 6371.0;

    //   var therapistsInRange = await _context.Users
    //       .Where(user => user.Latitude != null && user.Longitude != null && user.Radius != null)
    //       .ToListAsync();

    //   var lat = latitude * Math.PI / 180.0;
    //   var lon = longitude * Math.PI / 180.0;

    //   therapistsInRange = therapistsInRange
    //       .Where(user =>
    //       {
    //         var dLat = (double)user.Latitude!.Value * Math.PI / 180.0;
    //         var dLon = (double)user.Longitude!.Value * Math.PI / 180.0;

    //         var sdlat = Math.Sin((double)((dLat! - lat!) / 2));
    //         var sdlon = Math.Sin((double)((dLon! - lon!) / 2));
    //         var q = sdlat * sdlat + Math.Cos((double)lat!) * Math.Cos(dLat) * sdlon * sdlon;
    //         var distance = 2 * EarthRadiusInKm * Math.Asin(Math.Sqrt(q));

    //         return distance <= user.Radius!.Value / 3;
    //       })
    //       .ToList();

    //   return therapistsInRange;
    // }

    private async Task<List<User>> GetTherapistsInRange(double? latitude, double? longitude)
    {
      // 不正確，但可以展示看起來更正確的結果
      const double EarthRadiusInKm = 6371.0;
      //以半正矢公式計算兩個經緯度之間的距離。
      var therapistsInRange = await _context.Users
          .Where(user => user.Latitude != null && user.Longitude != null && user.Radius != null)
          .Where(user => (2 * EarthRadiusInKm * Math.Asin(Math.Sqrt(
              Math.Pow(Math.Sin(((double)user.Latitude! - (double)latitude!) / 2), 2) +
              Math.Cos((double)user.Latitude!) * Math.Cos((double)latitude) * Math.Pow(Math.Sin(((double)user.Longitude! - (double)longitude!) / 2), 2)
          ))) <= user.Radius!.Value * 3)
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
          .Select(time => time.StartDt!.Value.Date)
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

    private async Task<List<AvailableAppointmentDto>> GetAvailableAppointmentsForTherapists(List<User> therapistsInRange, DateTime date)
    {
      var availableAppointments = new List<AvailableAppointmentDto>();

      foreach (var therapist in therapistsInRange)
      {
        var appointmentsForTherapist = await GetAvailableAppointmentsForTherapist(therapist, date);
        availableAppointments.AddRange(appointmentsForTherapist);
      }

      return availableAppointments;
    }

    private async Task<List<AvailableAppointmentDto>> GetAvailableAppointmentsForTherapist(User therapist, DateTime date)
    {
      var openTimes = await _context.TherapistOpenTimes
          .Where(time => time.UserId == therapist.StaffId && time.StartDt.HasValue && time.StartDt.Value.Date == date.Date)
          .OrderBy(time => time.StartDt)
          .ToListAsync();

      var availableAppointments = new List<AvailableAppointmentDto>();

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
            availableAppointments.Add(new AvailableAppointmentDto
            {
              StartDt = timeSlotStart,
              EndDt = timeSlotStart.AddHours(1),
              Therapist = therapist
            });
          }

          timeSlotStart = timeSlotStart.AddHours(1);
        }
      }

      return availableAppointments;
    }
    public class AppointmentInputDto
    {
      public ulong ServiceId { get; set; }
      public string CustomerId { get; set; }
      public string CustomerPhone { get; set; }
      public string CustomerAddress { get; set; }
      public string Note { get; set; }
      public DateTime SelectedDate { get; set; }
      public double? Latitude { get; set; }
      public double? Longitude { get; set; }
      public string Gender { get; set; }
      public string AgeGroup { get; set; }
    }

    public class AvailableAppointmentDto
    {
      public DateTime StartDt { get; set; }
      public DateTime EndDt { get; set; }
      public TherapistOpenService? TherapistOpenService { get; set; }
      public User? Therapist { get; set; }
    }
  }


}

