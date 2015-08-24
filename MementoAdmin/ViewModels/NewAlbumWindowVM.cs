using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MementoAdmin.ViewModels
{
    public class NewAlbumWindowVM : BaseVM
    {

        public PhotoAlbumVM Album { get; set; }
        public ICommand ApplyCommand { get; set; }

        private NewAlbumWindowVM()
        {
            ApplyCommand = new RelayCommand((_) =>
            {
                var wnd = _ as Window;
                wnd.DialogResult = true;
            });
        }

        public NewAlbumWindowVM(PhotoAlbumVM vm) : this()
        {
            Album = vm;
        }
    }
}
