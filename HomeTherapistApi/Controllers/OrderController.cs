using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using HomeTherapistApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HomeTherapistApi.Controllers
{
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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var orders = await _context.Orders.ToListAsync();
      return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(ulong id)
    {
      var order = await _context.Orders.FindAsync(id);
      if (order == null)
        return NotFound();

      return Ok(order);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ulong id, Order updatedOrder)
    {
      if ((ulong)id != updatedOrder.Id)
        return BadRequest();

      var existingOrder = await _context.Orders.FindAsync(id);
      if (existingOrder == null)
        return NotFound();

      existingOrder.CustomerAddress = updatedOrder.CustomerAddress;
      existingOrder.CustomerId = updatedOrder.CustomerId;
      existingOrder.Longitude = updatedOrder.Longitude;
      existingOrder.Latitude = updatedOrder.Latitude;
      existingOrder.CustomerPhone = updatedOrder.CustomerPhone;
      // existingOrder.IsComplete = updatedOrder.IsComplete;
      existingOrder.StartDt = updatedOrder.StartDt;

      await _context.SaveChangesAsync();
      return NoContent();
    }
    [HttpGet("{id}/feedbacks")]
    public async Task<IActionResult> GetFeedbacks(ulong id)
    {
      var feedbacks = await _context.Feedbacks
          .Where(f => f.OrderId == (ulong)id)
          .ToListAsync();

      return Ok(feedbacks);
    }
    [HttpGet("{id}/orderdetails")]
    public async Task<IActionResult> GetOrderDetails(ulong id)
    {
      var orderDetails = await _context.OrderDetails
          .Where(od => od.OrderId == (ulong)id)
          .ToListAsync();

      return Ok(orderDetails);
    }
  }

}