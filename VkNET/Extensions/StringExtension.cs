using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Extensions
{
    public static class StringExtension
    {

        public static string AsUTF8(this string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string FormatWith(this string format, params object[] objs)
        {
            return String.Format(format, objs);
        }
    }
}
