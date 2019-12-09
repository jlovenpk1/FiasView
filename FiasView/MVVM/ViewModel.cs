using FiasView.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiasView.MVVM
{
    
    public class ViewModel : INotifyPropertyChanged
    {
        LoadAllTable lat;
        private int _progBarLoadDB;
        private int _progBarMaxValue;
        private string _progBarTextDB;
        private string _progBarLoadCount;
        public int ProgBarLoadDB
        {
            get { return _progBarLoadDB; }
            set
            {
                _progBarLoadDB = value;
                OnPropertyChanged("ProgBarLoadDB");
            }
        }
        public int ProgBarMaxValue
        {
            get { return _progBarMaxValue; }
            set
            {
                _progBarMaxValue = value;
                OnPropertyChanged("ProgBarMaxValue");
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
        public string ProgBarLoadCount
        {
            get { return _progBarLoadCount; }
            set
            {
                _progBarLoadCount = value;
                OnPropertyChanged("ProgBarLoadCount");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        async public void LoadStartUp(ViewModel vm)
        {
            lat = new LoadAllTable();
            await Task.Run(new Action(() =>
            {
                lat.LoadAllTables(vm);
            }));
        }
    }
}
