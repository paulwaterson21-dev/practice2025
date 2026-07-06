using System;
using System.Text;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            StringBuilder cleanBuilder = new StringBuilder();

            foreach (char ch in input)
            {
                if (!char.IsPunctuation(ch) && !char.IsWhiteSpace(ch))
                {
                    cleanBuilder.Append(char.ToLower(ch));
                }
            }

            string cleanString = cleanBuilder.ToString();

            if (cleanString.Length == 0)
            {
                return false;
            }

            int left = 0;
            int right = cleanString.Length - 1;

            while (left < right)
            {
                if (cleanString[left] != cleanString[right])
                {
                    return false;
                }
                left++;
                right--;
            }

            return true;
        }
    }
}