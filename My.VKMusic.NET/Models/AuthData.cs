using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.Models
{
    public class AuthData
    {

        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
        public long UserId { get; set; }
    }
}
