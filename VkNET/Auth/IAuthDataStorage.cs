using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Models;

namespace VkNET.Auth
{
    public interface IAuthDataStorage
    {

        AuthData GetAuthData();
        void SetAuthData(AuthData data);
    }
}
