using My.VKMusic.Models;
using My.VKMusic.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace My.VKMusic.NET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TestWindow playerWindow = new TestWindow();
            playerWindow.Show();

            VkAPI vk = new VkAPI();
            vk.DoAuth(() => {
                var audios = vk.AudioGet();
                playerWindow.InitAudioList(audios);
            });
            
        }
    }
}
