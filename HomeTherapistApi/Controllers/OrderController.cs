using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Services;
using Microsoft.AspNetCore.Authorization;
using HomeTherapistApi.Utilities;
using System.ComponentModel;

namespace HomeTherapistApi.Controllers
{
  // 面向治療師
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class OrderController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public OrderController(HometherapistContext context)
    {
      _context = context;
    }

    // // GET: api/Order
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    // {
    //   return await _context.Orders.ToListAsync();
    // }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請登入" });

      var orders = await _context.Orders
         .Include(a => a.OrderDetails)
         .ThenInclude(ad => ad.Service)
         .Where(a => a.UserId == userId)
         .Select(a => new
         {
           Order = a,
           OrderDetails = a.OrderDetails.Select(ad => new
           {
             OrderDetail = ad,
             Service = ad.Service
           }).ToList(),
           User = a.User,
           Calendar = a.Calendar
         })
         .ToListAsync();

      if (orders.Count == 0)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不-訂單" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "取得訂單成功", Data = orders });
    }
    [HttpGet("{IdNumber}")]
    public async Task<IActionResult> GetOrderByIdNumber(string IdNumber)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請登入" });

      var orders = await _context.Orders
         .Include(a => a.OrderDetails)
         .ThenInclude(ad => ad.Service)
         .Where(a => a.UserId == userId && a.CustomerId == IdNumber)
         .Select(a => new
         {
           Order = a,
           OrderDetails = a.OrderDetails.Select(ad => new
           {
             OrderDetail = ad,
             Service = ad.Service
           }).ToList(),
           User = a.User,
           Calendar = a.Calendar
         })
         .ToListAsync();

      if (orders.Count == 0)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不-訂單" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "取得訂單成功", Data = orders });
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetOrderById(ulong Id)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請登入" });

      var orders = await _context.Orders
         .Include(a => a.OrderDetails)
         .ThenInclude(ad => ad.Service)
         .Where(a => a.UserId == userId && a.Id == Id)
         .Select(a => new
         {
           Order = a,
           OrderDetails = a.OrderDetails.Select(ad => new
           {
             OrderDetail = ad,
             Service = ad.Service
           }).ToList(),
           User = a.User,
           Calendar = a.Calendar
         })
         .ToListAsync();

      if (orders.Count == 0)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不-訂單" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "取得訂單成功", Data = orders });
    }

    [HttpGet("gender/{gender}")]
    public async Task<IActionResult> GetOrdersByGender(string gender)
    {
      var totalOrderCount = await _context.Orders.CountAsync();

      if (!IsValidGender(gender))
      {
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無效的性別" });
      }

      var genderOrderCount = await _context.Orders.CountAsync(o => o.Gender.Equals(gender));

      var percentage = Math.Round((double)genderOrderCount / totalOrderCount * 100, 2);

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"在全部的訂單中，你所查詢的條件佔{percentage.ToString("0.00")}%." });
    }

    [HttpGet("agegroup/{ageGroup}")]
    public async Task<IActionResult> GetOrdersByAgeGroup(string ageGroup)
    {
      var totalOrderCount = await _context.Orders.CountAsync();

      if (!IsValidAgeGroup(ageGroup))
      {
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無效的年齡層" });
      }

      var ageGroupOrderCount = await _context.Orders.CountAsync(o => o.AgeGroup.Equals(ageGroup));

      var percentage = Math.Round((double)ageGroupOrderCount / totalOrderCount * 100, 2);

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"在全部的訂單中，你所查詢的條件佔{percentage.ToString("0.00")}%." });
    }


    [HttpGet("gender/{gender}/agegroup/{ageGroup}")]
    public async Task<IActionResult> GetOrdersByGenderAndAgeGroup(string gender, string ageGroup)
    {
      var totalOrderCount = await _context.Orders.CountAsync();

      // 驗證性別和年齡層是否為有效選項
      if (!IsValidGender(gender))
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無效的性別" });


      if (!IsValidAgeGroup(ageGroup))
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無效的年齡層" });


      var genderAndAgeGroupOrderCount = await _context.Orders.CountAsync(o =>
          o.Gender.Equals(gender) &&
          o.AgeGroup.Equals(ageGroup));

      var percentage = Math.Round((double)genderAndAgeGroupOrderCount / totalOrderCount * 100, 2);

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"在全部的訂單中，你所查詢的條件佔{percentage.ToString("0.00")}%." });
    }

    private bool IsValidGender(string gender)
    {
      // 檢查性別是否為有效選項
      var validGenders = new List<string> { "男", "女", "其他" };
      return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsValidAgeGroup(string ageGroup)
    {
      // 檢查年齡層是否為有效選項
      var validAgeGroups = new List<string> { "小於18", "18-25", "26-35", "36-45", "46-55", "56-65", "66-75", "大於75" };
      return validAgeGroups.Contains(ageGroup, StringComparer.OrdinalIgnoreCase);
    }

    [HttpPost]
    public IActionResult CreateOrderWithDetail(OrderDto orderDto)
    {
      var missingFields = new List<string>();

      missingFields.AddRange(orderDto switch
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

      if (missingFields.Count > 0)
      {
        var errorMessage = "缺少以下必要字段: " + string.Join(", ", missingFields);
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = errorMessage });
      }

      if (ValidatorService.ValidateTaiwanId(orderDto.CustomerId))
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "身份證字號錯誤。" });


      var order = new Order
      {
        UserId = orderDto.UserId,
        StartDt = orderDto.StartDt,
        CustomerId = orderDto.CustomerId,
        CustomerPhone = orderDto.CustomerPhone,
        CustomerAddress = orderDto.CustomerAddress,
        Latitude = orderDto.Latitude,
        Longitude = orderDto.Longitude,
        IsComplete = false,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      var orderDetail = new OrderDetail
      {
        ServiceId = orderDto.ServiceId,
        Price = orderDto.Price,
        Note = orderDto.Note,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      order.OrderDetails.Add(orderDetail);
      orderDetail.Order = order;

      _context.Orders.Add(order);
      _context.SaveChanges();

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "訂單創建成功", Data = order });
    }
    public class OrderDto
    {
      [Required]
      public string? UserId { get; set; }
      [Required]
      public DateTime? StartDt { get; set; }
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
      [Required]
      public ulong ServiceId { get; set; }
      [Required]
      public double Price { get; set; }
      public string? Note { get; set; }
    }


  }
}
