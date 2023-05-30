using HomeTherapistApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerFileUploadFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    if (!context.MethodInfo.Name.Contains("Upload"))
      return;
    // if (context.MethodInfo.Name != nameof(PhotoController.UploadProfileImage))
    //   return;

    // 添加文件參數
    operation.Parameters ??= new List<OpenApiParameter>();

    operation.Parameters.Add(new OpenApiParameter
    {
      Name = "file",
      In = ParameterLocation.Query,
      Description = "Upload File",
      Required = true,
      Schema = new OpenApiSchema
      {
        Type = "file",
        Format = "binary"
      }
    });

    var parameters = operation.Parameters.ToList();

    foreach (var parameter in parameters)
    {
      if (parameter.In == ParameterLocation.Query && parameter.Schema.Type == "file")
      {
        operation.Parameters.Remove(parameter);
        operation.RequestBody = new OpenApiRequestBody
        {
          Content = new Dictionary<string, OpenApiMediaType>
          {
            ["multipart/form-data"] = new OpenApiMediaType
            {
              Schema = new OpenApiSchema
              {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                  [parameter.Name] = new OpenApiSchema
                  {
                    Type = "string",
                    Format = "binary"
                  }
                }
              }
            }
          }
        };
        operation.RequestBody.Required = true;
        break;
      }
    }
  }
}