using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Extensions
{
    public static class BoolExtension
    {

        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }
    }
}
