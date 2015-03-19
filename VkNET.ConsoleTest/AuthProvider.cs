using My.VKMusic.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using VkNET.Auth;
using VkNET.Models;

namespace VkNET.ConsoleTest
{
    class AuthProvider : IAuthProvider
    {

        private IAuthDataStorage authStorage = new XMLAuthStorage("auth.dat");

        public void DoAuth(string request, Action<VkNET.Models.AuthData> callback)
        {
            AuthData data = authStorage.GetAuthData();
            if (data != null)
            {
                if (DateTime.Now < data.ExpiresAt)
                {
                    callback(data);
                    return;
                }
            }
            AuthWindow wnd = new AuthWindow();
            wnd.GotAccessToken += (d)=>{
                authStorage.SetAuthData(d);
                callback(d);
            };
            wnd.Open(request);
            wnd.ShowDialog();
        }
    }

    class XMLAuthStorage : IAuthDataStorage
    {

        private readonly string filePath;

        public XMLAuthStorage(string filename)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.filePath = Path.Combine(dir, filename);
        }

        public Models.AuthData GetAuthData()
        {
            AuthData data = null;
            try
            {
                string content = File.ReadAllText(filePath);
                XElement xel = XElement.Parse(content);
                data = new AuthData();
                data.AccessToken = xel.Element("access_token").Value;
                data.UserId = long.Parse(xel.Element("user_id").Value);
                data.ExpiresAt = DateTime.Parse(xel.Element("expires_at").Value);
                data.ExpiresIn = (data.ExpiresAt - DateTime.Now).Seconds;
            }
            catch (IOException)
            {
                //
            }
            return data;
        }

        public void SetAuthData(Models.AuthData data)
        {
            try
            {
                XElement xel = new XElement("auth_data",
                    new XElement("access_token", data.AccessToken),
                    new XElement("user_id",  data.UserId),
                    new XElement("expires_at", data.ExpiresAt )
                );
                File.WriteAllText(filePath, xel.ToString());
            }
            catch (IOException)
            {
                //
            }
        }
    }
}
