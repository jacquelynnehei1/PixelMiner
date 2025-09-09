using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixel_miner.Utils.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Length == 0;
        }
    }
}