using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Exceptions
{
    public class AccessToAlbumDeniedVkException : VkException
    {

        public AccessToAlbumDeniedVkException() : base() { }
        public AccessToAlbumDeniedVkException(string message) : base(message) { }
    }
}
