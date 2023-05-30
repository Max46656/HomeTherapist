using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace HomeTherapistApi.test
{
  public class PhotoControllerTests
  {
    [Fact]
    public async Task UploadProfileImage_WithValidData_ReturnsOk()
    {
      // Arrange
      var factory = new WebApplicationFactory<Program>();
      var client = factory.CreateClient();
      var content = new MultipartFormDataContent();
      var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Your file content"));
      fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
      content.Add(fileContent, "file", "filename.jpg");

      // Act
      var response = await client.PostAsync("/photo/upload-profile-image", content);

      // Assert
      response.EnsureSuccessStatusCode();
      var responseContent = await response.Content.ReadAsStringAsync();
      var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
      Assert.True(apiResponse.IsSuccess);
      Assert.Equal("圖片上傳成功", apiResponse.Message);
      Assert.NotNull(apiResponse.Data);
    }

  }
}