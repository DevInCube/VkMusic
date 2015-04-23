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
using System.Windows.Shapes;

namespace My.VKMusic.Views
{
    /// <summary>
    /// Interaction logic for Player2.xaml
    /// </summary>
    public partial class Player2 : Window
    {
        public Player2()
        {
            InitializeComponent();

            this.DataContextChanged += Player2_DataContextChanged;
            this.Loaded += Player2_Loaded;

            this.DataContext = new MainVM();
        }

        void Player2_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null) 
                throw new Exception("DataContext not set");

            (this.DataContext as IWindowVM).Loaded(sender);
        }

        void Player2_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is IWindowVM))
                throw new Exception("invalid data context");
        }
    }
}
