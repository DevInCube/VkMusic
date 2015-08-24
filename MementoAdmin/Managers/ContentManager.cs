using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MementoAdmin.Managers
{
    public class ContentManager
    {

        public static BitmapImage LoadImage(string url)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(url);
            logo.EndInit();
            return logo;
        }

    }
}
