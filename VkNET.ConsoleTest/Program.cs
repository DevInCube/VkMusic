using My.VkMusic.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            vk = new VkAPI(new IEAuthProvider());
            vk.DoAuth(() =>
            {
                Console.WriteLine("Auth OK");
                var albums = vk.Photos_GetAlbums();
                var photos = vk.Photos_Get(albums[0]);
                foreach (var p in photos)
                {
                    Console.WriteLine(p);
                }
                return;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int total = 0;
                vk.AudioGet(null, null, out total);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                //vk.AudioGetAlbums().ForEach(a => Console.WriteLine(a));
                //vk.AudioDeleteAlbum(1111);
                //vk.AudioReorder(123, 123, 15, null);
                //Console.WriteLine(vk.AudioAddAlbum("Root/Test"));
                //Console.WriteLine(vk.AudioGetLyrics(2428970));
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
