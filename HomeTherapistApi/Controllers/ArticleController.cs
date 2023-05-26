using HomeTherapistApi.Models;
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

    // // GET: api/Articles
    // [AllowAnonymous]
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
    // {
    //   return await _context.Articles.ToListAsync();
    // }

    // GET: api/Articles
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<Article>> GetArticleByUser()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest();

      var articles = await _context.Articles
         .Where(a => a.UserId == userId)
         .ToListAsync();

      if (articles == null)
        return NotFound();

      return Ok(articles);
    }
    // GET: api/Articles/5
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetArticle(ulong id)
    {
      var article = await _context.Articles.FindAsync(id);

      if (article == null)
        return NotFound();


      return article;
    }

    // POST: api/Articles
    [Authorize]
    [HttpPost]
    public ActionResult<Article> PostArticle(ArticleDto _article)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null) return BadRequest("尚未登入");

      var user = _context.Users.FirstOrDefault(u => u.StaffId == userId);
      if (user == null) return BadRequest("使用者錯誤");

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

      return CreatedAtAction(nameof(GetArticle), new
      {
        id = article.Id
      }, article);
    }

    // PUT: api/Articles/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutArticle(ulong id, ArticleDto _article)
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null || userId != _article.UserId) return BadRequest("使用者錯誤");

      var article = await _context.Articles.FindAsync(id);
      if (article == null) return NotFound();

      article.UpdatedAt = DateTime.Now;

      _context.Entry(article).CurrentValues.SetValues(_article);

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ArticleExists(id))
          return NotFound("該文章不存在。");
        else
          throw;
      }
      return NoContent();
    }

    // DELETE: api/Articles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(ulong id)
    {
      var article = await _context.Articles.FindAsync(id);
      var userId = User.FindFirst("StaffId")?.Value;
      if (article == null) return NotFound();
      if (userId == null || userId != article.UserId) return BadRequest();
      _context.Articles.Remove(article);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ArticleExists(ulong id)
    {
      return _context.Articles.Any(e => e.Id == id);
    }
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
