using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                var t = vk.AudioSearch("Muse");
                Console.WriteLine(t.Count);
                return;
            });
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
