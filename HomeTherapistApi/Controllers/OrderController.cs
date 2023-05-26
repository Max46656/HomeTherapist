using Microsoft.AspNetCore.Mvc;
using HomeTherapistApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Services;
using Microsoft.AspNetCore.Authorization;

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
      if (userId == null) return BadRequest();
      var Order = await _context.Orders
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
      if (Order.Count == 0)
        return NotFound();

      return Ok(Order);
    }
    [HttpGet("{IdNumber}")]
    public async Task<IActionResult> GetOrderByIdNumber(string IdNumber)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();
      var Order = await _context.Orders
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
      if (Order.Count == 0)
        return NotFound();

      return Ok(Order);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetOrderById(ulong Id)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();
      var Order = await _context.Orders
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
      if (Order.Count == 0)
        return NotFound();

      return Ok(Order);
    }
    // POST: api/Order
    [HttpPost]
    public Order CreateOrderWithDetail(OrderDto OrderDto)
    {
      var missingFields = new List<string>();

      missingFields.AddRange(OrderDto switch
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

      if (!ValidatorService.ValidateTaiwanId(OrderDto.CustomerId))
        throw new ArgumentException("身份證字號錯誤。");

      var Order = new Order
      {
        UserId = OrderDto.UserId,
        StartDt = OrderDto.StartDt,
        CustomerId = OrderDto.CustomerId,
        CustomerPhone = OrderDto.CustomerPhone,
        CustomerAddress = OrderDto.CustomerAddress,
        Latitude = OrderDto.Latitude,
        Longitude = OrderDto.Longitude,
        IsComplete = false,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      var OrderDetail = new OrderDetail
      {
        ServiceId = OrderDto.ServiceId,
        Price = OrderDto.Price,
        Note = OrderDto.Note,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      // 互相關連
      Order.OrderDetails.Add(OrderDetail);
      OrderDetail.Order = Order;

      _context.Orders.Add(Order);
      _context.SaveChanges();

      return Order;
    }

    // PATCH: api/Order/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateOrder(ulong id, [FromBody] JsonPatchDocument<Order> patchDocument)
    {
      var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);

      if (order == null)
        return NotFound();

      patchDocument.ApplyTo(order, ModelState);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // DELETE: api/Order/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(long id)
    {
      var order = await _context.Orders.FindAsync(id);
      if (order == null)
        return NotFound();

      order.IsComplete = false;
      await _context.SaveChangesAsync();

      return NoContent();
    }
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
