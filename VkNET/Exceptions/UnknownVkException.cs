using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Exceptions
{
    public class UnknownVkException : VkException
    {

        public UnknownVkException() : base() { }
        public UnknownVkException(string message) : base(message) { }
    }
}
