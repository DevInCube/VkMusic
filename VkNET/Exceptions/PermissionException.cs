using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Exceptions
{
    public class PermissionException : VkException
    {

         public PermissionException() : base() { }
         public PermissionException(string permission) : base(permission + " permission is missing") { }
    }
}
