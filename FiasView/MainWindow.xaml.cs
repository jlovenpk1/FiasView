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
        //private const string _street = "Улица";
        //private const string _checkStreet = "Проверьте адрес!";
        //private const string _FiasColumn = "Фиас индетификатор";
        //private const string _checkFiasColumn = "Фиас Код не обнаружен!";
        //public bool _DGWithFiasCode = false;
        //private Dictionary<int,house30> _cacheAdrr;
        //LoadExcelToGrid _excelWork;
        //OpenFileDialog _fileOpen;
        //XLWorkbook _workbook;
        //DataTable _data;
        //Model1 db;
        //DBFtoSQL dts;
        //progressBar _progress;
        //ViewModel vm;
        //DataTable _oldData;
        //string _path;
        //public string _firstColumn = "";
        //public string _secondColumn = "";

        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void ChangeColor(object sender, MouseEventArgs e)
        {
            Canvas _canvas = sender as Canvas;
            _canvas.Background =  (SolidColorBrush) new BrushConverter().ConvertFrom("#FF385D89");
           
        }

        private void ResetColor(object sender, MouseEventArgs e)
        {
            Canvas _canvas = sender as Canvas;
            _canvas.Background =  (SolidColorBrush) new BrushConverter().ConvertFrom("#FF3D689B");
        }

        private void Drag_Windows(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            
        }
        public DataTable MyTable { get; set; }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MyTable = new DataTable();
            this.MyTable.Columns.Add("Test");
            var row1 = this.MyTable.NewRow();
            row1["Test"] = "dsjfks";
            //ViewModel vm = new ViewModel();
            //vm.changeLabel();
            this.MyTable.Rows.Add(row1);
            this.DataContext = this;
        }

        //private void DictAutoswitch_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void CheckUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    db = new Model1();

        //        if (db.Database.Exists() == false)
        //        {
        //            if(MessageBox.Show("База отсутствует, создать новую?", "База данных не обнаруженна",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //            {
        //                MessageBox.Show("Создано");
        //                db.Database.Create();
        //                dts = new DBFtoSQL();
        //                dts.GetSQLData();
        //            } 
        //        }

        //        if (db.Database.Exists() == true)
        //        {
        //            if(MessageBox.Show("База данных существует, обновить?","База данных обнаруженна",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //            {
        //                MessageBox.Show("Обновляемся");
        //                dts = new DBFtoSQL();
        //            db.Database.Delete();
        //            db.Database.Create();
        //            dts.GetSQLData();
        //            }
        //        }
        //}

        //async void _start_Click(object sender, RoutedEventArgs e)
        //{

        //    _data.Dispose();
        //    _data = new DataTable();
        //    this.Hide();
        //    await Task.Run(new Action(() =>
        //    {
        //        _progress.Dispatcher.BeginInvoke(new Action(delegate
        //        {
        //            _progress.Show();
        //            _progress._progbar.IsIndeterminate = false;
        //        }));
        //        _data = _excelWork.GetFiasCode(vm);
        //    }));
        //    this.Show();
        //    _progress.Hide();
        //    _DGWithFiasCode = true;
        //    _oldData = _data;
        //    var wb = new XLWorkbook();
        //    wb.Worksheets.Add(_data);
        //    string _name = "Результат проверки.xlsx";
        //    wb.SaveAs(_name);
        //    _dataGrid.ItemsSource = _data.DefaultView;
        //    _dataGrid.Items.Refresh();
    }

}
