using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTherapistApi.Models;
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeTherapistApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FeedbackController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public FeedbackController(HometherapistContext context)
    {
      _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateFeedback([FromBody] FeedbackCreationDto feedbackDto)
    {
      var order = await _context.Orders.FirstOrDefaultAsync(o =>
          o.CustomerId == feedbackDto.CustomerId &&
          o.CustomerPhone == feedbackDto.CustomerPhone &&
          o.StartDt == feedbackDto.StartDt &&
          o.StartDt < DateTime.Now &&
          o.IsComplete == true);

      if (order == null)
        return NotFound(new ApiResponse<object> { IsSuccess = false, Message = $"找不到相關訂單。" });
      if (order.Feedbacks != null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = $"此訂單已評價過。" });
      if (order.StartDt?.AddMinutes(90) > DateTime.Now)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = $"此訂單還在儲存當中。" });

      var feedback = new Feedback
      {
        UserId = order.UserId,
        OrderId = order.Id,
        CustomerId = feedbackDto.CustomerId,
        Rating = feedbackDto.Rating,
        Comments = feedbackDto.Comments,
        CreatedAt = DateTime.Now,
      };

      _context.Feedbacks.Add(feedback);
      await _context.SaveChangesAsync();

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = $"成功提交評價", Data = feedback });
    }
    [HttpGet("User/AverageRating")]
    public async Task<ActionResult<ApiResponse<double>>> GetAverageRatingByUser()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請登入" });
      var averageRating = await _context.Feedbacks
          .Where(f => f.UserId == userId)
          .AverageAsync(f => f.Rating);

      return Ok(new ApiResponse<double> { IsSuccess = true, Message = $"使用者的評價平均分數為: {averageRating}", Data = averageRating });
    }

    [HttpGet("User/Ratings")]
    public async Task<ActionResult<ApiResponse<List<FeedbackWithOrderDto>>>> GetFeedbacksByUser(uint? minRating = 0, uint? maxRating = 5)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object>
        { IsSuccess = false, Message = "請登入" });
      if (minRating.Value > maxRating.Value || minRating.Value > 5 || maxRating.Value > 5 || minRating.Value < 0 || maxRating.Value <= 0)
        return BadRequest(new ApiResponse<object>
        { IsSuccess = false, Message = "參數錯誤" });

      var feedbacksQuery = _context.Feedbacks
          .Include(f => f.Order)
          .Include(f => f.Order.OrderDetails)
          .Where(f => f.UserId == userId);

      if (minRating.HasValue)
        feedbacksQuery = feedbacksQuery.Where(f => f.Rating >= minRating.Value);

      if (maxRating.HasValue)
        feedbacksQuery = feedbacksQuery.Where(f => f.Rating <= maxRating.Value);

      var feedbacks = await feedbacksQuery.ToListAsync();

      var feedbacksWithOrder = feedbacks.Select(f => new FeedbackWithOrderDto
      {
        Feedback = f,
        Order = f.Order,
        OrderDetails = f.Order.OrderDetails.ToList()
      }).ToList();

      return Ok(new ApiResponse<List<FeedbackWithOrderDto>>
      {
        IsSuccess = true,
        Message = $"找到 {feedbacksWithOrder.Count} 個評價",
        Data = feedbacksWithOrder
      });
    }

  }
  public class FeedbackWithOrderDto
  {
    public Feedback Feedback { get; set; }
    public Order Order { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
  }
  public class FeedbackCreationDto
  {
    public string CustomerId { get; set; }
    public string CustomerPhone { get; set; }
    public DateTime? StartDt { get; set; }
    public uint Rating { get; set; }
    public string? Comments { get; set; }
  }
}