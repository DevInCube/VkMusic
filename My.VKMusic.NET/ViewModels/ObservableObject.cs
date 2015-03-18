using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.VKMusic.ViewModels
{
    public class ObservableObject: INotifyPropertyChanged
    {

        #region propchanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }
}
