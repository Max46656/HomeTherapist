using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTherapistApi.Utilities
{
  public class ApiResponse<T>
  {
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
  }
}