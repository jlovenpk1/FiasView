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
using FiasView.UI;
using ClosedXML.Excel;
using System.Windows.Threading;

namespace FiasView.MVVM
{
    /// <summary>
    /// ViewModel реализует интерфейс INotifyPropertyChanged, почитать об этом: https://docs.microsoft.com/ru-ru/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8
    /// <para>Так же почитать можно тут: https://metanit.com/sharp/wpf/11.2.php </para>
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        #region Здесь все Command
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

        /// <summary>
        /// Command View - Вызывается при старте программы, для получение Window
        /// </summary>
        public DelegateCommand.DelegateCommand _Start
        {
            get { return new DelegateCommand.DelegateCommand((x => { Start(x); })); }
        }

        /// <summary>
        /// Command View - Вызывает поиск ФИАС адресов из DataGrid
        /// </summary>
        public DelegateCommand.DelegateCommand _StartSearch
        {
            get { return new DelegateCommand.DelegateCommand((x => { StartSearch(x); })); }
        }

        /// <summary>
        /// Command View - Сохранение DataGrid в Excel
        /// </summary>
        public DelegateCommand.DelegateCommand _StartSave
        {
            get { return new DelegateCommand.DelegateCommand(x => { StartSave(); }); }
        }

        /// <summary>
        /// Command View - Увеличить или уменьшить размер окна
        /// </summary>
        public DelegateCommand.DelegateCommand _MaxMinWindow
        {
            get { return new DelegateCommand.DelegateCommand(x => { MaxMinWindow(); }); }
        }

        /// <summary>
        /// Command View - Свернуть окно
        /// </summary>
        public DelegateCommand.DelegateCommand _HideWindow
        {
            get { return new DelegateCommand.DelegateCommand(x => { HideWindow(); }); }
        }
        #endregion
        #region Все переменные и экземпляры классов
        /// <summary>
        /// View - Форма InputFrom
        /// </summary>
        private InputForm _inputform;
        /// <summary>
        /// Command Parametr - получаем компонент DataGrid для работы с ним
        /// </summary>
        private DataGrid _grid;
        /// <summary>
        /// Model - тут у нас используется класс SelectFile
        /// </summary>
        private SelectFile _sf;
        /// <summary>
        /// View - Окно
        /// </summary>
        private Window _window;
        /// <summary>
        /// Результат того, что мы получили после обработки Excel файла
        /// </summary>
        private DataTable _resultFileLoad;
        /// <summary>
        /// View - экземпляр окна ProgressBar
        /// </summary>
        private progressBar _progress;
        /// <summary>
        /// Model - экземпляр класса LoadExcelToGrid используется для всей работы по поиску ФИАС кода
        /// 
        /// </summary>
        private WorkWithAllAddress _workWithAdress;
        /// <summary>
        /// ViewModel - переменная главной сетки
        /// </summary>
        private DataTable _dataGrid;
        /// <summary>
        /// Булевая  переменная для проверки, есть ли столбец Fias Code в DataGrid
        /// </summary>
        private bool _DGWithFiasCode = false;
        /// <summary>
        /// ViewModel - переменная для ProgressBar - Value
        /// </summary>
        private int _progBarValue;
        /// <summary>
        /// ViewModel - переменная для ProgressBar - MaxValue
        /// </summary>
        private int _progBarMaxValue;
        /// <summary>
        /// ViewModel - переменная для ProgressBar, в частности для TextBox - Content
        /// </summary>
        private string _progBarTextDB;
        /// <summary>
        /// ViewModel - переменная для ProgressBar, в частности для Label - Content; 
        /// <para>Отвечает за текст, пример: Загруженно 95 / 10000</para>
        /// </summary>
        private string _progBarLoadCount;
        /// <summary>
        /// ViewModel - переменная для MainWindow. Выводится количество поподаний по ФИАСу
        /// </summary>
        private string _countRows;
        /// <summary>
        /// Значение Value у ProgressBar
        /// </summary>
        public int ProgBarValue
        {
            get { return _progBarValue; }
            set
            {
                _progBarValue = value;
                OnPropertyChanged("ProgBarValue");
            }
        }
        /// <summary>
        /// Максимальное значение ProgressBar {MaxValue}
        /// </summary>
        public int ProgBarMaxValue
        {
            get { return _progBarMaxValue; }
            set
            {
                _progBarMaxValue = value;
                OnPropertyChanged("ProgBarMaxValue");
            }
        }
        /// <summary>
        /// TextBox текст на ProgressBar
        /// </summary>
        public string ProgBarTextDB
        {
            get { return _progBarTextDB; }
            set
            {
                _progBarTextDB = value;
                OnPropertyChanged("ProgBarTextDB");
            }
        }
        /// <summary>
        /// Label, отображает значение в стиле: Загружено 0/1000;
        /// </summary>
        public string ProgBarLoadCount
        {
            get { return _progBarLoadCount; }
            set
            {
                _progBarLoadCount = value;
                OnPropertyChanged("ProgBarLoadCount");
            }
        }
        /// <summary>
        /// Отображает количество строк, которым было присвоен FIAS CODE
        /// </summary>
        public string CountRows
        {
            get { return _countRows; }
            set { _countRows = value;
                OnPropertyChanged("CountRows");
            }
        }
        /// <summary>
        /// ViewModel - тут храниться инфа о MainGrid
        /// </summary>
        public DataTable MainGrid
        {
            get { return _dataGrid; }
            set {
                _dataGrid = value;
                OnPropertyChanged("MainGrid");
            }
        }
        #endregion
        #region Реализация INPC
        /// <summary>
        /// Реализация INotifyPropertyChanged - Метод отвечает за обновление переменной(свойства) участвующих в привязки. 
        /// </summary>
        /// <param name="prop">Имя свойства(переменной) участвующих в привязке</param>
        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            #region Для тех, кто не понимает работу PropertyChanged?.Invoke(object sender, PropertyChangedEventArgs e)
            /* 
             * PropertyChanged - событие. PropertyChanged?.Invoke(object sender, PropertyChangedEventArgs e):
             * - PropertyChanged?. - Проверка события на null, если оно не null выполняется Invoke
             * - object sender - что бы передаем? Правильно this, т.е действующий ViewModel 
             * - PropertyChangedEventArgs e - Какое свойство мы изменяем? Правиольно то, что мы передаем string prop.
             * Для более ясного понятие что за prop и как оно функционирует, советую в режиме дабага пошагово прогуляться по исполняемому коду.
             * 
             */
            #endregion
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Событие PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnModelPropertyChanged - данный метод отвечает за реакцию на изменения в Model во время выполнения поставленной задачей от ViewModel.
        /// </summary>
        /// <param name="parametr">Переменная которая изменяется в Model</param>
        private void OnModelPropertyChanged(string parametr)
        {
            switch (parametr)
            {
                case "ProgBarValue":
                    ProgBarValue = _workWithAdress.ProgBarValue;
                    break;
                case "ProgBarMaxValue":
                    ProgBarMaxValue = _workWithAdress.ProgBarMaxValue;
                    break;
                case "ProgBarTextDB":
                    ProgBarTextDB = _workWithAdress.ProgBarTextDB;
                    break;
                case "ProgBarLoadCount":
                    ProgBarLoadCount = _workWithAdress.ProgBarLoadCount;
                    break;
                case "CountRows":
                    CountRows = _workWithAdress.CountRows;
                    break;
                default:
                    ;
                    break;
            }

            OnPropertyChanged(parametr);
        }
        #endregion
        #region Методы для реализации Command
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
        private async void FindAdress()
        {
            string _address = string.Empty;
            string _result = string.Empty;
            _inputform = new InputForm();
            _inputform._start.Click += (s, e) => { _inputform.Close(); };
            _inputform.Closed += (s, e) => { _address = _inputform._Address.Text; };
            _inputform.ShowDialog();
            await Task.Run(new Action(() =>
            {
                _progress.Dispatcher.BeginInvoke(new Action(delegate
                {
                    _progress.Show();
                    _progress._progbar.IsIndeterminate = false;
                }));
                _workWithAdress.ParametrChange += OnModelPropertyChanged; // если в классе LoadExcelToGrid сработает событие ParametrChange выполняем OnModelPropertyCanged
                ProgBarLoadCount = "";
                ProgBarMaxValue = 0;
                ProgBarTextDB = "";
                ProgBarValue = 0;
                if (_workWithAdress._addr != null && _workWithAdress._house30 != null)
                {
                    _result = _workWithAdress.GetFiasCode(_address);
                }
                else
                {
                    _workWithAdress.LoadCacheAdrr(); // грузим кэш улиц
                    _workWithAdress.LoadCacheHouse(); // грузим Кэш Домов
                    _result = _workWithAdress.GetFiasCode(_address);
                }
            }));
        }
        
        /// <summary>
        /// Command View - Свернуть окно
        /// </summary>
        private  void HideWindow()
        {
            _window.WindowState = WindowState.Minimized;
        }
        
        /// <summary>
        /// Command View - Увеличить или уменьшить размер окна
        /// </summary>
        private  void MaxMinWindow()
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        /// Command View - Сохранить DataGrid в Excel
        /// </summary>
        private  void StartSave()
        {
            var wb = new XLWorkbook();
            wb.Worksheets.Add(_resultFileLoad);
            string _name = "Результат проверки.xlsx";
            wb.SaveAs(_name);
            MessageBox.Show("Файл сохранен!");
        }

        /// <summary>
        /// Command View - метод для получение Window и работы с ним в будущем 
        /// </summary>
        private void Start(object window)
        {
            _window = window as Window;
            _workWithAdress = new WorkWithAllAddress();
            _progress = new progressBar();
        }

        /// <summary>
        /// Command View - метод выбора файла для загрузки
        /// </summary>
        private async void SelectFile()
        {
            _sf = new SelectFile();
            _resultFileLoad = await _sf.FileLoad();
            MainGrid = _resultFileLoad;
            
        }

        /// <summary>
        /// Command View - закрыть программу
        /// </summary>
        /// <param name="x">Windows type object</param>
        private void CloseButton(object x)
        {
            var mv = x as MainWindow;
            mv.Close();
            Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        /// <summary>
        /// Command View - начать поиск ФИАС индетификаторов из DataGrid
        /// </summary>
        /// <param name="x"></param>
        private async void StartSearch(object row)
        {
            var _row = row as DataGrid;
            _window.Hide();
            await Task.Run(new Action(() =>
            {
                _progress.Dispatcher.BeginInvoke(new Action(delegate
                {
                    _progress.Show();
                    _progress._progbar.IsIndeterminate = false;
                }));
                _workWithAdress.ParametrChange += OnModelPropertyChanged; // если в классе LoadExcelToGrid сработает событие ParametrChange выполняем OnModelPropertyCanged
                ProgBarLoadCount = "";
                ProgBarMaxValue = 0;
                ProgBarTextDB = "";
                ProgBarValue = 0;
                if (_workWithAdress._addr != null && _workWithAdress._house30 != null)
                {
                    _resultFileLoad = _workWithAdress.GetFiasCode(_resultFileLoad);
                }
                else
                {
                    _workWithAdress.LoadCacheAdrr(); // грузим кэш улиц
                    _workWithAdress.LoadCacheHouse(); // грузим Кэш Домов
                    _resultFileLoad = _workWithAdress.GetFiasCode(_resultFileLoad);
                }
            }));
            _window.Show();
            _progress.Close();
            _DGWithFiasCode = true;
            MainGrid = _resultFileLoad;
            _grid.Items.Refresh();
        }

        /// <summary>
        /// Command View -  метод вызова загрузки строк DataGrid
        /// </summary>
        /// <param name="rows">DataGrid type Object</param>
        private void LoadRow(object rows)
        {
            _grid = rows as DataGrid;
            _grid.LoadingRow += LoadingRows;
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
            var i = e.Row.GetIndex();
            i = i >= MainGrid.Rows.Count ? i - 1 : i;
            e.Row.Background = MainGrid.Rows[i][_street].ToString() == _checkStreet ? e.Row.Background = Brushes.Red : e.Row.Background = Brushes.LightGreen;
            if (_DGWithFiasCode == true) { e.Row.Background = MainGrid.Rows[i][_FiasColumn].ToString() == _checkFiasColumn ? e.Row.Background = Brushes.Red : e.Row.Background = Brushes.LightGreen; }
        }
        #endregion
    }
}
