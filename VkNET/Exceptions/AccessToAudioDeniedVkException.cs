using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Exceptions
{
    public class AccessToAudioDeniedVkException : VkException
    {

        public AccessToAudioDeniedVkException() : base() { }
        public AccessToAudioDeniedVkException(string message) : base(message) { }
    }
}
