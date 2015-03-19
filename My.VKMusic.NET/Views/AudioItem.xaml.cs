using My.VKMusic.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for AudioItem.xaml
    /// </summary>
    public partial class AudioItem : UserControl
    {

        Brush artistFG = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B587A"));
        Brush currentColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#45668e"));
        Brush hoverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1E7ED"));

        public AudioItem()
        {
            InitializeComponent();

            this.DataContextChanged += AudioItem_DataContextChanged;
            item.MouseEnter += item_MouseEnter;
            item.MouseLeave += item_MouseLeave;
            this.MouseUp += AudioItem_MouseUp;
        }

        void AudioItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //(this.DataContext as AudioFile).Play();
        }

        void AudioItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AudioFile file = this.DataContext as AudioFile;
            file.PropertyChanged += file_PropertyChanged;
        }

        void file_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected"))
            {
                item.Background = (this.DataContext as AudioFile).IsSelected ? currentColor : Brushes.WhiteSmoke;
                artistLabel.Foreground = (this.DataContext as AudioFile).IsSelected ? Brushes.White : artistFG;
                titleLabel.Foreground = divLabel.Foreground =
                    (this.DataContext as AudioFile).IsSelected ? Brushes.White : Brushes.Black;
            }
        }

        void item_MouseLeave(object sender, MouseEventArgs e)
        {
            item.Background = (this.DataContext as AudioFile).IsSelected ? currentColor : Brushes.WhiteSmoke;
            Cursor = Cursors.Arrow;
        }

        void item_MouseEnter(object sender, MouseEventArgs e)
        {
            item.Background = (this.DataContext as AudioFile).IsSelected ? currentColor : hoverColor;
            Cursor = Cursors.Hand;
        }
    }
}
