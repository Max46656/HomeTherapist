using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTherapistApi.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace HomeTherapistApi.Services
{
  public interface IEmailService
  {
    bool SendAppointmentEmail(AppointmentDto appointmentDto, string therapistEmail);
  }
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;
    private readonly HometherapistContext _context;
    public EmailService(IConfiguration configuration, HometherapistContext context)
    {
      _configuration = configuration;
      _context = context;
    }

    public bool SendAppointmentEmail(AppointmentDto appointmentDto, string therapistEmail)
    {
      try
      {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("寄件人姓名", "寄件人郵箱地址"));
        emailMessage.To.Add(new MailboxAddress("收件人姓名", therapistEmail));
        emailMessage.Subject = "您在HomeTherapist平台上有新的預約";

        var googleMapUrl = $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(appointmentDto.CustomerAddress)}";

        var service = _context.Services.First(s => s.Id == appointmentDto.ServiceId).Name;

        // 構建郵件內容，包含預約和預約明細的相關內容
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $"<h1>預約詳情</h1>" +
            $"<p>預約時間：{appointmentDto.StartDt}</p>" +
            $"<p>服務內容：{service}</p>" +
            $"<p>顧客手機：{appointmentDto.CustomerPhone}</p>" +
            $"<p>服務地點：<a href=\"{googleMapUrl}\">查看地圖</a></p>" +
            $"<p>顧客備註：{appointmentDto.Note}/p>";

        emailMessage.Body = bodyBuilder.ToMessageBody();

        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"]);
        var smtpUsername = emailSettings["SmtpUsername"];
        var smtpPassword = emailSettings["SmtpPassword"];

        using (var client = new SmtpClient())
        {
          client.Connect(smtpServer, smtpPort, false);
          client.Authenticate(smtpUsername, smtpPassword);
          client.Send(emailMessage);
          client.Disconnect(true);
        }

        return true; // 郵件發送成功
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }

}