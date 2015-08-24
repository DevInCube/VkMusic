using MementoAdmin.Auth;
using MementoAdmin.Managers;
using MementoAdmin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VkNET.Models;
using VkNET.Models.Photos;

namespace MementoAdmin.ViewModels
{
    public class MainVM : BaseVM
    {

        private VkNET.VkAPI vk;

        private bool _LoggedIn;
        private PhotoAlbumVM _SelectedAlbum;
        private int _SelectedTabIndex;
        private PhotoVM _SelectedPhoto;
        private string _AudioSearch;

        public bool LoggedIn
        {
            get { return _LoggedIn; }
            set { _LoggedIn = value; NotifyChanged("LoggedIn"); }
        }

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                NotifyChanged("SelectedTabIndex");
            }
        }

        public ObservableCollection<PhotoAlbumVM> Albums { get; private set; }
        public PhotoAlbumVM SelectedAlbum
        {
            get { return _SelectedAlbum; }
            set
            {
                _SelectedAlbum = value;
                NotifyChanged("SelectedAlbum");
            }
        }

        public PhotoVM SelectedPhoto
        {
            get { return _SelectedPhoto; }
            set
            {
                _SelectedPhoto = value;
                NotifyChanged("SelectedPhoto");
            }
        }

        public string AudioSearch { 
            get { return _AudioSearch; }
            set
            {
                _AudioSearch = value;
                NotifyChanged("AudioSearch");
            }
        }

        public ObservableCollection<AudioVM> AudioResults { get; private set; }

        public ICommand LogInCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand UploadPhotosCommand { get; set; }
        public ICommand PhotoRemoveCommand { get; set; }
        public ICommand PhotoMakeCoverCommand { get; set; }
        public ICommand CreatePhotoAlbumCommand { get; set; }
        

        public MainVM()
        {
            vk = new VkNET.VkAPI(new IEAuthProvider());
            Albums = new ObservableCollection<PhotoAlbumVM>();
            AudioResults = new ObservableCollection<AudioVM>();

            LogInCommand = new RelayCommand((obj) =>
            {
                vk.DoAuth(() =>
                {
                    LoggedIn = true;
                });
            });
            SearchCommand = new RelayCommand((obj) => {
                this.AudioResults.Clear();
                var q = this.AudioSearch;
                var opts = new AudioSearchSettings() { };
                var audios = vk.AudioSearch(q, opts);
                foreach (var audio in audios)
                    this.AudioResults.Add(new AudioVM(audio));
            });
            UploadPhotosCommand = new RelayCommand((_) => {
                var albumInfo = this.SelectedAlbum.AlbumInfo;
                var uploadUrl = vk.Photos_GetUploadServer(albumInfo);
                
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".png";
                dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                dlg.Multiselect = true;
                Nullable<bool> result = dlg.ShowDialog();                
                if (result == true)
                {
                    string[] files = dlg.FileNames;
                    foreach (var fn in files)
                    {
                        IList<PhotoInfo> newPhotos = vk.UploadPhoto(fn, uploadUrl);
                        foreach (var p in newPhotos)
                        {
                            this.SelectedAlbum.Photos.Add(new PhotoVM(p));
                        }
                    }
                }
            });
            PhotoRemoveCommand = new RelayCommand((_) => {
                var photo = this.SelectedPhoto;
                vk.Photos_Delete(photo.PhotoInfo);
                this.SelectedAlbum.Photos.Remove(photo);
                this.SelectedPhoto = null;
            });
            PhotoMakeCoverCommand = new RelayCommand((_) =>
            {
                var photo = this.SelectedPhoto;
                vk.Photos_MakeCover(photo.PhotoInfo);
                this.SelectedAlbum.ThumbImage = photo.ThumbImage;                
            });
            CreatePhotoAlbumCommand = new RelayCommand((_) =>
            {
                var window = _ as MainWindow;
                var vm = new PhotoAlbumVM();
                var wnd = new NewAlbumWindow();
                wnd.DataContext = new NewAlbumWindowVM(vm);
                wnd.Owner = window;
                var res = wnd.ShowDialog();
                if (res == true)
                {
                    var album = vm.AlbumInfo;
                    vk.Photos_CreateAlbum(ref album);                    
                    this.Albums.Add(new PhotoAlbumVM(album));
                }
            });


            this.PropertyChanged += MainVM_PropertyChanged;
        }

        void MainVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case ("SelectedTabIndex"): OnTabChanged(this.SelectedTabIndex); break;
                case ("SelectedAlbum"): OnAlbumChanged(this.SelectedAlbum); break;
            }
        }

        private void OnAlbumChanged(PhotoAlbumVM album)
        {
            if (album.Photos.Count == 0)
            {
                var photos = vk.Photos_Get(album.AlbumInfo);
                foreach (var p in photos)
                    album.Photos.Add(new PhotoVM(p));
            }
        }

        private void OnTabChanged(int tabIndex)
        {
            switch (tabIndex)
            {
                case (1): OnPhotosTabEnter(); break;
            }
        }

        private void OnPhotosTabEnter()
        {
            if (this.Albums.Count == 0)
            {
                var albums = vk.Photos_GetAlbums();
                foreach (var a in albums)
                    this.Albums.Add(new PhotoAlbumVM(a));
            }
        }
    }
}
