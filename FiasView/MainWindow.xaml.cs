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

namespace FiasView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoadExcelToGrid _excelWork;
        OpenFileDialog _fileOpen;
        XLWorkbook _workbook;
        DataTable _data;
        Window1 _selectColumn;
        Model1 db;
        DBFtoSQL dts;
        progressBar _progress;
        string _path;
        public string _firstColumn = "";
        public string _secondColumn = "";
        
        public MainWindow()
        {
            InitializeComponent();
            
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
            _data = new DataTable();
            _selectColumn = new Window1();
            _progress = new progressBar();
            _selectColumn.ShowDialog();
            _selectColumn_Closed();
            _fileOpen.Reset();
            
            await Task.Run(new Action(() =>
            {
                _progress.Dispatcher.BeginInvoke(new Action(delegate { _progress.Show(); _progress._progbar.IsIndeterminate = true; }));
                _data = new LoadExcelToGrid().OpenExcel(_workbook = new XLWorkbook(_path), _firstColumn, _secondColumn);
            }));
            _progress.Close();
            _dataGrid.ItemsSource = _data.DefaultView;
        }

        /// <summary>
        /// получаем названия нужных колонок
        /// </summary>
        private void _selectColumn_Closed()
        {
            _firstColumn = _selectColumn._adresColumn.Text;
            _secondColumn = _selectColumn._fiadColumn != null ? _secondColumn = _selectColumn._fiadColumn.Text : "";
        }

        /// <summary>
        /// DoWork - включить прогресс бар
        /// </summary>
        private void DoWork_ProgressBar()
        {
            ProgressWork.IsIndeterminate = true;
        }

        /// <summary>
        /// StopWork - выключить прогресс бар
        /// </summary>
        private void StopWork_ProgressBar()
        {
            ProgressWork.IsIndeterminate = false;
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

        private void _start_Click(object sender, RoutedEventArgs e)
        {
            string adress = "414050, Российская Федерация, область Астраханская ,город. Астрахань, улица Николая Островского, 76а дом, кв.1";
            string test = string.Empty;
            int oldCityIndex = 0;
            int oldStreetIndex = 0;
            bool checkCity = false, checkStreet = false;
            List<string> _pAdress = adress.Split(new char[] { ','}).ToList();
            string error = string.Empty;
            List<string> city = new List<string>() { "город.", "г.", "город", "г" };
            List<string> street = new List<string>() { " улица", "ул", "у", "ул.", "улица.", "у.", "пер", "переулок" };
            List<string> house = new List<string>() { "дом.", "д.", " дом", "д" };
            
            try
            {
                
                for (int i = 0; i < _pAdress.Count; i++)
                {
                    for (int x = 0; x < city.Count; x++)
                    {
                        if (_pAdress[i].StartsWith(city[x]))
                        {
                                test += _pAdress[i].Replace(city[x], "");
                                break;
                        }
                    }

                    for (int x = 0; x < street.Count; x++)
                    {
                        if (_pAdress[i].StartsWith(street[x]))
                        {
                                test += _pAdress[i].Replace(street[x], "");
                                break;
                        }
                    }

                    for (int x = 0; x < house.Count; x++)
                    {
                        if (_pAdress[i].EndsWith(house[x]))
                        {
                                test += _pAdress[i].Replace(house[x], "");
                                break;
                        }
                    }

                }
                MessageBox.Show(test);
            }
            catch
            {
                MessageBox.Show("Ошибка: "+error);
            }
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
    }
}
