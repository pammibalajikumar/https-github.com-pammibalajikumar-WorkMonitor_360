using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkMonitor_360.Helper
{
    public class StringHelper
    {
        public static string MaskMiddle(string? input)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= 4)
                return input ?? "";

            int start = input.Length / 2;
            int length = input.Length / 2;

            return input.Remove(start, length)
                        .Insert(start, new string('*', length));
        }

    }
}
