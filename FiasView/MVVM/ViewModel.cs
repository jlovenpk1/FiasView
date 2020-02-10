using FiasView.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FiasView.MVVM.DelegateCommand;
namespace FiasView.MVVM
{
    
    public class ViewModel : INotifyPropertyChanged
    {
        LoadAllTable _lat;
        MainWindow _mv;
        ManagerForms _mf;
        StartUp _st;
        /// <summary>
        /// Команда - При загрузке Стартового окна 
        /// </summary>
        //public DelegateCommand.DelegateCommand WindowsLoad
        //{
        //    get { return new DelegateCommand.DelegateCommand((x => { LoadStartUp(this); })); }
        //}

        private int _progBarLoadDB;
        private int _progBarMaxValue;
        private string _progBarTextDB;
        private string _progBarLoadCount;
        private string _countRows;
        private Visibility _isvisible;
        public Visibility isVisible
        {
            get { return _isvisible; }
            set
            {
                _isvisible = value;
                OnPropertyChanged("isVisible");
            }
        }
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
        public string CountRows
        {
            get { return _countRows; }
            set { _countRows = value;
                OnPropertyChanged("CountRows");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Более не используется, но может пригодиться
        //async public void LoadStartUp(ViewModel vm)
        //{
        //    _lat = new LoadAllTable();
        //    _mf = new ManagerForms();

        //    await Task.Run(new Action(() =>
        //    {
        //        _lat.LoadAllTables(vm);
        //    }));
        //    isVisible = Visibility.Hidden;
        //    _mf._mv.Show();
        //}
        #endregion

    }
}
