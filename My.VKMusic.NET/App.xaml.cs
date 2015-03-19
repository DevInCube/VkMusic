using My.VKMusic.Models;
using My.VKMusic.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VkNET;
using VkNET.Auth;
using VkNET.Models;

namespace My.VKMusic.NET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private VkAPI vk = new VkAPI(new AuthProvider());

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TestWindow playerWindow = new TestWindow();
            playerWindow.Show();

            BackgroundWorker loader = new BackgroundWorker();
            loader.DoWork += (sender2, e2) =>
            {
                var vk2 = e2.Argument as VkAPI;
                var audios = vk2.AudioGet();
                e2.Result = audios;
            };
            loader.RunWorkerCompleted += (s2, e2) =>
            {
                var audios = e2.Result as List<AudioFileInfo>;
                playerWindow.InitAudioList(audios);
            };

            vk.DoAuth(() => {
                loader.RunWorkerAsync(vk);                
            });
            
        }

        class AuthProvider : IAuthProvider
        {

            public void DoAuth(string request, Action<VkNET.Models.AuthData> callback)
            {
                AuthWindow wnd = new AuthWindow();
                wnd.GotAccessToken += callback;
                wnd.Open(request);
                wnd.Show();
            }
        }
    }
}
