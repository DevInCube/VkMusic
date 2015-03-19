using My.VKMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VkNET.Auth;

namespace VkNET.ConsoleTest
{
    class AuthProvider : IAuthProvider
    {

        public void DoAuth(string request, Action<VkNET.Models.AuthData> callback)
        {
            AuthWindow wnd = new AuthWindow();
            wnd.GotAccessToken += callback;
            wnd.Open(request);
            wnd.ShowDialog();
        }
    }
}
