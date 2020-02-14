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

namespace FiasView.Operation.WorkWithExcel
{
    class WorkWithAllAddress
    {
        private const string _checkFiasColumn = "Фиас Код не обнаружен!"; // для проверки 
        private const string _editAdress = "Проверьте адрес!"; // для оповещения
        private const string _city30 = "Астрахань"; // для проверки
        private DataTable _data; // хранится таблица 
        private Model1 _db; // База
        private ViewModel vm;
        private MainWindow mv; // главное окно 
        private Dictionary<KeyAddrob, addrob30> _cacheAdrr; // кэш Адресов
        private Dictionary<KeyHouse, house30> _cacheHouse; // кэш домов
        private Dictionary<int, KeyAddrob> _keyDict; // Словарь для адресов
        private KeyAddrob keyA; // Ключи адресов
        public List<house30> _house30; // требуются для формирования кэш-таблицы
        public List<addrob30> _addr; // требуется для формирования кэш-таблицы
        private string _firstColumn = string.Empty; // первая колонка
        private string _secondColumn = string.Empty; // вторая колонка
        private string[] _adress; // адреса 
        private string _postcode = string.Empty; 
        private string _country = string.Empty;
        private string _state = string.Empty;
        private string _city = string.Empty;
        private string _street = string.Empty;
        private string _house = string.Empty;
        private string _fiasCode = string.Empty;
        private string _corpus = string.Empty;

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
            _data.Columns.Add(Columns._city);
            _data.Columns.Add(Columns._street);
            _data.Columns.Add(Columns._house);
            _data.Columns.Add(Columns._corpus);
            _data.Columns.Add(Columns._fiasCode);
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                _adress = _data.Rows[i][_firstColumn].ToString().Split(new char[] { ',' });
                ParsingAdress(_data.Rows[i][_firstColumn].ToString());
                _data.Rows[i][Columns._city] = _city;
                _data.Rows[i][Columns._street] = _street;
                _data.Rows[i][Columns._house] = _house;
                _data.Rows[i][Columns._corpus] = _corpus;
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
            _data.Columns.Add(Columns._city);
            _data.Columns.Add(Columns._street);
            _data.Columns.Add(Columns._house);
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                _adress = _data.Rows[i][_firstColumn].ToString().Split(new char[] { ',' });
                ParsingAdress(_data.Rows[i][_firstColumn].ToString());
                _data.Rows[i][Columns._city] = _city;
                _data.Rows[i][Columns._street] = _street;
                _data.Rows[i][Columns._house] = _house;
            }
            return _data;
        }
        /// <summary>
        /// Парсинг адреса на: Адрес, Улица, Дом. Шаблон: город Астрахань, улица Джедая, дом 46(все А,Б,С и прочие корпусы, пишутся вот так: 46а)
        /// </summary>
        /// <param name="adress">Адрес шаблон: город Астрахань, улица Джедая, дом 46а</param>
        private void ParsingAdress(string adress)
        {
            adress = adress.Replace("№", "");
            List<string> _pAdress = adress.Split(new char[] { ',', }).ToList();
            string error = string.Empty;
            List<string> city = new List<string>() {
                "город.", "г.", "город", "г",
                "город. ", "г. ", "город ", "г ",
                " город.", " г.", " город", " г" };
            List<string> street = new List<string>() {
                " улица", " ул", " ул.", " улица.", " у.", " пер", " пер.", " переулок", " проспект"," пр-кт"," проспект."," п-к"," площадь"," пл"," пл."," проезд"," п-д",
                "улица ", "ул ", "у ", "ул. ", "улица. ", " у. ", "пер ", "пер. ", "переулок ","проспект ","пр-кт ","проспект. ","п-к ","площадь ","пл ","пл. ","проезд ","п-д ",
                "улица", "ул", "у", " ул. ", "улица.", "у.", "пер", "переулок", "пер.","проспект","пр-кт","проспект.","п-к","площадь","пл","пл.","проезд","п-д","снт."," снт","снт. ", "снт ","снт, "," снт,"};
            List<string> house = new List<string>() {
                "дом.", "д.", "дом", "д",
                " дом.", "  д."," д.", " дом", " д",
                "дом. ", "д. ", "дом ","  д.", "д ","  д.", "участок", " участок", "участок ", " участок ", "  участок "};
            List<string> corpus = new List<string>() { " - корп. ", "- корп.", "-корп.", " корп.,", "корп., ", "корп, ", " корп,", " корп. ", " корп.", "корпус "," корпус"," корпус "};
            #region Магия Индии и Китая в одном флаконе
            try
            {
                _city = string.Empty;
                _corpus = string.Empty;
                _house = string.Empty;
                _street = _editAdress;
                for (int i = 0; i < _pAdress.Count; i++)
                {

                    for (int x = 0; x < city.Count; x++)
                    {
                        if (_city != string.Empty) { break; }
                        if (_pAdress[i].StartsWith(city[x]))
                        {
                            //test += _pAdress[i].Replace(city[x], "");
                            _city = _pAdress[i].Replace(city[x], "");
                            var posStart = _city.IndexOf(" ");
                            var posLast = _city.LastIndexOf(" ");
                            break;
                        }
                        else if (_pAdress[i].EndsWith(city[x]))
                        {
                            //test += _pAdress[i].Replace(city[x], "");
                            _city = _pAdress[i].Replace(city[x], "");
                            break;
                        }
                    }

                    for (int x = 0; x < street.Count; x++)
                    {
                        if (_pAdress[i].StartsWith(street[x]) || _pAdress[i].IndexOf("/") > 9)
                        {
                            //test += _pAdress[i].Replace(street[x], "");
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
                            //test += _pAdress[i].Replace(street[x], "");
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
                    }

                    for (int x = 0; x < house.Count; x++)
                    {
                        if (_house != string.Empty) { break; }
                        if (_pAdress[i].EndsWith(house[x]))
                        {
                            //test += _pAdress[i].Replace(house[x], "");
                            _house = _pAdress[i].Replace(house[x], "");
                            var posStart = _house.IndexOf(" ");
                            _house = posStart < 0 ? _house : _house.Remove(posStart, 1);
                            var posLast = _house.LastIndexOf(" ");
                            _house = posLast < 0 ? _house.ToLower() : _house.Remove(posLast, 1).ToLower();
                            break;
                        }
                        else if (_pAdress[i].StartsWith(house[x]))
                        {
                            //test += _pAdress[i].Replace(house[x], "");
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
                    }

                    for (int x = 0; x < corpus.Count; x++)
                    {
                        if (_corpus != string.Empty) { break; }
                        _corpus = string.Empty;
                        if (_pAdress[i].EndsWith(corpus[x]))
                        {
                            //test += _pAdress[i].Replace(house[x], "");
                            _corpus = _pAdress[i].Replace(corpus[x], "");
                            var posStart = _corpus.IndexOf(" ");
                            _corpus = posStart < 0 ? _corpus : _corpus.Remove(posStart, 1);
                            var posLast = _corpus.LastIndexOf(" ");
                            _corpus = posLast < 0 ? _corpus : _corpus.Remove(posLast, 1);
                            break;
                        }
                        else if (_pAdress[i].StartsWith(corpus[x]))
                        {
                            //test += _pAdress[i].Replace(house[x], "");
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
                    }
                }
                //MessageBox.Show(test);
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
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                int x = 0;
                _street = _data.Rows[i][Columns._street].ToString();
                _house = _data.Rows[i][Columns._house].ToString();
                _corpus = _data.Rows[i][Columns._corpus].ToString();
                if (_street == _editAdress || _street == string.Empty && _house == string.Empty || _street != null && _house == string.Empty)
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
                    if (_fiasCode != _checkFiasColumn || _fiasCode != string.Empty && _house != string.Empty)
                    {
                        x++;
                        CountRows = "Поподания: " + x.ToString() + " из " + _data.Rows.Count.ToString();
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
                if (_data.Rows[i][Columns._street] == _editAdress || _data.Rows[i][Columns._street] == string.Empty || _data.Rows[i][Columns._house] == string.Empty)
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
                var key = new KeyAddrob(x.AOID, x.AOGUID, x.OFFNAME.ToLower());
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
                var key = h.HOUSENUM == string.Empty ? new KeyHouse(h.AOGUID, "Пустота" + _index.ToString(), h.BUILDNUM) : new KeyHouse(h.AOGUID, h.HOUSENUM, h.BUILDNUM);
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
        private string ParseFiasCodeString(Dictionary<KeyAddrob, addrob30> query, Dictionary<KeyHouse, house30> query2)
        {
            List<addrob30> _newAdrrob = new List<addrob30>();
            _fiasCode = string.Empty;
            _street = _street.ToLower();
            string _aoid = string.Empty;
            string _aoguid = string.Empty;
            keyA = new KeyAddrob("", "", _street);
            _keyDict = new Dictionary<int, KeyAddrob>();
            var _addr1 = query.Keys.Where(k => k._offname == _street);
            foreach (KeyAddrob _key in _addr1)
            {
                var _checkStatus = query[_key].ACTSTATUS;
                if (_checkStatus == 1)
                {
                    _newAdrrob.Add(query[_key]);
                }
                keyA.Clear();
            }
            int i = 0;
            foreach (var c in _newAdrrob)
            {
                var _parentGuid = c.PARENTGUID;
                var _checkStreet = query.Keys.Where(k => k._aoguid == _parentGuid);
                if (keyA._offname == string.Empty && keyA._aoid == string.Empty && keyA._aoguid == string.Empty)
                {
                    foreach (var p in _checkStreet) // what is this huynya
                    {
                        var _checkAOLvLandCity = query[p];
                        if (_checkAOLvLandCity.AOLEVEL == 4 && _checkAOLvLandCity.OFFNAME == _city30)
                        {
                            _keyDict.Add(i,new KeyAddrob(c.AOID,c.AOGUID, _street));
                            i++;
                        }
                    }
                } else { break; }
            }
            /*Проблема в том, что бывают ситуации когда AOLEVEL = 4 и так же OFFNAME == Астрахань, но разница между двумя улицами в том, что на одной есть дом 9, а у другого нет. 
             Беда ли эта самой ФИАС или мое распиздяйство, я не знаю, но по всей видимости нужно из KeyA сделать массив с поподаниями и потом уже искать хату. 
             */
             foreach(var key in _keyDict)
            {
                if (query.ContainsKey(key.Value))
                {
                    var _getStreet = query[key.Value];
                    _aoguid = _getStreet.AOGUID;
                    var _key = new KeyHouse(_aoguid, _house, _corpus);
                    if (query2.ContainsKey(_key))
                    {
                        var ss = query2[_key];
                        _fiasCode = query2[_key].HOUSEGUID;
                    }
                    else { _fiasCode = _checkFiasColumn; }
                }
                else { _fiasCode = _checkFiasColumn; }
            }
            
            return _fiasCode;
            }
        /// <summary>
        /// Поиск FIAS кода по одному адресу
        /// </summary>
        /// <param name="query">кэш улицы</param>
        /// <param name="query2">кэш дома</param>
        /// <returns></returns>
        private string ParseFiasCodeString(Dictionary<KeyAddrob, addrob30> query, Dictionary<KeyHouse, house30> query2, string _address)
        {
            List<addrob30> _newAdrrob = new List<addrob30>();
            _fiasCode = string.Empty;
            _street = _street.ToLower();
            string _aoid = string.Empty;
            string _aoguid = string.Empty;
            keyA = new KeyAddrob("", "", _street);
            _keyDict = new Dictionary<int, KeyAddrob>();
            var _addr1 = query.Keys.Where(k => k._offname == _street);
            foreach (KeyAddrob _key in _addr1)
            {
                var _checkStatus = query[_key].ACTSTATUS;
                if (_checkStatus == 1)
                {
                    _newAdrrob.Add(query[_key]);
                }
                keyA.Clear();
            }
            int i = 0;
            foreach (var c in _newAdrrob)
            {
                var _parentGuid = c.PARENTGUID;
                var _checkStreet = query.Keys.Where(k => k._aoguid == _parentGuid);
                if (keyA._offname == string.Empty && keyA._aoid == string.Empty && keyA._aoguid == string.Empty)
                {
                    foreach (var p in _checkStreet) // what is this huynya
                    {
                        var _checkAOLvLandCity = query[p];
                        if (_checkAOLvLandCity.AOLEVEL == 4 && _checkAOLvLandCity.OFFNAME == _city30)
                        {
                            _keyDict.Add(i,new KeyAddrob(c.AOID,c.AOGUID, _street));
                            i++;
                        }
                    }
                } else { break; }
            }
            /*Проблема в том, что бывают ситуации когда AOLEVEL = 4 и так же OFFNAME == Астрахань, но разница между двумя улицами в том, что на одной есть дом 9, а у другого нет. 
             Беда ли эта самой ФИАС или мое распиздяйство, я не знаю, но по всей видимости нужно из KeyA сделать массив с поподаниями и потом уже искать хату. 
             */
             foreach(var key in _keyDict)
            {
                if (query.ContainsKey(key.Value))
                {
                    var _getStreet = query[key.Value];
                    _aoguid = _getStreet.AOGUID;
                    var _key = new KeyHouse(_aoguid, _house, _corpus);
                    if (query2.ContainsKey(_key))
                    {
                        var ss = query2[_key];
                        _fiasCode = query2[_key].HOUSEGUID;
                    }
                    else { _fiasCode = _checkFiasColumn; }
                }
                else { _fiasCode = _checkFiasColumn; }
            }
            
            return _fiasCode;
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

        /// <summary>
        /// При создания нового экземпляра передаем обязательно: AOGUID, HOUSENUM, CORPUS
        /// </summary>
        /// <param name="AOGUID">AOGUID Смотреть в таблице HOUSE(HOUSE30 если Астрахань) </param>
        /// <param name="HouseNUM">HOUSENUM Смотреть в таблице HOUSE(HOHUSE30 если Астрахань)</param>
        /// <param name="Corpus">CORPUS Смотреть в таблице HOUSE(HOHUSE30 если Астрахань)</param>
        public KeyHouse(string AOGUID, string HouseNUM, string Corpus)
        {
            this._aoguid = AOGUID;
            this._houseNum = HouseNUM;
            this._corpus = Corpus;
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
                result = typedObj._aoguid.Equals(_aoguid) && typedObj._houseNum.Equals(_houseNum) && typedObj._corpus.Equals(_corpus);
            }

            return result;
        }

        public override int GetHashCode()
        {
            this._corpus = this._corpus == null ? "" : this._corpus;
            this._houseNum = this._houseNum == null ? "" : this._houseNum;
            var result = _aoguid.GetHashCode() ^ _houseNum.GetHashCode() ^ _corpus.GetHashCode();

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

        /// <summary>
        /// При создание нового экземпляра обязательно передавать: AOID, AOGUID, OFFNAME - TABLE ADDROB
        /// </summary>
        /// <param name="aoid">AOID TABLE ADDROB</param>
        /// <param name="aoguid">AOGUID TABLE ADDROB</param>
        /// <param name="offName">OFFNAME TABLE ADDROB</param>
        public KeyAddrob(string aoid, string aoguid, string offName )
        {
            this._aoid = aoid;
            this._aoguid = aoguid;
            this._offname = offName;
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
                result = typedObj._aoguid.Equals(_aoguid) && typedObj._aoid.Equals(_aoid) && typedObj._offname.Equals(_offname);
            }

            return result;
        }

        /// <summary>
        /// Получить HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var result = _aoguid.GetHashCode() ^ _aoid.GetHashCode() ^ _offname.GetHashCode();

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

