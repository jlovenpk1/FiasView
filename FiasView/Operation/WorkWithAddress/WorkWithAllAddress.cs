using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiasView.UI;
using System.Data;
using MySql.Data.EntityFramework;
using System.Windows;
using FiasView.MVVM;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FiasView.Operation.WorkWithExcel;
using System.Windows.Documents;
using System.Globalization;

namespace FiasView.Operation.WorkWithExcel
{
    class WorkWithAllAddress
    {
        #region ПЕРЕМЕННЫЕ 
        private const string _checkFiasColumn = "Фиас Код не обнаружен!"; // для проверки 
        private const string _editAdress = "Проверьте адрес!"; // для оповещения
        private const string _city30 = "Астрахань"; // для проверки
        private DataTable _data; // хранится таблица 
        private Model1 _db; // База
        private Dictionary<KeyAddrob, addrob30> _cacheAdrr; // кэш Адресов
        private Dictionary<KeyHouse, house30> _cacheHouse; // кэш домов
        private Dictionary<int, KeyAddrob> _keyDict; // Словарь для адресов
        private KeyAddrob keyA; // Ключи адресов
        public List<house30> _house30; // требуются для формирования кэш-таблицы
        public List<addrob30> _addr; // требуется для формирования кэш-таблицы
        private string _firstColumn = string.Empty; // первая колонка
        private string _secondColumn = string.Empty; // вторая колонка
        private string[] _adress; // адреса 
        private string _area = string.Empty;
        private string _district = string.Empty;
        private string _village = string.Empty;
        private string _snt = string.Empty;
        private string _city = string.Empty;
        private string _street = string.Empty;
        private string _house = string.Empty;
        private string _fiasCode = string.Empty;
        private string _corpus = string.Empty;
        private static readonly List<string> lStreet = new List<string>() { "ул", "пер", "проезд", "пл", "пр-д" };
        private static readonly List<string> lSnt = new List<string> { "тер. СНТ", "снт", "снт." };
        private static readonly string areaCheck = "обл";
    #endregion

        #region Параметры и реализация события, для передачи данных в ViewModel
        private int _progBarValue;
        private int _progBarMaxValue;
        private string _progBarTextDB;
        private string _progBarLoadCount;
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
                OnParametrChange("ProgBarValue");
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
                OnParametrChange("ProgBarMaxValue");
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
                OnParametrChange("ProgBarTextDB");
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
                OnParametrChange("ProgBarLoadCount");
            }
        }
        /// <summary>
        /// Отображает количество строк, которым было присвоен FIAS CODE
        /// </summary>
        public string CountRows
        {
            get { return _countRows; }
            set
            {
                _countRows = value;
                OnParametrChange("CountRows");
            }
        }
        /// <summary>
        /// Событие для передачи информации по изменению переменной. Способ подписывания на событие: Model.ParametrChange += OnPropertyChanged;
        /// </summary>
        public event Action<string> ParametrChange; 
        /// <summary>
        /// Вызываемый метод для события
        /// </summary>
        /// <param name="prop">имя перменной</param>
        private void OnParametrChange(string param = "")
        {
            ParametrChange?.Invoke(param);
        }
        #endregion

        /// <summary>
        /// Название колонок для формирования DataGridView
        /// </summary>
        private struct Columns
        {
            public static string _area = "Область";
            public static string _district = "Район";
            public static string _village = "Поселок Село";
            public static string _snt = "СНТ СДТ";
            public static string _city = "Город";
            public static string _street = "Улица";
            public static string _house = "Дом";
            public static string _corpus = "Корпус";
            public static string _fiasCode = "Фиас индетификатор";
        } 
        public DataTable OpenExcel(XLWorkbook workbook, string _firstColumn, string _secondColumn)
        {
            this._firstColumn = _firstColumn;
            this._secondColumn = _secondColumn;
            var ws = workbook.Worksheet(1);
            _data = _firstColumn != "" && _secondColumn == "" ? AdresColumnOnly(ws) : AdresColumnWithFiasID(ws);
            return _data;
        }
        /// <summary>
        /// Вернуть таблицу только с адресами
        /// </summary>
        /// <param name="ws">страница эксела(прим. workbook.Worksheet(1) -> откроет первую страницу</param>
        /// <returns></returns>
        private DataTable AdresColumnOnly(IXLWorksheet ws)
        {
            // 414040, Астраханская обл, Астрахань г, Анри Барбюса ул, дом № 30, помещение 019
            _data = ws.RangeUsed().AsTable().AsNativeDataTable();
            for (int i = 0; i < _data.Columns.Count; i++)
            {
                if (_data.Columns[i].ToString() != _firstColumn)
                {
                    _data.Columns.Remove(_data.Columns[i].ToString());
                    --i;
                }
            }
            _data.Columns.Add(Columns._area);
            _data.Columns.Add(Columns._district);
            _data.Columns.Add(Columns._village);
            _data.Columns.Add(Columns._city);
            _data.Columns.Add(Columns._snt);
            _data.Columns.Add(Columns._street);
            _data.Columns.Add(Columns._house);
            _data.Columns.Add(Columns._corpus);
            _data.Columns.Add(Columns._fiasCode);
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                _adress = _data.Rows[i][_firstColumn].ToString().Split(new char[] { ',' });
                ParsingAdress(_data.Rows[i][_firstColumn].ToString());
                _data.Rows[i][Columns._area] = _area.TrimStart();
                _data.Rows[i][Columns._district] = _district.TrimStart();
                _data.Rows[i][Columns._village] = _village.TrimStart();
                _data.Rows[i][Columns._city] = _city.TrimStart();
                _data.Rows[i][Columns._snt] = _snt.TrimStart();
                _data.Rows[i][Columns._street] = _street.TrimStart();
                _data.Rows[i][Columns._house] = _house.TrimStart();
                _data.Rows[i][Columns._corpus] = _corpus.TrimStart();
            }
            return _data;
        }
        /// <summary>
        /// Вернуть таблицу с адресами + FIAS ID
        /// </summary>
        /// <param name="ws">страница эксела(прим. workbook.Worksheet(1) -> откроет первую страницу</param>
        /// <returns></returns>
        private DataTable AdresColumnWithFiasID(IXLWorksheet ws)
        {
            _data = ws.RangeUsed().AsTable().AsNativeDataTable();
            for (int i = 0; i < _data.Columns.Count; i++)
            {
                if (_data.Columns[i].ToString() != _firstColumn && _data.Columns[i].ToString() != _secondColumn)
                {
                    _data.Columns.Remove(_data.Columns[i].ToString());
                    --i;
                }
            }
            _data.Columns.Add(Columns._area);
            _data.Columns.Add(Columns._district);
            _data.Columns.Add(Columns._village);
            _data.Columns.Add(Columns._city);
            _data.Columns.Add(Columns._snt);
            _data.Columns.Add(Columns._street);
            _data.Columns.Add(Columns._house);
            _data.Columns.Add(Columns._corpus);
            _data.Columns.Add(Columns._fiasCode);
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                _adress = _data.Rows[i][_firstColumn].ToString().Split(new char[] { ',' });
                ParsingAdress(_data.Rows[i][_firstColumn].ToString());
                _data.Rows[i][Columns._area] = _area;
                _data.Rows[i][Columns._district] = _district;
                _data.Rows[i][Columns._village] = _village;
                _data.Rows[i][Columns._city] = _city;
                _data.Rows[i][Columns._snt] = "ЖОПА";
                _data.Rows[i][Columns._street] = _street;
                _data.Rows[i][Columns._house] = _house;
                _data.Rows[i][Columns._corpus] = _corpus;
            }
            return _data;
        }
        /// <summary>
        /// Парсинг адреса на: Адрес, Улица, Дом. Шаблон: город Астрахань, улица Джедая, дом 46(все А,Б,С и прочие корпусы, пишутся вот так: 46а)
        /// </summary>
        /// <param name="adress">Адрес шаблон: город Астрахань, улица Джедая, дом 46а</param>
        public void ParsingAdress(string adress)
        {
            /*
             Проблема данного метода в том, что слишком много итераций, нужно найти менее время затратный способ. 
             Увы, пока что лучший метод это циклы. 
             */
            adress = adress.Replace("№", "");
            List<string> _pAdress = adress.Split(new char[] { ',', }).ToList();
            string error = string.Empty;
            List<string> area = new List<string>() { " область,", " область.,", " обл.,", " обл,", " обл,", "область ", " область", " область,", " обл", "обл ", " обл"
            };
            List<string> district = new List<string>() { " р-н.", " р-н", " район.", " район", " р-н", "р-н ", " район", "район ", " р-н", "р-н ", " р-н"
            };
            List<string> village = new List<string>() { " п.", " п", " поселок.", " поселок", " поселок", "поселок ", " с.", " с", " село.", " село", " село", "село "," с","с ","п "," п", " с"
            };
            List<string> snt = new List<string>() { " СНТ,", " СНТ.,", " СДТ.,", " СДТ,", " СНТ", "СНТ ", " СДТ", "СДТ ", "снт ", " снт", "сдт ", " сдт"
            };
            List<string> city = new List<string>() {
                "город.", "г.", "город", "г",
                "город. ", "г. ", "город ", "г ",
                " город.", " г.", " город", " г" };
            List<string> street = new List<string>() {
                " улица", " ул.", " ул", " улица.", " у.", " пер", " пер.", " переулок", " проспект"," пр-кт"," проспект."," п-к"," площадь"," пл"," пл."," проезд"," п-д",
                "улица ", "ул ", "у ", "ул. ", "улица. ", " у. ", "пер ", "пер. ", "переулок ","проспект ","пр-кт ","проспект. ","п-к ","площадь ","пл ","пл. ","проезд ","п-д ",
                "улица", "ул", "у", " ул. ", "улица.", "у.", "пер", "переулок", "пер.","проспект","пр-кт","проспект.","п-к","площадь","пл","пл.","проезд","п-д" };
            List<string> house = new List<string>() {
                "дом.", "д.", "дом", "д",
                " дом.", "  д."," д.", " дом", " д",
                "дом. ", "д. ", "дом ","  д.", "д ","  д.", "участок", " участок", "участок ", " участок ", "  участок "};
            List<string> corpus = new List<string>() { " - корп. ", "- корп.", "-корп.", " корп.,", "корп., ", "корп, ", " корп,", " корп. ", " корп.", "корпус ", " корпус", " корпус " 
            };
            #region Магия Индии и Китая в одном флаконе
            try
            {
                _area = string.Empty;
                _district = string.Empty;
                _village = string.Empty;
                _snt = string.Empty;
                _city = string.Empty;
                _corpus = string.Empty;
                _house = string.Empty;
                _street = string.Empty;
                for (int i = 0; i < _pAdress.Count; i++)
                {
                    for (int x = 0; x < area.Count; x++)
                    {
                        if (_area != string.Empty) { break; }
                        if (_pAdress[i].StartsWith(area[x]))
                        {
                            _area = _pAdress[i].Replace(area[x], "");
                            var posStart = _area.IndexOf(" ");
                            var posLast = _area.LastIndexOf(" ");
                            break;
                        }
                        else if (_pAdress[i].EndsWith(area[x]))
                        {
                            _area = _pAdress[i].Replace(area[x], "");
                            break;
                        }
                    } // поиск Области

                    for (int x = 0; x < district.Count; x++)
                    {
                        if (_district != string.Empty) { break; }
                        if (_pAdress[i].StartsWith(district[x]))
                        {
                            //test += _pAdress[i].Replace(city[x], "");
                            _district = _pAdress[i].Replace(district[x], "");
                            var posStart = _district.IndexOf(" ");
                            var posLast = _district.LastIndexOf(" ");
                            break;
                        }
                        else if (_pAdress[i].EndsWith(district[x]))
                        {
                            //test += _pAdress[i].Replace(city[x], "");
                            _district = _pAdress[i].Replace(district[x], "");
                            break;
                        }
                    } // поиск Района

                    for (int x = 0; x < village.Count; x++)
                    {
                        if (_village != string.Empty) { break; }
                        if (_pAdress[i].StartsWith(village[x]))
                        {
                            //test += _pAdress[i].Replace(city[x], "");
                            _village = _pAdress[i].Replace(village[x], "");
                            var posStart = _village.IndexOf(" ");
                            var posLast = _village.LastIndexOf(" ");
                            break;
                        }
                        else if (_pAdress[i].EndsWith(village[x]))
                        {
                            //test += _pAdress[i].Replace(city[x], "");
                            _village = _pAdress[i].Replace(village[x], "");
                            break;
                        }
                    } // поиск Села/Поселка

                    for (int x = 0; x < city.Count; x++)
                    {
                        if (_city != string.Empty) { break; }
                        if (_pAdress[i].StartsWith(city[x]))
                        {
                            _city = _pAdress[i].Replace(city[x], "");
                            var posStart = _city.IndexOf(" ");
                            var posLast = _city.LastIndexOf(" ");
                            break;
                        }
                        else if (_pAdress[i].EndsWith(city[x]))
                        {
                            _city = _pAdress[i].Replace(city[x], "");
                            break;
                        }
                    } // поиск Города

                    for (int x = 0; x < snt.Count; x++)
                    {
                        if (_snt != string.Empty) { break; }
                        if (_pAdress[i].StartsWith(snt[x]))
                        {
                            _snt = _pAdress[i].Replace(snt[x], "");
                            var posStart = _snt.IndexOf(" ");
                            var posLast = _snt.LastIndexOf(" ");
                            break;
                        }
                        else if (_pAdress[i].EndsWith(snt[x]))
                        {
                            _snt = _pAdress[i].Replace(snt[x], "");
                            break;
                        }
                    } // Поиск СНТ/СДТ
                     
                    for (int x = 0; x < street.Count; x++)
                    {
                        if (_pAdress[i].StartsWith(street[x]) || _pAdress[i].IndexOf("/") > 9)
                        {
                            _street = _pAdress[i].Replace(street[x], "");
                            var posStart = _street.IndexOf(" ");
                            _street = posStart < 0 ? _street : _street.Remove(posStart, 1);
                            var posLast = _street.LastIndexOf(" ");
                            _street = posLast < 0 || posLast < 0 && _pAdress[i].Length > 15 || posLast > 0 && _pAdress[i].Length > 8 ? _street : _street.Remove(posLast, 1);
                            if (_street.IndexOf("/") > 0)
                            {
                                _street = _street.IndexOf("/") > 0 ? _street.Remove(_street.IndexOf("/"), _street.Length - _street.IndexOf("/")) : _street;
                                _street = _street.IndexOf("ул") > 0 ? _street.Remove(_street.IndexOf("ул"), _street.Length - _street.IndexOf("ул")) : _street;
                                _street = _street.IndexOf("ул.") > 0 ? _street.Remove(_street.IndexOf("ул."), _street.Length - _street.IndexOf("ул.")) : _street;
                                _street = _street.IndexOf("пер") > 0 ? _street.Remove(_street.IndexOf("пер"), _street.Length - _street.IndexOf("пер")) : _street;
                                _street = _street.IndexOf("пер.") > 0 ? _street.Remove(_street.IndexOf("пер."), _street.Length - _street.IndexOf("пер.")) : _street;
                            }
                            break;
                        }
                        else if (_pAdress[i].EndsWith(street[x]) || _pAdress[i].IndexOf("/") > 9)
                        {
                            _street = _pAdress[i].Replace(street[x], "");
                            var posStart = _street.IndexOf(" ");
                            _street = posStart < 0 ? _street : _street.Remove(posStart, 1);
                            var posLast = _street.LastIndexOf(" ");
                            _street = posLast < 0 || posLast < 0 && _pAdress[i].Length > 15 || posLast > 0 && _pAdress[i].Length > 8 ? _street : _street.Remove(posLast, 1);
                            if (_street.IndexOf("/") > 0)
                            {
                                _street = _street.IndexOf("/") > 0 ? _street.Remove(_street.IndexOf("/"), _street.Length - _street.IndexOf("/")) : _street;
                                _street = _street.IndexOf("ул") > 0 ? _street.Remove(_street.IndexOf("ул"), _street.Length - _street.IndexOf("ул")) : _street;
                                _street = _street.IndexOf("ул.") > 0 ? _street.Remove(_street.IndexOf("ул."), _street.Length - _street.IndexOf("ул.")) : _street;
                                _street = _street.IndexOf("пер") > 0 ? _street.Remove(_street.IndexOf("пер"), _street.Length - _street.IndexOf("пер")) : _street;
                                _street = _street.IndexOf("пер.") > 0 ? _street.Remove(_street.IndexOf("пер."), _street.Length - _street.IndexOf("пер.")) : _street;
                            }
                            break;
                        }
                    } // Поиск Улицы

                    for (int x = 0; x < house.Count; x++)
                    {
                        if (_house != string.Empty) { break; }
                        if (_pAdress[i].EndsWith(house[x]))
                        {
                            _house = _pAdress[i].Replace(house[x], "");
                            var posStart = _house.IndexOf(" ");
                            _house = posStart < 0 ? _house : _house.Remove(posStart, 1);
                            var posLast = _house.LastIndexOf(" ");
                            _house = posLast < 0 ? _house.ToLower() : _house.Remove(posLast, 1).ToLower();
                            break;
                        }
                        else if (_pAdress[i].StartsWith(house[x]))
                        {
                            _house = _pAdress[i].Replace(house[x], "");
                            if (_house.Length > 10)
                            {
                                var pos = _house.LastIndexOf(' ');
                                _house = _house.Substring(0, pos);
                            }
                            _house = _house.Replace(" ", "");
                            var posStart = _house.IndexOf(" ");
                            _house = posStart < 0 ? _house.ToLower() : _house.Remove(posStart, 1).ToLower();
                            var posLast = _house.LastIndexOf(" ");
                            _house = posLast < 0 ? _house.ToLower() : _house.Remove(posLast, 1).ToLower();
                            break;
                        }
                    } // Поиск Дома

                    for (int x = 0; x < corpus.Count; x++)
                    {
                        if (_corpus != string.Empty) { break; }
                        _corpus = string.Empty;
                        if (_pAdress[i].EndsWith(corpus[x]))
                        {
                            _corpus = _pAdress[i].Replace(corpus[x], "");
                            var posStart = _corpus.IndexOf(" ");
                            _corpus = posStart < 0 ? _corpus : _corpus.Remove(posStart, 1);
                            var posLast = _corpus.LastIndexOf(" ");
                            _corpus = posLast < 0 ? _corpus : _corpus.Remove(posLast, 1);
                            break;
                        }
                        else if (_pAdress[i].StartsWith(corpus[x]))
                        {
                            _corpus = _pAdress[i].Replace(corpus[x], "");
                            if (_corpus.Length > 10)
                            {
                                var pos = _corpus.LastIndexOf(' ');
                                _corpus = _corpus.Substring(0, pos);
                            }
                            _corpus = _corpus.Replace(" ", "");
                            var posStart = _corpus.IndexOf(" ");
                            _corpus = posStart < 0 ? _corpus : _corpus.Remove(posStart, 1);
                            var posLast = _corpus.LastIndexOf(" ");
                            _corpus = posLast < 0 ? _corpus : _corpus.Remove(posLast, 1);
                            break;
                        }
                    } // Поиск Корпуса
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex);
            }
            #endregion
        }

        /// <summary>
        /// Получение Фиас Кода с записью его в DataTable Column = "Фиас Индетификатор"
        /// </summary>
        /// <returns></returns>
        public DataTable GetFiasCode(DataTable _dataViewModel)
        {
            List<string> _progInfo = new List<string>();
            _data = _dataViewModel;
            DeleteEmptyRow();
            int x = 0;
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                _area = (string)_data.Rows[i][Columns._area];
                _area = _area.ToLower().Trim();
                _district = (string)_data.Rows[i][Columns._district];
                _district = _district.ToLower().Trim();
                _city = (string)_data.Rows[i][Columns._city];
                _city = _city.ToLower(culture: CultureInfo.CurrentCulture).Trim();
                _street = (string)_data.Rows[i][Columns._street];
                _street = _street.ToLower().Trim();
                _house = (string)_data.Rows[i][Columns._house];
                _corpus = (string)_data.Rows[i][Columns._corpus];
                _snt = (string)_data.Rows[i][Columns._snt];
                _snt = _snt.ToLower().Trim();
                _village = (string)_data.Rows[i][Columns._village];
                _village = _village.ToLower().Trim();
                if (_street == _editAdress || (string.IsNullOrEmpty(_street) && string.IsNullOrEmpty(_house)) || (_street != null && string.IsNullOrEmpty(_house)))
                {
                    _data.Rows[i][Columns._fiasCode] = _checkFiasColumn;
                }
                else
                {

                    var result = ParseFiasCodeString(_cacheAdrr, _cacheHouse);
                    _data.Rows[i][Columns._fiasCode] = result;
                    ProgBarMaxValue = _data.Rows.Count;
                    ProgBarTextDB = "Фиас код: " + result + "; Улица: " + _street;
                    ProgBarValue = i;
                    ProgBarLoadCount = "Прочитано: " + i + " из " + _data.Rows.Count;
                    if (_fiasCode != _checkFiasColumn)
                    {
                        x++;
                        CountRows = $"Поподания: {x.ToString()} из {_data.Rows.Count.ToString()}";
                    }  
                }
            }
            return _data;
        }

        /// <summary>
        /// Получение Фиас Кода одного адреса
        /// </summary>
        /// <returns></returns>
        public string GetFiasCode(string _address)
        {
            ParsingAdress(_address);
            var result = ParseFiasCodeString(_cacheAdrr, _cacheHouse);
            ProgBarMaxValue = 1;
            ProgBarTextDB = "Фиас код: " + result + "; Улица: " + _street;
            ProgBarValue = 1;
            
            return result;
        }

        /// <summary>
        /// Удаление пустых строк и строк где строка адреса не распарсилась
        /// </summary>
        private void DeleteEmptyRow()
        {
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                if ((string)_data.Rows[i][Columns._street] == string.Empty && (string)_data.Rows[i][Columns._house] == string.Empty && (string)_data.Rows[i][Columns._city] == string.Empty)
                {
                    _data.Rows[i].Delete();
                }
            }
        }
        /// <summary>
        /// Загрузка кэш-данных улицы
        /// </summary>
        /// <param name="vm"></param>
        public void LoadCacheAdrr()
        {
            ProgBarTextDB = "Загружаю Улицы, ожидайте!";
            _db = new Model1();
            _db.Database.CommandTimeout = 300;
            _cacheAdrr = new Dictionary<KeyAddrob, addrob30>();
            _addr = new List<addrob30>();
            _addr = _db.addrob30.ToList();
            int countRow = _addr.Count;
            int _index = 0;
            ProgBarMaxValue = countRow;
            foreach (addrob30 x in _addr)
            {
                _index++;
                var key = x.PARENTGUID == null ? new KeyAddrob(x.AOID, x.AOGUID, x.OFFNAME.ToLower(), "ссылки нет", x.SHORTNAME) : new KeyAddrob(x.AOID, x.AOGUID, x.OFFNAME.ToLower(), x.PARENTGUID, x.SHORTNAME);
                _cacheAdrr.Add(key, x);
                ProgBarTextDB = "Загруженно улиц: " + _index.ToString();
                ProgBarValue = _index;
            }
        }

        /// <summary>
        /// Загрузка кэш-данных номера дома
        /// </summary>
        /// <param name="vm"></param>
        public void LoadCacheHouse()
        {
            ProgBarTextDB = "Загружаю Дома, ожидайте!";
            int _index = 0;
            _cacheHouse = new Dictionary<KeyHouse, house30>();
            _house30 = new List<house30>();
            _house30 = _db.house30.ToList();
            int countRows = _house30.Count;
            ProgBarMaxValue = countRows;
            foreach (house30 h in _house30)
            {
                _index++;
                var key = h.HOUSENUM == string.Empty ? new KeyHouse(h.AOGUID, "Пустота" + _index.ToString(), h.BUILDNUM, h.HOUSEGUID) : new KeyHouse(h.AOGUID, h.HOUSENUM, h.BUILDNUM,h.HOUSEGUID);
                if (_cacheHouse.ContainsKey(key))
                {
                    if (_cacheHouse[key].UPDATEDATE < h.UPDATEDATE)
                    {
                        _cacheHouse.Remove(key);
                        _cacheHouse.Add(key, h);
                    }
                }
                else { _cacheHouse.Add(key, h); }

                ProgBarTextDB = "Загуженно домов: " + _index.ToString();
                ProgBarValue = _index;
            }
        }
        /// <summary>
        /// Поиск FIAS кода массовый
        /// </summary>
        /// <param name="query">кэш улицы</param>
        /// <param name="query2">кэш дома</param>
        /// <returns></returns>
        private string ParseFiasCodeString(Dictionary<KeyAddrob, addrob30> _cacheAddrob, Dictionary<KeyHouse, house30> _cacheHouse)
        {
            // ДОБАВИТЬ МИКРОРАЙОНЫ! ПРОВЕРКУ НА МИКРОРАЙОНЫ!
            if (!string.IsNullOrEmpty(_street) && string.IsNullOrEmpty(_snt) && string.IsNullOrEmpty(_village)) { _fiasCode = IsCity(_cacheAddrob, _cacheHouse); } // есть только улица, город и область
            if (!string.IsNullOrEmpty(_snt) && !string.IsNullOrEmpty(_village) && !string.IsNullOrEmpty(_area)) { _fiasCode = IsVillageSNT(_cacheAddrob, _cacheHouse); } // Есть Село снт участок
            if (!string.IsNullOrEmpty(_street) &&  !string.IsNullOrEmpty(_village)) { _fiasCode = IsVillage(_cacheAddrob, _cacheHouse); } // Есть село и улица
            if (!string.IsNullOrEmpty(_snt) && string.IsNullOrEmpty(_village)) { _fiasCode = IsSNT(_cacheAddrob, _cacheHouse); } // TODO
            if (!string.IsNullOrEmpty(_street) && !string.IsNullOrEmpty(_snt) && string.IsNullOrEmpty(_village)) { _fiasCode = IsSNTWithStreet(_cacheAddrob, _cacheHouse); } // TODO
            return string.IsNullOrEmpty(_fiasCode) ? _checkFiasColumn : _fiasCode;
        }

        /// <summary>
        /// Найти ФИАС код в городе Астрахань
        /// </summary>
        /// <param name="_cacheAddrob"> кэш из базы ADDROB</param>
        /// <param name="_cacheHouse"> кэш из базы HOUSE</param>
        /// <returns></returns>
        private string IsCity(Dictionary<KeyAddrob, addrob30> _cacheAddrob, Dictionary<KeyHouse, house30> _cacheHouse)
        {
            string Astrachan = _cacheAddrob.Keys.Where(x => x._offname == _city).Select(x => x._aoguid).FirstOrDefault();
            var street = _cacheAddrob.Keys.Where(k => k._offname == _street.ToLower() && 
                                                    lStreet.Contains(k._shortName)
                                                    && k._parent == Astrachan).ToDictionary(p => new KeyAddrob(p._aoid, p._aoguid, p._offname, p._parent, p._shortName)) ?? new Dictionary<KeyAddrob, KeyAddrob>();
            string aoguid = street.Count == 0 ? "" : street.FirstOrDefault().Key._aoguid ?? "";
            string fiasCode = _cacheHouse.Keys
                .Where(k => k._aoguid == aoguid && k._houseNum == _house || k._aoguid == aoguid && k._houseNum == _house && k._corpus == _corpus)
                .Select(r => r._houseguid)
                .FirstOrDefault() ?? "";
            return string.IsNullOrEmpty(fiasCode) ? _checkFiasColumn : fiasCode;
        }

        private string IsVillageSNT(Dictionary<KeyAddrob, addrob30> _cacheAddrob, Dictionary<KeyHouse, house30> _cacheHouse)
        {
            /*Связь такая: Ищем дом. Потом по AOGUID(дома) ищем СНТ, потом по AOGUID(снт) ищем село/деревню, потом по AOGUID села/деревни ищем Район, потом по Району область
             после можем выводить FiasCode, если снт нашло свой поселок, поселок свой район, район область. 
              */
            string streets = "";
            string oblast = _cacheAddrob.Keys
                .Where(x => x._offname == _area && x._shortName == areaCheck)
                .Select(x => x._aoguid)
                .FirstOrDefault() == null ? "" : _cacheAddrob.Keys
                .Where(x => x._offname == _area && x._shortName == areaCheck)
                .Select(x => x._aoguid)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(oblast)) { return _checkFiasColumn; }
            string district = _cacheAddrob.Keys
                .Where(x => x._offname == _district && x._parent == oblast)
                .Select(x => x._aoguid)
                .FirstOrDefault() ?? "";
            string village = _cacheAddrob.Keys
                .Where(x => x._offname == _village && x._parent == district)
                .Select(x => x._aoguid)
                .FirstOrDefault() ?? "";
            var snt = _cacheAddrob.Keys
                .Where(x => x._offname == _snt && x._parent == village)
                .ToDictionary(p => new KeyAddrob(p._aoid, p._aoguid, p._offname, p._parent, p._shortName)) ?? new Dictionary<KeyAddrob, KeyAddrob>();
            if (!string.IsNullOrEmpty(_street)) {
                 streets = snt.Count == 0 ? "" : _cacheAddrob.Keys.Where(x => x._offname == _street && x._parent == snt.Keys.FirstOrDefault()._aoguid && lStreet.Contains(x._shortName))
                .Select(x => x._aoguid).FirstOrDefault() ?? "";
            }
            string aoguid = !string.IsNullOrEmpty(streets) || snt.Count == 0 ? streets : snt.FirstOrDefault().Key._aoguid ?? "";
            string fiasCode = _cacheHouse.Keys
                .Where(k => k._aoguid == aoguid && k._houseNum == _house || k._aoguid == aoguid && k._houseNum == _house && k._corpus == _corpus)
                .Select(r => r._houseguid)
                .FirstOrDefault() ?? "";
                return string.IsNullOrEmpty(fiasCode) ? _checkFiasColumn : fiasCode;
        }

        private string IsVillage(Dictionary<KeyAddrob, addrob30> _cacheAddrob, Dictionary<KeyHouse, house30> _cacheHouse)
        {

            string oblast = _cacheAddrob.Keys
                .Where(x => x._offname == _area && x._shortName == areaCheck)
                .Select(x => x._aoguid)
                .FirstOrDefault() == null ? "" : _cacheAddrob.Keys
                .Where(x => x._offname == _area && x._shortName == areaCheck)
                .Select(x => x._aoguid)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(oblast)) { return _checkFiasColumn; }
            string district = _cacheAddrob.Keys
                .Where(x => x._offname == _district && x._parent == oblast)
                .Select(x => x._aoguid)
                .FirstOrDefault() ?? "";
            string village = _cacheAddrob.Keys
                .Where(x => x._offname == _village && x._parent == district)
                .Select(x => x._aoguid)
                .FirstOrDefault() ?? "";
            string aoguid = _cacheAddrob.Keys.Where(x => x._offname == _street && x._parent == village && lStreet.Contains(x._shortName))
               .Select(x => x._aoguid).FirstOrDefault();
            string fiasCode = _cacheHouse.Keys
                .Where(k => k._aoguid == aoguid && k._houseNum == _house || k._aoguid == aoguid && k._houseNum == _house && k._corpus == _corpus)
                .Select(r => r._houseguid)
                .FirstOrDefault() ?? "";
            return string.IsNullOrEmpty(fiasCode) ? _checkFiasColumn : fiasCode;
        }

        private string IsSNT(Dictionary<KeyAddrob, addrob30> _cacheAddrob, Dictionary<KeyHouse, house30> _cacheHouse)
        {
            string streets = string.Empty;
            string Astrachan = _cacheAddrob.Keys
                .Where(x => x._offname == _city)
                .Select(x => x._aoguid)
                .FirstOrDefault();
            var snt = _cacheAddrob.Keys
                .Where(k => k._offname == _snt.ToLower(culture: CultureInfo.CurrentCulture) && lSnt.Contains(k._shortName) && k._parent == Astrachan)
                .ToDictionary(p => new KeyAddrob(p._aoid, p._aoguid, p._offname, p._parent, p._shortName));
            if (!string.IsNullOrEmpty(_street))
            {
                streets = snt.Count == 0 ? "" : _cacheAddrob.Keys.Where(x => x._offname == _street && x._parent == snt.Keys.FirstOrDefault()._aoguid && lStreet.Contains(x._shortName))
               .Select(x => x._aoguid).FirstOrDefault();
            }
            string aoguid = !string.IsNullOrEmpty(streets) || snt.Count == 0 ? streets : snt.FirstOrDefault().Key._aoguid ?? "";
            string fiasCode = _cacheHouse.Keys
                .Where(k => k._aoguid == aoguid && k._houseNum == _house || k._aoguid == aoguid && k._houseNum == _house && k._corpus == _corpus)
                .Select(r => r._houseguid)
                .FirstOrDefault() ?? "";
            return string.IsNullOrEmpty(fiasCode) ? _checkFiasColumn : fiasCode;
        }

        private string IsSNTWithStreet(Dictionary<KeyAddrob, addrob30> _cacheAddrob, Dictionary<KeyHouse, house30> _cacheHouse)
        {

            return _checkFiasColumn;
        }

} 

    /// <summary>
    /// KeyHouse каждый новый экземляр используется для индексации 
    /// </summary>
    class KeyHouse
    {
        /* Информация по переменным.
         _aoguid = Часть ключа, для индексации требуется AOGUID
         _houseNum = Часть ключа, для индексации требуется HOUSENUM
         _corpus = Часть ключа, для индексации требуется CORPUS

            По AOGUID + HOUSENUM + CORPUS - можно вытащить максимально точный ФИАС индетификатор
             */
        public string _aoguid { get; private set; } 
        public string _houseNum { get; private set; }
        public string _corpus { get; private set; }
        public string _houseguid { get; private set; }

        /// <summary>
        /// При создания нового экземпляра передаем обязательно: AOGUID, HOUSENUM, CORPUS, HOUSEGUID
        /// </summary>
        /// <param name="AOGUID">AOGUID Смотреть в таблице HOUSE(HOUSE30 если Астрахань) </param>
        /// <param name="HouseNUM">HOUSENUM Смотреть в таблице HOUSE(HOHUSE30 если Астрахань)</param>
        /// <param name="Corpus">CORPUS Смотреть в таблице HOUSE(HOHUSE30 если Астрахань)</param>
        /// <param name="HouseGUID">HOUSEGUID это и есть нужный FIAS индетификатор</param>
        public KeyHouse(string AOGUID, string HouseNUM, string Corpus, string HouseGUID)
        {
            this._aoguid = AOGUID;
            this._houseNum = HouseNUM;
            this._corpus = Corpus;
            this._houseguid = HouseGUID;
        }

        /// <summary>
        /// Функция проверки типа объекта(является ли он KeyHouse)
        /// </summary>
        /// <param name="obj">объект сравнения</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var typedObj = obj as KeyHouse;

            var result = typedObj != null;

            if (result)
            {
                result = typedObj._aoguid.Equals(_aoguid) && typedObj._houseNum.Equals(_houseNum) && typedObj._corpus.Equals(_corpus) && typedObj._houseguid.Equals(_houseguid);
            }

            return result;
        }

        public override int GetHashCode()
        {
            this._corpus = this._corpus == null ? "" : this._corpus;
            this._houseNum = this._houseNum == null ? "" : this._houseNum;
            var result = _aoguid.GetHashCode() ^ _houseNum.GetHashCode() ^ _corpus.GetHashCode() ^ _houseguid.GetHashCode();

            return result;
        }
    }

    /// <summary>
    /// KeyAddroub каждый новый экземпляр используется для индексации
    /// </summary>
    class KeyAddrob
    {
        /* Информаци переменные
         _aoid = часть ключа, требуется для индексции, AOID 
         _aoguid = часть ключа, требуется для индексации, AOGUID
         _offname = часть ключа, требуется для индексации, OFFNAME

            AOID, AOGUID, OFFNAME = все три параметра играют роль индекса. Таблица ADDROB.
             */
        public string _aoid { get; private set; } // AOID TABLE ADDROB
        public string _aoguid { get; private set; } // AOGUID TABLE ADDROB
        public string _offname { get; private set; } // OFFNAME TABLE ADDROB
        public string _parent { get; private set; } // PARENTGUID TABLE ADDROB
        public string _shortName { get; private set; }// SHORTNAME TABLE ADDROB

        /// <summary>
        /// При создание нового экземпляра обязательно передавать: AOID, AOGUID, OFFNAME - TABLE ADDROB
        /// </summary>
        /// <param name="aoid">AOID TABLE ADDROB</param>
        /// <param name="aoguid">AOGUID TABLE ADDROB</param>
        /// <param name="offName">OFFNAME TABLE ADDROB</param>
        public KeyAddrob(string aoid, string aoguid, string offName, string parentguid, string shortname)
        {
            this._aoid = aoid;
            this._aoguid = aoguid;
            this._offname = offName;
            this._parent = parentguid;
            this._shortName = shortname;
        }

        /// <summary>
        /// Проверка является ли объект KeyADDROB 
        /// </summary>
        /// <param name="obj">объект</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var typedObj = obj as KeyAddrob;

            var result = typedObj != null;

            if (result)
            {
                result = typedObj._aoguid.Equals(_aoguid) && typedObj._aoid.Equals(_aoid) && typedObj._offname.Equals(_offname)
                                && typedObj.Equals(_parent) && typedObj.Equals(_shortName);
            }

            return result;
        }

        /// <summary>
        /// Получить HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var result = _aoguid.GetHashCode() ^ _aoid.GetHashCode() ^ _offname.GetHashCode() ^ _parent.GetHashCode() ^ _shortName.GetHashCode();

            return result;
        }

        /// <summary>
        /// Очистить
        /// </summary>
        public void Clear()
        {
            this._aoguid = string.Empty;
            this._aoid = string.Empty;
            this._offname = string.Empty;
        }

    }
}


