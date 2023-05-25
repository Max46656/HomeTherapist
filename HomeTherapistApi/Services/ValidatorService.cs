using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HomeTherapistApi.Services
{
  public class ValidatorService
  {
    private static Dictionary<char, int> idLocationMapping = new Dictionary<char, int> {
        {'A', 10}, {'B', 11}, {'C', 12}, {'D', 13}, {'E', 14}, {'F', 15}, {'G', 16}, {'H', 17},
        {'I', 34}, {'J', 18}, {'K', 19}, {'M', 21}, {'N', 22}, {'O', 35}, {'P', 23}, {'Q', 24},
        {'T', 27}, {'U', 28}, {'V', 29}, {'W', 32}, {'X', 30}, {'Z', 33}
    };

    public static bool ValidateTaiwanId(string id)
    {
      if (!Regex.IsMatch(id, @"^[A-Za-z][0-9]{9}$"))
        return false;

      int[] weights = { 1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
      char[] charArray = id.ToUpper().ToCharArray();
      int sum = idLocationMapping[charArray[0]];

      for (int i = 1; i < charArray.Length; i++)
        sum += (charArray[i] - '0') * weights[i];

      return sum % 10 == 0;
    }

    public static bool ValidateTaiwanPhone(string phone)
    {
      return Regex.IsMatch(phone, @"^09[0-9]{8}$");
    }
  }
}