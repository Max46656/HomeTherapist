using HomeTherapistApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTherapistApi.Controllers
{
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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
    {
      return await _context.Articles.ToListAsync();
    }

    // GET: api/Articles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetArticle(ulong id)
    {
      var article = await _context.Articles.FindAsync(id);

      if (article == null)
      {
        return NotFound();
      }

      return article;
    }

    // POST: api/Articles
    [HttpPost]
    public async Task<ActionResult<Article>> PostArticle(Article article)
    {
      article.CreatedAt = DateTime.Now;
      article.UpdatedAt = DateTime.Now;

      _context.Articles.Add(article);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    // PUT: api/Articles/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutArticle(ulong id, Article article)
    {
      if (id != article.Id)
      {
        return BadRequest();
      }

      article.UpdatedAt = DateTime.Now;

      _context.Entry(article).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ArticleExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // DELETE: api/Articles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(ulong id)
    {
      var article = await _context.Articles.FindAsync(id);

      if (article == null)
      {
        return NotFound();
      }

      _context.Articles.Remove(article);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool ArticleExists(ulong id)
    {
      return _context.Articles.Any(e => e.Id == id);
    }
  }
}
