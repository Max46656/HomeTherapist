using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeTherapistApi.Models;

public partial class HometherapistContext : IdentityDbContext<User, Role, ulong>
{
  private readonly IConfiguration? _configuration;
  public HometherapistContext()
  {
  }

  public HometherapistContext(DbContextOptions<HometherapistContext> options, IConfiguration configuration)
      : base(options) =>
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

  // public DbSet<IdentityUser> IdentityUsers { get; set; }

  public virtual DbSet<Appointment> Appointments { get; set; }

  public virtual DbSet<AppointmentDetail> AppointmentDetails { get; set; }

  public virtual DbSet<Article> Articles { get; set; }

  public virtual DbSet<Calendar> Calendars { get; set; }

  public virtual DbSet<FailedJob> FailedJobs { get; set; }

  public virtual DbSet<Feedback> Feedbacks { get; set; }

  public virtual DbSet<Migration> Migrations { get; set; }

  public virtual DbSet<Order> Orders { get; set; }

  public virtual DbSet<OrderDetail> OrderDetails { get; set; }

  public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

  public virtual DbSet<PersonalAccessToken> PersonalAccessTokens { get; set; }

  public virtual DbSet<Service> Services { get; set; }

  public virtual DbSet<TherapistOpenService> TherapistOpenServices { get; set; }

  public virtual DbSet<TherapistOpenTime> TherapistOpenTimes { get; set; }
  // public DbSet<User> Users { get; set; }


  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

      => optionsBuilder.UseMySql(_configuration!.GetConnectionString("DefaultConnection"), ServerVersion.Parse("10.4.27-mariadb"), x => x.UseNetTopologySuite());

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder
        .UseCollation("utf8mb4_general_ci")
        .HasCharSet("utf8mb4");

    modelBuilder.Entity<IdentityUserLogin<ulong>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
    modelBuilder.Entity<IdentityUserRole<ulong>>().HasKey(r => new { r.UserId, r.RoleId });
    modelBuilder.Entity<IdentityUserToken<ulong>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
    modelBuilder.Entity<Appointment>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("appointments")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.StartDt, "appointments_start_dt_foreign");

      entity.HasIndex(e => e.UserId, "appointments_user_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.CustomerAddress)
              .HasMaxLength(255)
              .HasColumnName("customer_address");
      entity.Property(e => e.CustomerId)
              .HasMaxLength(10)
              .HasColumnName("customer_ID");
      entity.Property(e => e.Longitude)
  .HasPrecision(10, 7)
  .HasColumnName("longitude");
      entity.Property(e => e.Latitude)
             .HasPrecision(10, 7)
             .HasColumnName("latitude");
      entity.Property(e => e.CustomerPhone)
              .HasMaxLength(10)
              .HasColumnName("customer_phone");
      entity.Property(e => e.IsComplete).HasColumnName("is_complete");
      entity.Property(e => e.StartDt)
              .HasColumnType("datetime")
              .HasColumnName("start_dt");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
      entity.Property(e => e.UserId).HasColumnName("user_id");

      entity.HasOne(d => d.Calendar).WithMany(p => p.Appointments)
                    .HasForeignKey(e => e.StartDt)
                    .HasPrincipalKey(u => u.Dt)
                   .HasConstraintName("appointments_start_dt_foreign");

      entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                    .HasForeignKey(e => e.UserId)
                    .HasPrincipalKey(u => u.StaffId)
                   .HasConstraintName("appointments_user_id_foreign");
    });

    modelBuilder.Entity<AppointmentDetail>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("appointment_details")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.AppointmentId, "appointment_details_appointment_id_foreign");

      entity.HasIndex(e => e.ServiceId, "appointment_details_service_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.AppointmentId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("appointment_id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.Note)
              .HasMaxLength(255)
              .HasColumnName("note");
      entity.Property(e => e.Price)
              .HasColumnType("double(8,2)")
              .HasColumnName("price");
      entity.Property(e => e.ServiceId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("service_id");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");

      entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentDetails)
              .HasForeignKey(d => d.AppointmentId)
              .HasConstraintName("appointment_details_appointment_id_foreign");

      entity.HasOne(d => d.Service).WithMany(p => p.AppointmentDetails)
              .HasForeignKey(d => d.ServiceId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("appointment_details_service_id_foreign");
    });

    modelBuilder.Entity<Article>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("articles")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.UserId, "articles_user_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.Body)
              .HasColumnType("text")
              .HasColumnName("body");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.Subtitle)
              .HasMaxLength(255)
              .HasColumnName("subtitle");
      entity.Property(e => e.Title)
              .HasMaxLength(255)
              .HasColumnName("title");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
      entity.Property(e => e.UserId).HasColumnName("user_id");

      entity.HasOne(d => d.User).WithMany(p => p.Articles)
              .HasForeignKey(e => e.UserId)
              .HasPrincipalKey(u => u.StaffId)
             .HasConstraintName("articles_user_id_foreign");
    });

    modelBuilder.Entity<Calendar>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("calendar")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => new { e.Year, e.Month, e.Day }, "Ymd");

      entity.HasIndex(e => e.Dt, "calendar_dt_index");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.Date).HasColumnName("date");
      entity.Property(e => e.Day)
              .HasColumnType("tinyint(3) unsigned")
              .HasColumnName("day");
      entity.Property(e => e.DayOfWeek)
              .HasColumnType("tinyint(3) unsigned")
              .HasColumnName("day_of_week");
      entity.Property(e => e.Dt)
              .HasColumnType("datetime")
              .HasColumnName("dt");
      entity.Property(e => e.IsHoliday)
              .HasDefaultValueSql("'0'")
              .HasColumnName("is_holiday");
      entity.Property(e => e.IsWeekend)
              .HasDefaultValueSql("'0'")
              .HasColumnName("is_weekend");
      entity.Property(e => e.Month)
              .HasColumnType("tinyint(3) unsigned")
              .HasColumnName("month");
      entity.Property(e => e.Quarter)
              .HasColumnType("tinyint(3) unsigned")
              .HasColumnName("quarter");
      entity.Property(e => e.WeekOfYear)
              .HasColumnType("tinyint(3) unsigned")
              .HasColumnName("week_of_year");
      entity.Property(e => e.Year)
              .HasColumnType("smallint(5) unsigned")
              .HasColumnName("year");
    });

    modelBuilder.Entity<FailedJob>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("failed_jobs")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.Uuid, "failed_jobs_uuid_unique").IsUnique();

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.Connection)
              .HasColumnType("text")
              .HasColumnName("connection");
      entity.Property(e => e.Exception).HasColumnName("exception");
      entity.Property(e => e.FailedAt)
              .HasDefaultValueSql("current_timestamp()")
              .HasColumnType("timestamp")
              .HasColumnName("failed_at");
      entity.Property(e => e.Payload).HasColumnName("payload");
      entity.Property(e => e.Queue)
              .HasColumnType("text")
              .HasColumnName("queue");
      entity.Property(e => e.Uuid).HasColumnName("uuid");
    });

    modelBuilder.Entity<Feedback>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("feedbacks")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.OrderId, "feedbacks_order_id_foreign");

      entity.HasIndex(e => e.UserId, "feedbacks_user_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.Comments)
              .HasColumnType("text")
              .HasColumnName("comments");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.CustomerId)
              .HasMaxLength(255)
              .HasColumnName("customer_id");
      entity.Property(e => e.OrderId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("order_id");
      entity.Property(e => e.Rating)
              .HasColumnType("int(10) unsigned")
              .HasColumnName("rating");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
      entity.Property(e => e.UserId).HasColumnName("user_id");

      entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
              .HasForeignKey(d => d.OrderId)
              .HasConstraintName("feedbacks_order_id_foreign");

      entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
              .HasForeignKey(e => e.UserId)
              .HasPrincipalKey(u => u.StaffId)
     .HasConstraintName("feedbacks_user_id_foreign");
    });

    modelBuilder.Entity<Migration>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("migrations")
              .UseCollation("utf8mb4_unicode_ci");

      entity.Property(e => e.Id)
              .HasColumnType("int(10) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.Batch)
              .HasColumnType("int(11)")
              .HasColumnName("batch");
      entity.Property(e => e.Migration1)
              .HasMaxLength(255)
              .HasColumnName("migration");
    });

    modelBuilder.Entity<Order>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("orders")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.StartDt, "orders_start_dt_foreign");

      entity.HasIndex(e => e.UserId, "orders_user_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.CustomerAddress)
              .HasMaxLength(255)
              .HasColumnName("customer_address");
      entity.Property(e => e.CustomerId)
              .HasMaxLength(10)
              .HasColumnName("customer_ID");
      entity.Property(e => e.Longitude)
        .HasPrecision(10, 7)
        .HasColumnName("longitude");
      entity.Property(e => e.Latitude)
             .HasPrecision(10, 7)
             .HasColumnName("latitude");
      entity.Property(e => e.CustomerPhone)
              .HasMaxLength(10)
              .HasColumnName("customer_phone");
      entity.Property(e => e.IsComplete).HasColumnName("is_complete");
      entity.Property(e => e.StartDt)
              .HasColumnType("datetime")
              .HasColumnName("start_dt");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
      entity.Property(e => e.UserId).HasColumnName("user_id");

      entity.HasOne(d => d.Calendar).WithMany(p => p.Orders)
                     .HasForeignKey(e => e.StartDt)
                     .HasPrincipalKey(u => u.Dt)
                    .HasConstraintName("orders_start_dt_foreign");

      entity.HasOne(d => d.User).WithMany(p => p.Orders)
              .HasForeignKey(e => e.UserId)
              .HasPrincipalKey(u => u.StaffId)
             .HasConstraintName("orders_user_id_foreign");
    });

    modelBuilder.Entity<OrderDetail>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("order_details")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.OrderId, "order_details_order_id_foreign");

      entity.HasIndex(e => e.ServiceId, "order_details_service_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.Note)
              .HasMaxLength(255)
              .HasColumnName("note");
      entity.Property(e => e.OrderId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("order_id");
      entity.Property(e => e.Price)
              .HasColumnType("double(8,2)")
              .HasColumnName("price");
      entity.Property(e => e.ServiceId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("service_id");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");

      entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
              .HasForeignKey(d => d.OrderId)
              .HasConstraintName("order_details_order_id_foreign");

      entity.HasOne(d => d.Service).WithMany(p => p.OrderDetails)
              .HasForeignKey(d => d.ServiceId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("order_details_service_id_foreign");
    });

    modelBuilder.Entity<PasswordResetToken>(entity =>
    {
      entity.HasKey(e => e.Email).HasName("PRIMARY");

      entity
              .ToTable("password_reset_tokens")
              .UseCollation("utf8mb4_unicode_ci");

      entity.Property(e => e.Email).HasColumnName("email");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.Token)
              .HasMaxLength(255)
              .HasColumnName("token");
    });


    modelBuilder.Entity<PersonalAccessToken>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("personal_access_tokens")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.Token, "personal_access_tokens_token_unique").IsUnique();

      entity.HasIndex(e => new { e.TokenableType, e.TokenableId }, "personal_access_tokens_tokenable_type_tokenable_id_index");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.Abilities)
              .HasColumnType("text")
              .HasColumnName("abilities");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.ExpiresAt)
              .HasColumnType("timestamp")
              .HasColumnName("expires_at");
      entity.Property(e => e.LastUsedAt)
              .HasColumnType("timestamp")
              .HasColumnName("last_used_at");
      entity.Property(e => e.Name)
              .HasMaxLength(255)
              .HasColumnName("name");
      entity.Property(e => e.Token)
              .HasMaxLength(64)
              .HasColumnName("token");
      entity.Property(e => e.TokenableId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("tokenable_id");
      entity.Property(e => e.TokenableType).HasColumnName("tokenable_type");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
    });

    modelBuilder.Entity<Service>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("services")
              .UseCollation("utf8mb4_unicode_ci");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.Enabled)
              .IsRequired()
              .HasDefaultValueSql("'1'")
              .HasColumnName("enabled");
      entity.Property(e => e.Name)
              .HasMaxLength(50)
              .HasColumnName("name");
      entity.Property(e => e.Price)
              .HasColumnType("double(8,2)")
              .HasColumnName("price");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
    });

    modelBuilder.Entity<TherapistOpenService>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("therapist_open_services")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.ServiceId, "therapist_open_services_service_id_foreign");

      entity.HasIndex(e => e.UserId, "therapist_open_services_user_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.ServiceId)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("service_id");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
      entity.Property(e => e.UserId).HasColumnName("user_id");

      entity.HasOne(d => d.Service).WithMany(p => p.TherapistOpenServices)
              .HasForeignKey(d => d.ServiceId)
              .HasConstraintName("therapist_open_services_service_id_foreign");

      entity.HasOne(d => d.User).WithMany(p => p.TherapistOpenServices)
     .HasForeignKey(e => e.UserId)
                    .HasPrincipalKey(u => u.StaffId)
     .HasConstraintName("therapist_open_services_user_id_foreign");
    });

    modelBuilder.Entity<TherapistOpenTime>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PRIMARY");

      entity
              .ToTable("therapist_open_times")
              .UseCollation("utf8mb4_unicode_ci");

      entity.HasIndex(e => e.StartDt, "therapist_open_times_start_dt_foreign");

      entity.HasIndex(e => e.UserId, "therapist_open_times_user_id_foreign");

      entity.Property(e => e.Id)
              .HasColumnType("bigint(20) unsigned")
              .HasColumnName("id");
      entity.Property(e => e.CreatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("created_at");
      entity.Property(e => e.StartDt)
              .HasColumnType("datetime")
              .HasColumnName("start_dt");
      entity.Property(e => e.UpdatedAt)
              .HasColumnType("timestamp")
              .HasColumnName("updated_at");
      entity.Property(e => e.UserId).HasColumnName("user_id");

      entity.HasOne(d => d.Calendar).WithMany(p => p.TherapistOpenTimes)
               .HasForeignKey(e => e.StartDt)
               .HasPrincipalKey(u => u.Dt)
              .HasConstraintName("therapist_open_times_start_dt_foreign");

      entity.HasOne(d => d.User).WithMany(p => p.TherapistOpenTimes)
              .HasForeignKey(e => e.UserId)
              .HasPrincipalKey(u => u.StaffId)
             .HasConstraintName("therapist_open_times_user_id_foreign");
    });

    modelBuilder.Entity<User>(entity =>
      {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

        entity.HasIndex(e => e.CertificateNumber, "users_certificate_number_unique").IsUnique();

        entity.HasIndex(e => e.StaffId, "users_staff_id_index");

        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.AccessFailedCount)
                .HasColumnType("int(11)")
                .HasColumnName("access_failed_count");
        entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
        entity.Property(e => e.CertificateNumber)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("certificate_number");
        entity.Property(e => e.ConcurrencyStamp)
                .HasMaxLength(255)
                .HasColumnName("concurrency_stamp");
        entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
        entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
        entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
        entity.Property(e => e.Latitude)
                .HasPrecision(10, 7)
                .HasColumnName("latitude");
        entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
        entity.Property(e => e.LockoutEnd)
                .HasColumnType("timestamp")
                .HasColumnName("lockout_end");
        entity.Property(e => e.Longitude)
                .HasPrecision(10, 7)
                .HasColumnName("longitude");
        entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(256)
                .HasColumnName("normalized_email");
        entity.Property(e => e.NormalizedUserName)
                .HasMaxLength(256)
                .HasColumnName("normalized_username");
        entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
        entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phone_number");
        entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
        entity.Property(e => e.Radius)
                .HasDefaultValueSql("'12'")
                .HasColumnType("int(10) unsigned")
                .HasColumnName("radius");
        entity.Property(e => e.RememberToken)
                .HasMaxLength(100)
                .HasColumnName("remember_token");
        entity.Property(e => e.SecurityStamp)
                .HasMaxLength(255)
                .HasColumnName("security_stamp");
        entity.Property(e => e.StaffId)
                .HasMaxLength(50)
                .HasColumnName("staff_id");
        entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
        entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        entity.Property(e => e.UserName)
                .HasMaxLength(256)
                .HasColumnName("username");
      });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
