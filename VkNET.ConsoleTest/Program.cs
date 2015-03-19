using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkNET.Models;

namespace VkNET.ConsoleTest
{
    
    class Program 
    {

        private static VkAPI vk;

        [STAThread]
        static void Main(string[] args)
        {
            vk = new VkAPI(new AuthProvider());
            vk.DoAuth(() =>
            {
                Console.WriteLine("Auth OK");
                Console.WriteLine(vk.AudioGetLyrics(2428970));
                //Console.WriteLine(vk.AudioSearch("Muse").Count);
                //Console.WriteLine(vk.AudioGetCount(vk.AuthData.UserId));
                //var pop = vk.AudioGetPopular(new AudioPopularSettings() { OnlyEng = true });
                return;
            });
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
