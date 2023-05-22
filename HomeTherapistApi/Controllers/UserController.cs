using HomeTherapistApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTherapistApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public UsersController(HometherapistContext context)
    {
      _context = context;
    }

    // GET: api/Users
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    // {
    //   var pageNumber = 1; // 要擷取的分頁號碼
    //   var pageSize = 100; // 每頁的資料筆數

    //   // 計算要跳過的資料筆數
    //   var skipCount = (pageNumber - 1) * pageSize;

    //   // 使用原生 SQL 查詢取得使用者資料
    //   var query = $"SELECT *  FROM Users LIMIT {skipCount}, {pageSize};";
    //   var users = await _context.Users.FromSqlRaw(query).AsNoTracking().ToListAsync();

    //   // 轉換 User objects 為 User objects
    //   var Users = users.Select(user => new User
    //   {
    //     Id = user.Id,
    //     Name = user.Name,
    //     Location = user.Location.AsText()
    //   });

    //   return Ok(Users);
    // }



    // GET: api/Users/T0001
    [HttpGet("{StaffId}")]
    public async Task<ActionResult<User>> GetUser(string StaffId)
    {
      var user = await _context.Users.FindAsync(StaffId);

      if (user == null)
      {
        return NotFound();
      }

      return user;
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User User)
    {

      _context.Users.Add(User);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetUser), new { id = User.Id }, User);
    }

    // PUT: api/Users/T0001
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(string StaffId, User user)
    {
      if (StaffId != user.StaffId)
      {
        return BadRequest();
      }

      _context.Entry(user).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return NoContent();
    }

    // DELETE: api/Users/T0001
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string StaffId)
    {
      var user = await _context.Users.FindAsync(StaffId);

      if (user == null)
      {
        return NotFound();
      }

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    [HttpGet("{StaffId}/articles")]
    public async Task<ActionResult<IEnumerable<IGrouping<DateTime, Article>>>> GetArticleByUserId(string StaffId)
    {
      var articles = await _context.Articles
          .Where(a => a.User.StaffId == StaffId)
          .ToListAsync();

      if (articles == null)
      {
        return NotFound();
      }

      var articlesByDate = articles
               .GroupBy(a => a.CreatedAt);

      return Ok(articlesByDate);
    }


    [HttpGet("{StaffId}/appointments")]
    public async Task<ActionResult<IEnumerable<IGrouping<DateTime, Appointment>>>> GetUserAppointmentsByDate(string StaffId)
    {
      var appointments = await _context.Appointments
       .Where(a => a.User.StaffId == StaffId)
       .ToListAsync();

      // var appointments = await _context.Users
      //     .Where(u => u.StaffId == StaffId)
      //     .SelectMany(u => u.Appointments)
      //     .ToListAsync();

      if (appointments == null)
      {
        return NotFound();
      }

      var appointmentsByDate = appointments
          .OrderByDescending(a => a.StartDt)
          .Select(a => new
          {
            a.StartDt,
            a.CustomerId,
            a.CustomerPhone,
            a.CustomerAddress,
            CustomerLocation = a.CustomerLocation != null ? a.CustomerLocation.AsText() : null,
            a.IsComplete
          })
          .OrderByDescending(a => a.StartDt);

      return Ok(appointmentsByDate);
    }
  }
}
