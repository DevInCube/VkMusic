using My.VKMusic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.Views.DragManagement
{
    public abstract class ADragVM : ObservableObject, ICloneable, IEquatable<ADragVM>
    {

        private bool _IsDropPreview, _IsMouseHover, _IsSelected;

        public ObservableCollection<ADragVM> List { get; set; }

        public bool IsDropPreview
        {
            get { return _IsDropPreview; }
            set { _IsDropPreview = value; OnPropertyChanged("IsDropPreview"); }
        }

        public bool IsMouseHover
        {
            get { return _IsMouseHover; }
            set { _IsMouseHover = value; OnPropertyChanged("IsMouseHover"); }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public abstract object Clone();

        public abstract bool Equals(ADragVM other);
    }
}
