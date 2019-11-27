using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FiasView.MVVM
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int _progBarLoadDB;
        private string _progBarTextDB;
        public int ProgBarLoadDB
        {
            get { return _progBarLoadDB; }
            set
            {
                _progBarLoadDB = value;
                OnPropertyChanged("ProgBarLoadDB");
            }
        }
        public string ProgBarTextDB
        {
            get { return _progBarTextDB; }
            set
            {
                _progBarTextDB = value;
                OnPropertyChanged("ProgBarTextDB");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
