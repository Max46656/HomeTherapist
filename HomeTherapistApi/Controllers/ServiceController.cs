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

    [HttpGet("GetAllServiceNames")]
    public ActionResult<ApiResponse<List<string>>> GetAllServiceNames()
    {
      var serviceNames = _dbContext.Services.Select(s => s.Name).ToList();
      return Ok(new ApiResponse<List<string>>
      { IsSuccess = true, Data = serviceNames });
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
}