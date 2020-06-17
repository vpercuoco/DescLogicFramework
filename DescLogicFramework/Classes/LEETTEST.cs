using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{


    public class Solution
    {
        public int[] TwoSum(int[] nums, int target)
        {
            int currentIndex = 0;
            Dictionary<int, int> Store = new Dictionary<int, int>();

            foreach (int x in nums)
            {
               if (Store.ContainsKey(target - x))
                  {
                     return new int[] { Store[target - x], currentIndex };
                  }
               else
                 {
                    if (!Store.ContainsKey(x))
                    {
                        Store.Add(x, currentIndex);
                    }
                 }
                currentIndex++;
            }

            return null;
        }
    }

    public class Solution2
    {
        public int Reverse(int x)
        {
            char[] digits = x.ToString().ToCharArray();

            try
            {
                if (digits[0].ToString() == "-")
                {
                    char[] digitsReversed = new char[digits.Length - 1];
                    for (int i = digits.Length - 1; i >= 1; i--)
                    {
                        digitsReversed[digits.Length - 1 - i] = digits[i];
                    }
                    return Convert.ToInt32(string.Join("", "-" + digitsReversed));
                }
                else
                {
                    char[] digitsReversed = new char[digits.Length];
                    for (int i = digits.Length - 1; i >= 0; i--)
                    {
                        digitsReversed[digits.Length - 1 - i] = digits[i];
                    }
                    return Convert.ToInt32(string.Join("", digitsReversed));
                }
            }
            catch (Exception)
            {

                return 0;
            }
        }
    }
}
