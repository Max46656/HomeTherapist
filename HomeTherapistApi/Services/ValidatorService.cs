using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HomeTherapistApi.Services
{
  public static class ValidatorService
  {
    public static bool ValidateTaiwanId(string id)
    {
      // 身份证号码必须为10位
      if (id.Length != 10)
        return false;

      // 将身份证号码每一位拆分并转换为整数
      int[] digits = new int[10];
      for (int i = 0; i < 10; i++)
      {
        if (!int.TryParse(id[i].ToString(), out digits[i]))
          return false;

      }

      // 根据规则计算校验码
      int sum = digits[0] * 1 +
                digits[1] * 9 +
                digits[2] * 8 +
                digits[3] * 7 +
                digits[4] * 6 +
                digits[5] * 5 +
                digits[6] * 4 +
                digits[7] * 3 +
                digits[8] * 2 +
                digits[9] * 1;

      // 检查校验码是否为10的倍数
      return sum % 10 == 0;
    }
  }
}
