using MementoAdmin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using VkNET.Models.Photos;

namespace MementoAdmin.ViewModels
{
    public class PhotoVM : BaseVM
    {

        private PhotoInfo _PhotoInfo;
        private BitmapImage _ThumbImage;

        public PhotoInfo PhotoInfo
        {
            get { return _PhotoInfo; }
        }

        public BitmapImage ThumbImage
        {
            get
            {
                if (_ThumbImage == null)
                {
                    _ThumbImage = ContentManager.LoadImage(this.PhotoInfo.Src);
                    NotifyChanged("ThumbImage");
                }
                return _ThumbImage;
            }
        }

        public PhotoVM(PhotoInfo info)
        {
            this._PhotoInfo = info;
        }
    }
}
