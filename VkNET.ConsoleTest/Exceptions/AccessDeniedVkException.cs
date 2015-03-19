using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.ConsoleTest.Exceptions
{
    public class AccessDeniedVkException : VkException
    {

        public AccessDeniedVkException() : base() { }
        public AccessDeniedVkException(string message) : base(message) { }
    }
}
