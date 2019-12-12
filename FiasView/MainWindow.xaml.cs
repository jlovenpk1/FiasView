using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClosedXML.Excel;
using System.Data;
using System.Threading;
using FiasView.Forms;
using FiasView.Operation.WorkWithExcel;
using FiasView.Operation.OperationWithDBF;
using FiasView.UI;
using FiasView.MVVM;
using System.Collections;

namespace FiasView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _street = "Улица";
        private const string _checkStreet = "Проверьте адрес!";
        private const string _FiasColumn = "Фиас индетификатор";
        private const string _checkFiasColumn = "Фиас Код не обнаружен!";
        public bool _DGWithFiasCode = false;
        private Dictionary<int,house30> _cacheAdrr;
        LoadExcelToGrid _excelWork;
        OpenFileDialog _fileOpen;
        XLWorkbook _workbook;
        DataTable _data;
        Window1 _selectColumn;
        Model1 db;
        DBFtoSQL dts;
        progressBar _progress;
        ViewModel vm;
        DataTable _oldData;
        string _path;
        public string _firstColumn = "";
        public string _secondColumn = "";
        
        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel();
            _data = new DataTable();
            _dataGrid.CanUserAddRows = false;
            this.DataContext = vm;

        }

        /// <summary>
        /// Загружаем файл 
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        async private void FileLoad_Click(object sender, RoutedEventArgs e)
        {
            _fileOpen = new OpenFileDialog
            {
                Filter = "Excel File 2003 (*.xls,*.xlsx)|*.txt;*.xlsx|All files (*.*)|*.*"
                //any settings
            };
            if (_fileOpen.ShowDialog() == true) {_path = _fileOpen.FileName;}
            _excelWork = new LoadExcelToGrid();
            
            _selectColumn = new Window1();
            _progress = new progressBar(vm);
            _selectColumn.ShowDialog();
            _selectColumn_Closed();
            _fileOpen.Reset();
            _excelWork = new LoadExcelToGrid();
            _data.Clear();
            await Task.Run(new Action(() =>
            {
                _progress.Dispatcher.BeginInvoke(new Action(delegate 
                {
                    _progress.Show();
                    _progress._progbar.IsIndeterminate = true; }));
                    _data = _excelWork.OpenExcel(_workbook = new XLWorkbook(_path), _firstColumn, _secondColumn);
                }));
            _oldData = _data;
            _dataGrid.ItemsSource = _data.DefaultView;
            _progress.Hide();
            
            
        }

        /// <summary>
        /// получаем названия нужных колонок
        /// </summary>
        private void _selectColumn_Closed()
        {
            _firstColumn = _selectColumn._adresColumn.Text;
            _secondColumn = _selectColumn._fiadColumn != null ? _secondColumn = _selectColumn._fiadColumn.Text : "";
        }

        private void DictAutoswitch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            db = new Model1();
            
                if (db.Database.Exists() == false)
                {
                    if(MessageBox.Show("База отсутствует, создать новую?", "База данных не обнаруженна",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("Создано");
                        db.Database.Create();
                        dts = new DBFtoSQL();
                        dts.GetSQLData();
                    } 
                }

                if (db.Database.Exists() == true)
                {
                    if(MessageBox.Show("База данных существует, обновить?","База данных обнаруженна",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("Обновляемся");
                        dts = new DBFtoSQL();
                    db.Database.Delete();
                    db.Database.Create();
                    dts.GetSQLData();
                    }
                }
        }

        async void _start_Click(object sender, RoutedEventArgs e)
        {
            
            _data.Dispose();
            _data = new DataTable();
            this.Hide();
            await Task.Run(new Action(() =>
            {
                _progress.Dispatcher.BeginInvoke(new Action(delegate
                {
                    _progress.Show();
                    _progress._progbar.IsIndeterminate = false;
                }));
                _data = _excelWork.GetFiasCode(vm);
            }));
            this.Show();
            _progress.Hide();
            _DGWithFiasCode = true;
            _oldData = _data;
            _dataGrid.ItemsSource = _data.DefaultView;
            _dataGrid.Items.Refresh();
            #region Для тестов с персингом адреса
            //string adress = "414038, Астраханская обл, Астрахань г, Грановский пер, дом № 59"; 
            //List<string> adress = new List<string>() {
            //    "414000, Астраханская обл, Астрахань г, Урицкого ул/Тихий пер., дом № 29/7, литера А, помещение 1",
            //    //"414032, Астраханская обл, Астрахань г, Жилая ул, дом № 1 п 8а",
            //    //"414000, Астраханская обл, Астрахань г, Красная Набережная ул, дом № 92 А, помещение 006",
            //    //"Кировский ул.Кр.Набережная д.92\"А\"",
            //    //"414004, Астраханская обл, Астрахань г, С.Перовской ул, дом № 73, помещение 4",
            //    //"Астраханская обл, Астрахань г, Коммунистическая ул/Молодой Гвардии ул, дом № 8/8, помещение 003",
            //    //"Астраханская обл, Астрахань г, Коммунистическая ул",
            //};
            //for (int c = 0; c < adress.Count; c++)
            //{

            //string test = string.Empty;
            //var newLine = adress[c].Replace("№", "");
            //List<string> _pAdress = newLine.Split(new char[] { ',', }).ToList();
            //string error = string.Empty;
            //List<string> city = new List<string>() { "город.", "г.", "город", "г", "город. ", "г. ", "город ", "г ", " город.", " г.", " город", " г" };
            //List<string> street = new List<string>() { " улица", " ул", " у", " ул.", " улица.", " у.", " пер"," пер.", " переулок", "улица ", "ул ", "у ", "ул. ", "улица. ", " у. ", "пер ","пер. ", "переулок ", "улица", "ул", "у", "ул.", "улица.", "у.", "пер", "переулок", "пер." };
            //List<string> house = new List<string>() { "дом.", "д.", "дом", "д", " дом.", " д.", " дом", " д", "дом. ", "д. ", "дом ", "д " };

            //try
            //{

            //    for (int i = 0; i < _pAdress.Count; i++)
            //    {
            //        for (int x = 0; x < city.Count; x++)
            //        {
            //            if (_pAdress[i].StartsWith(city[x]))
            //            {
            //                test += _pAdress[i].Replace(city[x], "");
            //                    var posStart = test.IndexOf(" ");
            //                    test = test.Remove(posStart, 1);
            //                    var posLast = test.LastIndexOf(" ");
            //                    test = test.Remove(posLast, 1);
            //                    break;
            //            }
            //            else if (_pAdress[i].EndsWith(city[x]))
            //            {
            //                test += _pAdress[i].Replace(city[x], "");
            //                    break;
            //            }
            //        }

            //        for (int x = 0; x < street.Count; x++)
            //        {
            //            if (_pAdress[i].StartsWith(street[x]))
            //            {
            //                    _pAdress[i].Replace(street[x], "");
            //                    var posStart = _pAdress[i].IndexOf(" ");
            //                    _pAdress[i].Remove(posStart, 1);
            //                    var posLast = _pAdress[i].LastIndexOf(" ");
            //                    test += _pAdress[i].Remove(posLast, 1);
            //                    break;
            //            }
            //            else if (_pAdress[i].EndsWith(street[x]))
            //            {
            //                    var text = _pAdress[i].Replace(street[x], "");
            //                    var posStart = text.IndexOf(" ");
            //                    text = text.Remove(posStart, 1);
            //                    if (text.IndexOf("/") > 0)
            //                    {
            //                        text = text.Remove(text.IndexOf("/"),text.Length - text.IndexOf("/"));
            //                        text = text.Remove(text.IndexOf("ул"), text.Length - text.IndexOf("ул"));
            //                    }
            //                    var posLast = text.LastIndexOf(" ");
            //                    test += text.Remove(posLast, 1);
            //                    break;
            //            }
            //        }

            //        for (int x = 0; x < house.Count; x++)
            //        {
            //            if (_pAdress[i].EndsWith(house[x]))
            //            {
            //                test += _pAdress[i].Replace(house[x], "");
            //                break;
            //            }
            //            else if (_pAdress[i].StartsWith(house[x]))
            //            {
            //                    var text = _pAdress[i].Replace(house[x], "");
            //                    var post = text.IndexOf(" ");
            //                    text = text.Remove(post, 1);
            //                    var pos = text.LastIndexOf(" ");
            //                    text = text.Remove(pos, 1);
            //                    test += text;
            //                    break;
            //            }
            //        }

            //    }
            //    MessageBox.Show(test);
            //}
            //catch
            //{
            //    MessageBox.Show("Ошибка: " + error);
            //}
            //}
            #endregion
            #region oldShit
            //for (int i = 0; i < ArrayString.Length; i++)
            //{
            //    string name = ArrayString[i];
            //    var query = _db.addrob30.Where(q => q.OFFNAME == name).ToList();
            //    if (query.Count == 0)
            //    {

            //    } else
            //    {
            //        foreach (addrob30 x in query)
            //        {
            //            if (x.AOLEVEL == 4 && ArrayString[i + 1].ToString() != "Федерация")
            //            {
            //                //MessageBox.Show("Город!"+x.OFFNAME);
            //            }
            //            if (x.AOLEVEL == 7 && ArrayString[i - 1].ToString() != "область" && ArrayString[i+1] != "Федерация")
            //        {
            //            //MessageBox.Show("Улица!" + x.OFFNAME);
            //            fias_id = x.AOGUID;
            //        }
            //        }
            //    var _q = _db.house30.Where(q => q.AOGUID == fias_id).ToList();
            //    if (_q.Count != 0)
            //    {
            //        for (int j = i; j < ArrayString.Length; j++)
            //        {
            //            foreach (house30 x in _q)
            //            {
            //                if (x.HOUSENUM == ArrayString[j])
            //                {
            //                    MessageBox.Show("Дом: " + x.HOUSENUM + "FIAS: " + x.HOUSEID);
            //                    break;
            //                }  
            //            }
            //            j++;
            //        }

            //    }
            //    }

            //}
            #endregion
        }

        private void UpdateAddrCorrect_Click(object sender, RoutedEventArgs e)
        {
            UpdateAdress update = new UpdateAdress();
            update.GetAllAdressWithRoom();
        }

        private void FindOneAdress_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int i = e.Row.GetIndex();
            //var cc = _data.Rows[i]["Улица"]; // для тестов
            e.Row.Background = _oldData.Rows[i][_street].ToString() == _checkStreet ? e.Row.Background = Brushes.Red : e.Row.Background = Brushes.LightGreen;
            if (_DGWithFiasCode == true) { e.Row.Background = _oldData.Rows[i][_FiasColumn].ToString() == _checkFiasColumn ? e.Row.Background = Brushes.Red : e.Row.Background = Brushes.LightGreen; }
            
        }
    }
}
