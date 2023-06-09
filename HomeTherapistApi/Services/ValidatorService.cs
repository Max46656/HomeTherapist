using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HomeTherapistApi.Services
{
  public static class ValidatorService
  {
    public static bool ValidateTaiwanId(string id)
    {
      // 使用「正規表達式」檢驗格式 [A~Z] {1}個數字 [0~9] {9}個數字
      var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
      if (!regex.IsMatch(id))
        //Regular Expression 驗證失敗，回傳 ID 錯誤
        return false;

      //除了檢查碼外每個數字的存放空間
      int[] seed = new int[10];

      //建立字母陣列(A~Z)
      //A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
      //P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35
      string[] charMapping = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
      string target = id.Substring(0, 1); //取第一個英文數字
      for (int index = 0; index < charMapping.Length; index++)
      {
        if (charMapping[index] == target)
        {
          index += 10;
          //10進制的高位元放入存放空間   (權重*1)
          seed[0] = index / 10;
          //10進制的低位元*9後放入存放空間 (權重*9)
          seed[1] = (index % 10) * 9;

          break;
        }
      }
      for (int index = 2; index < 10; index++) //(權重*8~1)
      {   //將剩餘數字乘上權數後放入存放空間
        seed[index] = Convert.ToInt32(id.Substring(index - 1, 1)) * (10 - index);
      }

      return ((10 - (seed.Sum() % 10)) % 10 != Convert.ToInt32(id.Substring(9, 1)));
    }
  }
}
