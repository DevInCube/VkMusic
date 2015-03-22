using My.VKMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VkNET.Models;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for TestWindowPerf.xaml
    /// </summary>

    public partial class TestWindowPerf : Window
    {
        ObservableCollection<AudioFile> items;

        public TestWindowPerf()
        {
            InitializeComponent();
            items = new ObservableCollection<AudioFile>();
            foreach (var i in Enumerable.Range(0, 10000).Select(i => new AudioFile(new AudioFileInfo(){
                Artist = "Artist " + i,
                Title = "Title " + i,
            })))
            { items.Add(i); }
            DataContext = items;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var i in Enumerable.Range(0, 1000).Select(i => new AudioFile(new AudioFileInfo()
            {
                Artist = "NewArtist " + i,
                Title = "NewTitle " + i,
            })))
            { items.Add(i); }
        }
    }
}
