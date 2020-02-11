using ClosedXML.Excel;
using FiasView.Forms;
using FiasView.MVVM;
using FiasView.Operation.OperationWithDBF;
using FiasView.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiasView.Operation.WorkWithExcel
{
    class SelectFile
    {
        private const string _street = "Улица";
        private const string _checkStreet = "Проверьте адрес!";
        private const string _FiasColumn = "Фиас индетификатор";
        private const string _checkFiasColumn = "Фиас Код не обнаружен!";
        public bool _DGWithFiasCode = false;
        private Dictionary<int, house30> _cacheAdrr;
        LoadExcelToGrid _excelWork;
        OpenFileDialog _fileOpen;
        XLWorkbook _workbook;
        DataTable _data;
        SelectColumns _selectColumn;
        Model1 db;
        DBFtoSQL dts;
        progressBar _progress;
        ViewModel vm;
        DataTable _oldData;
        string _path;
        public string _firstColumn = "";
        public string _secondColumn = "";

        /// <summary>
        /// Загружаем файл 
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        async public Task<DataTable> FileLoad()
        {
            _fileOpen = new OpenFileDialog
            {
                Filter = "Excel File 2003 (*.xls,*.xlsx)|*.txt;*.xlsx|All files (*.*)|*.*"
                //any settings
            };
            if (_fileOpen.ShowDialog() == true) { _path = _fileOpen.FileName; }
            _excelWork = new LoadExcelToGrid();

            _selectColumn = new SelectColumns();
            _progress = new progressBar();
            _data = new DataTable();
            _selectColumn.ShowDialog();
            _selectColumn_Closed();
            if (_firstColumn != "")
            {
                _fileOpen.Reset();
                _excelWork = new LoadExcelToGrid();
                _data.Clear();
                await Task.Run(new Action(() =>
                {
                    _progress.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        _progress.Show();
                        _progress._progbar.IsIndeterminate = true;
                    }));
                    _data = _excelWork.OpenExcel(_workbook = new XLWorkbook(_path), _firstColumn, _secondColumn);
                }));
                _oldData = _data;
                //_dataGrid.ItemsSource = _data.DefaultView;
                _progress.Hide();
            }

            return _data;
            //MySql.Data.MySqlClient.MySqlBulkLoader ld = new MySql.Data.MySqlClient.MySqlBulkLoader();
        }

        /// <summary>
        /// получаем названия нужных колонок
        /// </summary>
        private void _selectColumn_Closed()
        {
            _firstColumn = _selectColumn._adresColumn.Text;
            _secondColumn = _selectColumn._fiadColumn != null ? _secondColumn = _selectColumn._fiadColumn.Text : "";
        }
    }
}
