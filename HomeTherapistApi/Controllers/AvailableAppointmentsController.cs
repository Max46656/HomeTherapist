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
using System.Diagnostics;

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

      return randomAppointment.Therapist!.StaffId;
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
    private async Task<List<User>> GetTherapistsInRange(double? latitude, double? longitude)
    {
      int batchSize = 1000;
      int totalBatches = (int)Math.Ceiling((double)_context.Users.Count() / batchSize);

      var therapistsInRange = new List<User>();

      for (int i = 0; i < totalBatches; i++)
      {
        var usersBatch = await _context.Users
            .Where(user => user.Latitude != null && user.Longitude != null && user.Radius != null)
            .Skip(i * batchSize).Take(batchSize)
            .ToListAsync();

        therapistsInRange.AddRange(usersBatch.Where(user => Haversine((double)latitude!, (double)longitude!, (double)user.Latitude!, (double)user!.Longitude!) <= user.Radius!.Value));
      }
      return therapistsInRange;
    }
    private static double Haversine(double latitude1, double longitude1, double latitude2, double longitude2)
    {
      const double EarthRadiusInKm = 6378.1;

      latitude1 = latitude1 * Math.PI / 180;
      longitude1 = longitude1 * Math.PI / 180;
      latitude2 = latitude2 * Math.PI / 180;
      longitude2 = longitude2 * Math.PI / 180;

      var sdlat = Math.Sin((latitude2 - latitude1) / 2);
      var sdlon = Math.Sin((longitude2 - longitude1) / 2);
      var q = sdlat * sdlat + Math.Cos(latitude1) * Math.Cos(latitude2) * sdlon * sdlon;
      var d = 2 * EarthRadiusInKm * Math.Asin(Math.Sqrt(q));

      return d;
    }

    [HttpGet("GetTherapistsInRange")]
    public async Task<IActionResult> GetTherapistsInRangeTest([FromQuery] double? latitude, [FromQuery] double? longitude)
    {
      var therapistsInRange = await GetTherapistsInRange(latitude, longitude);
      // return Ok(Haversine((double)latitude!, (double)longitude!, 25.0322863, 121.3884860));
      return Ok(therapistsInRange);
    }
    private async Task<List<DateTime>> GetAvailableDaysForTherapists(List<User> therapistsInRange, DateTime date)
    {
      // var stopwatch = new Stopwatch();
      // stopwatch.Start();
      var availableDays = new List<DateTime>();
      var therapistIds = therapistsInRange.Select(t => t.StaffId).ToList();

      var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
      var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

      int batchSize = 5000;
      int totalBatches = (int)Math.Ceiling((double)therapistIds.Count / batchSize);

      for (int i = 0; i < totalBatches; i++)
      {
        var currentBatchIds = therapistIds.Skip(i * batchSize).Take(batchSize);

        var openTimes = await _context.TherapistOpenTimes
            .Where(time => currentBatchIds.Contains(time.UserId) && time.StartDt.HasValue && time.StartDt.Value.Date >= firstDayOfMonth && time.StartDt.Value.Date <= lastDayOfMonth)
            .ToListAsync();

        foreach (var therapist in therapistsInRange)
        {
          var therapistOpenTimes = openTimes.Where(time => time.UserId == therapist.StaffId).ToList();
          var openDays = GetOpenDaysForTherapist(therapist, therapistOpenTimes);
          availableDays.AddRange(openDays);
        }
      }
      // stopwatch.Stop();
      // Console.WriteLine($"GetAvailableDaysForTherapists: {stopwatch.Elapsed.TotalMilliseconds} ms");
      return availableDays;
    }
    private List<DateTime> GetOpenDaysForTherapist(User therapist, List<TherapistOpenTime> openTimes)
    {
      var availableDays = new List<DateTime>();
      foreach (var openTime in openTimes)
      {
        if (openTime.StartDt.HasValue)
        {
          var openDay = openTime.StartDt.Value.Date;
          // 排除已被預約的日期
          var overlappingAppointments = _context.Appointments
              .Where(a => a.UserId == therapist.StaffId && a.StartDt >= openDay && a.StartDt < openDay.AddDays(1))
              .ToList();
          if (!overlappingAppointments.Any())
            availableDays.Add(openDay);
        }
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

