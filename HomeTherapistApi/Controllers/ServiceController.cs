using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeTherapistApi.Models;
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace HomeTherapistApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ServiceController : ControllerBase
  {
    private readonly HometherapistContext _dbContext;

    public ServiceController(HometherapistContext dbContext)
    {
      _dbContext = dbContext;
    }

    [HttpGet("GetAllServices")]
    public ActionResult<ApiResponse<List<ServiceDto>>> GetAllServices()
    {
      var services = _dbContext.Services
          .Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Price = s.Price })
          .ToList();

      return Ok(new ApiResponse<List<ServiceDto>>
      {
        IsSuccess = true,
        Data = services
      });
    }

    [HttpGet("GetServicePrice/{serviceId}")]
    public ActionResult<ApiResponse<double?>> GetServicePrice(ulong serviceId)
    {
      var service = _dbContext.Services.FirstOrDefault(s => s.Id == serviceId);
      if (service == null)
        return NotFound(new ApiResponse<double?>
        { IsSuccess = false, Message = "沒有此服務" });

      return Ok(new ApiResponse<double?> { IsSuccess = true, Data = service.Price });
    }
  }
  public class ServiceDto
  {
    public ulong Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
  }

}