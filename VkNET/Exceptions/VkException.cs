using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Exceptions
{
    public abstract class VkException : Exception
    {

        public VkException() : base() { }
        public VkException(string message) : base(message) { }
    }
}
