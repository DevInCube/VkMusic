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
            VkAPI vk = new VkAPI();
            vk.DoAuth(() => {
                var audios = vk.AudioGet();
                AudioFile audio = new AudioFile(audios[0]);
                audio.Play();
                return;
            });
            testWindow w = new testWindow();
            w.Show();
        }
    }
}
