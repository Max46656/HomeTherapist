using HomeTherapistApi.Models;
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTherapistApi.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class ArticlesController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public ArticlesController(HometherapistContext context)
    {
      _context = context;
    }

    // GET: api/Articles
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetAllArticles()
    {
      return Ok(new ApiResponse<IEnumerable<Article>>
      {
        IsSuccess = true,
        Message = "查詢所有文章完成",
        Data = await _context.Articles.ToListAsync()
      });
      // return await _context.Articles.ToListAsync();
    }
    // GET: api/Articles
    [Authorize]
    [HttpGet("ByUser")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Article>>>> GetArticleByUser()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<IEnumerable<Article>> { IsSuccess = false, Message = "尚未登入" });

      var articles = await _context.Articles
         .Where(a => a.UserId == userId)
         .ToListAsync();

      if (articles == null)
        return NotFound(new ApiResponse<IEnumerable<Article>> { IsSuccess = false, Message = "查無文章" });

      return Ok(new ApiResponse<IEnumerable<Article>> { IsSuccess = true, Message = "查詢用戶文章完成", Data = articles });
    }
    // GET: api/Articles/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Article>>> GetArticle(ulong id)
    {
      var article = await _context.Articles.FindAsync(id);

      if (article == null)
        return NotFound(new ApiResponse<Article> { IsSuccess = false, Message = "查無文章" });

      return Ok(new ApiResponse<Article> { IsSuccess = true, Message = "查詢文章完成", Data = article });
    }

    // POST: api/Articles
    [Authorize]
    [HttpPost]
    public ActionResult<ApiResponse<Article>> Create(InputArticleDto _article)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<Article> { IsSuccess = false, Message = "尚未登入" });

      var user = _context.Users.FirstOrDefault(u => u.StaffId == userId);
      if (user == null)
        return BadRequest(new ApiResponse<Article> { IsSuccess = false, Message = "使用者錯誤" });

      var article = new Article
      {
        UserId = userId,
        Title = _article.Title,
        Subtitle = _article.Subtitle,
        Body = _article.Body,
        User = user,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
      };

      _context.Articles.Add(article);
      _context.SaveChanges();

      return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, new ApiResponse<Article> { IsSuccess = true, Message = "新增文章完成", Data = article });
    }

    // PUT: api/Articles/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Article>>> Update(ulong id, ArticleDto _article)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null || userId != _article.UserId)
        return BadRequest(new ApiResponse<Article> { IsSuccess = false, Message = "使用者錯誤" });

      var article = await _context.Articles.FindAsync(id);
      if (article == null)
        return NotFound(new ApiResponse<Article> { IsSuccess = false, Message = "該文章不存在" });

      article.UpdatedAt = DateTime.Now;
      _context.Entry(article).CurrentValues.SetValues(_article);

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ArticleExists(id))
          return NotFound(new ApiResponse<Article> { IsSuccess = false, Message = "該文章不存在" });
        else
          throw;
      }

      return Ok(new ApiResponse<object> { IsSuccess = true, Message = "更新文章成功", Data = article });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteArticle(ulong id)
    {
      var article = await _context.Articles.FindAsync(id);
      var userId = User.FindFirst("StaffId")?.Value;
      if (article == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "該文章不存在" });
      if (userId == null || userId != article.UserId) return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "使用者錯誤" });

      _context.Articles.Remove(article);
      await _context.SaveChangesAsync();

      return Ok(new ApiResponse<object>
      { IsSuccess = true, Message = "刪除文章成功" });
    }


    private bool ArticleExists(ulong id)
    {
      return _context.Articles.Any(e => e.Id == id);
    }
  }
  public class InputArticleDto
  {

    public string Title { get; set; } = null!;

    public string? Subtitle { get; set; }

    public string Body { get; set; } = null!;
  }
  public class ArticleDto
  {
    public string UserId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Subtitle { get; set; }

    public string Body { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
  }
}
