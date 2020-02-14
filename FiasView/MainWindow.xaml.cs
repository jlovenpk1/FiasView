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
    }

}
