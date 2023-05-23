using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeTherapistApi.App_code
{
  public class AddJwtTokenToSwagger : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      // 檢查是否需要 JWT 授權
      var hasAuthorizeAttribute = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
          .Union(context.MethodInfo.GetCustomAttributes(true))
          .OfType<AuthorizeAttribute>()
          .Any();

      if (hasAuthorizeAttribute)
      {
        // 在 Swagger 的請求 Header 中添加 JWT Token
        operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new List<string>()
                    }
                }
            };
      }
    }
  }
}
