using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VkNET.Models
{
    public class AuthData
    {

        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
        public long UserId { get; set; }
        public DateTime ExpiresAt { get; set; }

    }
}
