using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Exceptions
{
    public class AlbumFullVkException : VkException
    {

        public AlbumFullVkException() : base() { }
        public AlbumFullVkException(string message) : base(message) { }
    }
}
