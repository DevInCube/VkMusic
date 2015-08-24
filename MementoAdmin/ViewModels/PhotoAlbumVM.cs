using MementoAdmin.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using VkNET.Models.Photos;

namespace MementoAdmin.ViewModels
{
    public class PhotoAlbumVM : BaseVM
    {

        private PhotoAlbumInfo _AlbumInfo;
        private BitmapImage _ThumbImage;

        public PhotoAlbumInfo AlbumInfo
        {
            get { return _AlbumInfo; }
        }

        public string Title
        {
            get { return _AlbumInfo.Title; }
            set { _AlbumInfo.Title = value; NotifyChanged("Title"); }
        }

        public string Description
        {
            get { return _AlbumInfo.Description; }
            set { _AlbumInfo.Description = value; NotifyChanged("Description"); }
        }

        public BitmapImage ThumbImage
        {
            get
            {
                if (_ThumbImage == null)
                {
                    this.ThumbImage = ContentManager.LoadImage(this.AlbumInfo.ThumbSrc);                    
                }
                return _ThumbImage;
            }
            set
            {
                this._ThumbImage = value;
                NotifyChanged("ThumbImage");
            }
        }

        public ObservableCollection<PhotoVM> Photos { get; private set; }

        public PhotoAlbumVM()
        {
            this._AlbumInfo = new PhotoAlbumInfo();
            this.Photos = new ObservableCollection<PhotoVM>();
        }

        public PhotoAlbumVM(PhotoAlbumInfo info)
        {
            this._AlbumInfo = info;
            this.Photos = new ObservableCollection<PhotoVM>();
        }        
    }
}
