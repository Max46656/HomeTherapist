using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTherapistApi.Utilities
{
  public static class Base64Utils
  {
    public static byte[] Base64UrlDecode(string input)
    {
      string base64 = input;
      base64 = base64.Replace('-', '+').Replace('_', '/');
      while (base64.Length % 4 != 0)
      {
        base64 += '=';
      }
      return Convert.FromBase64String(base64);
    }
  }

}