using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using HomeTherapistApi.Models;
using HomeTherapistApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Cryptography;
using TextEncoder = System.Text;

namespace HomeTherapistApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class PhotoController : ControllerBase
  {
    private readonly HometherapistContext _context;

    public PhotoController(HometherapistContext context)
    {
      _context = context;
    }
    [Authorize]
    [HttpPost("UploadProfileImage")]
    public async Task<ActionResult<ApiResponse<string>>> UploadProfileImage()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請登入" });

      var file = Request.Form.Files.FirstOrDefault();
      if (file == null)
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "請上傳圖片" });

      // 檢查檔案類型
      var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
      if (!allowedFileTypes.Contains(file.ContentType))
        return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "無效的檔案格式" });

      // var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
      // var targetFolder = Path.Combine(currentDirectory, "ProfilePhoto");
      var projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
      var targetFolder = Path.Combine(projectDirectory, "ProfilePhoto");
      // 建立工號.jpg 的檔名
      var fileName = $"{userId}.jpg";

      // 轉換圖片格式為 JPG
      var targetPath = Path.Combine(targetFolder, fileName);
      await ConvertToJpg(file, targetPath);

      return Ok(new ApiResponse<string> { IsSuccess = true, Message = "圖片上傳成功", Data = fileName });
    }

    private async Task ConvertToJpg(IFormFile file, string targetPath)
    {
      using (var image = Image.FromStream(file.OpenReadStream()))
      {
        if (file.ContentType != "image/jpeg")
        {
          var encoder = GetEncoderInfo("image/jpeg");
          var encoderParameters = new EncoderParameters(1);
          encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 75L);
          image.Save(targetPath, encoder, encoderParameters);
        }
        else
        {
          await using (var stream = new FileStream(targetPath, FileMode.Create))
            await file.CopyToAsync(stream);

        }
      }
    }

    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
      var encoders = ImageCodecInfo.GetImageEncoders();
      return encoders.FirstOrDefault(encoder => encoder.MimeType == mimeType);
    }

    [HttpGet("ProfileImage")]
    public IActionResult GetProfileImage()
    {
      var userId = User.FindFirst("StaffId")?.Value;
      if (userId == null)
      {
        // 返回匿名請求所對應的工號.jpg 圖片
        var anonymousUserId = "anonymous";
        var fileName = $"{anonymousUserId}.jpg";
        var imagePath = Path.Combine("ProfilePhoto", fileName);
        var physicalFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        return File(physicalFileProvider.GetFileInfo(imagePath).CreateReadStream(), "image/jpeg");
      }

      // 返回使用者所對應的工號.jpg 圖片
      var userFileName = $"{userId}.jpg";
      var userImagePath = Path.Combine("ProfilePhoto", userFileName);
      var userPhysicalFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
      return File(userPhysicalFileProvider.GetFileInfo(userImagePath).CreateReadStream(), "image/jpeg");
    }
    // [HttpGet("generate-dummy-images")]
    // public async Task<IActionResult> GenerateDummyImages()
    // {
    //   try
    //   {
    //     // 從資料庫獲取所有 StaffId
    //     List<string> staffIds = await _context.Users.Select(u => u.StaffId).ToListAsync();

    //     // 保存假圖的目錄路徑
    //     var projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    //     string imagesDirectory = Path.Combine(projectDirectory, "ProfilePhoto");

    //     // 創建目錄（如果不存在）
    //     Directory.CreateDirectory(imagesDirectory);

    //     // 使用 Gravatar 服務生成隨機頭像
    //     foreach (string staffId in staffIds)
    //     {
    //       // 根據 StaffId 計算 Gravatar 的哈希值
    //       string hash = CalculateGravatarHash(staffId);

    //       // 生成 Gravatar 圖片的 URL
    //       string gravatarUrl = $"https://www.gravatar.com/avatar/{hash}?s=200&d=identicon";

    //       // 下載 Gravatar 圖片
    //       using (HttpClient httpClient = new HttpClient())
    //       {
    //         HttpResponseMessage response = await httpClient.GetAsync(gravatarUrl);

    //         if (response.IsSuccessStatusCode)
    //         {
    //           // 讀取圖片數據
    //           byte[] imageData = await response.Content.ReadAsByteArrayAsync();

    //           // 圖片檔案的路徑
    //           string imagePath = Path.Combine(imagesDirectory, $"{staffId}.jpg");

    //           // 保存圖片
    //           await System.IO.File.WriteAllBytesAsync(imagePath, imageData);
    //         }
    //       }
    //     }

    //     return Ok("假圖生成完成");
    //   }
    //   catch (Exception ex)
    //   {
    //     // 錯誤處理邏輯
    //     return StatusCode(500, "生成假圖時發生錯誤");
    //   }
    // }

    // private string CalculateGravatarHash(string input)
    // {
    //   using (MD5 md5 = MD5.Create())
    //   {
    //     byte[] inputBytes = TextEncoder.Encoding.ASCII.GetBytes(input.ToLowerInvariant());
    //     byte[] hashBytes = md5.ComputeHash(inputBytes);
    //     TextEncoder.StringBuilder builder = new TextEncoder.StringBuilder();

    //     for (int i = 0; i < hashBytes.Length; i++)
    //     {
    //       builder.Append(hashBytes[i].ToString("x2"));
    //     }

    //     return builder.ToString();
    //   }
    // }
  }
}