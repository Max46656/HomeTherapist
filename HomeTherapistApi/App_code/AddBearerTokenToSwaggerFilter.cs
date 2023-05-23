using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeTherapistApi.App_code
{
  public class AddBearerTokenToSwaggerFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      if (operation.Parameters == null)
        operation.Parameters = new List<OpenApiParameter>();

      operation.Parameters.Add(new OpenApiParameter
      {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Bearer token",
        Required = true,
        Schema = new OpenApiSchema
        {
          Type = "string",
          Default = new OpenApiString("Bearer ")
        }
      });
    }
  }

}