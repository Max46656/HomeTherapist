using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HomeTherapistApi.Models;


namespace HomeTherapistApi.Services
{
  public interface IAppointmentService
  {
    Appointment CreateAppointmentWithDetail(AppointmentDto appointmentDto);
  }

  public class AppointmentService : IAppointmentService
  {
    private readonly HometherapistContext _context;

    public AppointmentService(HometherapistContext context)
    {
      _context = context;
    }
    public Appointment CreateAppointmentWithDetail(AppointmentDto appointmentDto)
    {
      var missingFields = new List<string>();

      missingFields.AddRange(appointmentDto switch
      {
        { UserId: null or "" } => new[] { "治療師" },
        { StartDt: null } => new[] { "預約時間" },
        { CustomerId: null or "" } => new[] { "身份證字號" },
        { CustomerPhone: null or "" } => new[] { "手機" },
        { CustomerAddress: null or "" } => new[] { "地址" },
        { Latitude: 0 } => new[] { "緯度" },
        { Longitude: 0 } => new[] { "經度" },
        { ServiceId: 0 } => new[] { "服務內容" },
        { Price: 0 } => new[] { "價格" },
        _ => Array.Empty<string>()
      });

      var errorMessage = "缺少以下必要字段: " + string.Join(", ", missingFields);

      if (missingFields.Count > 0)
        throw new ArgumentException(errorMessage);

      if (ValidatorService.ValidateTaiwanId(appointmentDto.CustomerId))
        throw new ArgumentException("身份證字號錯誤。");

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
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      var appointmentDetail = new AppointmentDetail
      {
        ServiceId = appointmentDto.ServiceId,
        Price = appointmentDto.Price,
        Note = appointmentDto.Note,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      // 互相關連
      appointment.AppointmentDetails.Add(appointmentDetail);
      appointmentDetail.Appointment = appointment;

      _context.Appointments.Add(appointment);
      _context.SaveChanges();

      return appointment;
    }

  }
  public class AppointmentDto
  {
    [Required]
    public string UserId { get; set; }
    [Required]
    public DateTime? StartDt { get; set; }
    [Required]
    public string CustomerId { get; set; }
    [Required]
    public string CustomerPhone { get; set; }
    [Required]
    public string CustomerAddress { get; set; }
    [Required]
    public decimal Latitude { get; set; }
    [Required]
    public decimal Longitude { get; set; }
    [Required]
    public ulong ServiceId { get; set; }
    [Required]
    public double Price { get; set; }
    public string Note { get; set; }
  }

}