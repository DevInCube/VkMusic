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

        private bool _IsDropPreview;

        public ObservableCollection<ADragVM> List { get; set; }

        public bool IsDropPreview
        {
            get { return _IsDropPreview; }
            set { _IsDropPreview = value; OnPropertyChanged("IsDropPreview"); }
        }

        public abstract object Clone();

        public abstract bool Equals(ADragVM other);
    }
}
