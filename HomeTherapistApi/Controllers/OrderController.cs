using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Services;
using Microsoft.AspNetCore.Authorization;
using HomeTherapistApi.Utilities;

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
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請提供使用者 ID" });

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
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不到訂單" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "取得訂單成功", Data = orders });
    }
    [HttpGet("{IdNumber}")]
    public async Task<IActionResult> GetOrderByIdNumber(string IdNumber)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請提供使用者 ID" });

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
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不到訂單" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "取得訂單成功", Data = orders });
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetOrderById(ulong Id)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請提供使用者 ID" });

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
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "找不到訂單" });

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "取得訂單成功", Data = orders });
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

      if (!ValidatorService.ValidateTaiwanId(orderDto.CustomerId))
      {
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "身份證字號錯誤。" });
      }

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
