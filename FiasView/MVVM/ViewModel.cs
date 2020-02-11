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
using System.Windows.Controls;
using System.Data;
using FiasView.Operation.WorkWithExcel;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace FiasView.MVVM
{

    public class ViewModel : INotifyPropertyChanged
    {
        SelectFile _sf;

        /// <summary>
        /// Команда - При загрузке Стартового окна 
        /// </summary>
        //public DelegateCommand.DelegateCommand WindowsLoad
        //{
        //    get { return new DelegateCommand.DelegateCommand((x => { LoadStartUp(this); })); }
        //}

        /// <summary>
        /// Command View - Вызвать метод FindAdress
        /// </summary>
        public DelegateCommand.DelegateCommand _FindAdress
        {
            get { return new DelegateCommand.DelegateCommand((x => { FindAdress(); })); }
        }

        /// <summary>
        /// Command View - Вызвать метод SelectFile
        /// </summary>
        public DelegateCommand.DelegateCommand _SelectFile
        {
            get { return new DelegateCommand.DelegateCommand((x => { SelectFile(); })); }
        }

        /// <summary>
        /// Command View - вызвать метод CloseButton(object Window) обязательно передавать объект Window
        /// </summary>
        public DelegateCommand.DelegateCommand _CloseButton
        {
            get { return new DelegateCommand.DelegateCommand((x => { CloseButton(x); })); }
        }

        /// <summary>
        /// Command View - вызвать метод LoadRow(object DataGrid) обязательно передавать DataGrid
        /// </summary>
        public DelegateCommand.DelegateCommand _LoadRow
        {
            get { return new DelegateCommand.DelegateCommand((x => { LoadRow(x); })); }
        }

        private int _progBarLoadDB;
        private int _progBarMaxValue;
        private DataTable _dataGrid;
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
        public DataTable DataGrid
        {
            get { return _dataGrid; }
            set {
                _dataGrid = value;
                OnPropertyChanged("DataGrid");
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

        /// <summary>
        /// Command View - метод для поиска ФИАС кода по адресу 
        /// </summary>
        private void FindAdress()
        {
            MessageBox.Show("TEST");
        }

        /// <summary>
        /// Command View - метод выбора файла для загрузки
        /// </summary>
        private async void SelectFile()
        {
            _sf = new SelectFile();
            DataTable ss = await _sf.FileLoad();
            DataGrid = ss;
        }

        /// <summary>
        /// Command View - закрыть программу
        /// </summary>
        /// <param name="x">Windows type object</param>
        private void CloseButton(object x)
        {
            var mv = x as MainWindow;
            mv.Close();
        }

        /// <summary>
        /// Command View -  метод вызова загрузки строк DataGrid
        /// </summary>
        /// <param name="rows">DataGrid type Object</param>
        private void LoadRow(object rows)
        {
            var row = rows as DataGrid;
            row.LoadingRow += LoadingRows;
        }

        /// <summary>
        /// Загрузка DataGrid
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DataGridRowEventArgs</param>
        private void LoadingRows(object sender, DataGridRowEventArgs e)
        {
            const string _street = "Улица";
            const string _checkStreet = "Проверьте адрес!";
            const string _FiasColumn = "Фиас индетификатор";
            const string _checkFiasColumn = "Фиас Код не обнаружен!";
            bool _DGWithFiasCode = false;
            var i = e.Row.GetIndex() - 1;
            i = i < 0 ? 0 : 0;
            e.Row.Background = DataGrid.Rows[i][_street].ToString() == _checkStreet ? e.Row.Background = Brushes.Red : e.Row.Background = Brushes.LightGreen;
            if (_DGWithFiasCode == true) { e.Row.Background = DataGrid.Rows[i][_FiasColumn].ToString() == _checkFiasColumn ? e.Row.Background = Brushes.Red : e.Row.Background = Brushes.LightGreen; }
        }
    }
}
