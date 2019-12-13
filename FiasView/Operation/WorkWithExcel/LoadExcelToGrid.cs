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

namespace FiasView.Operation.WorkWithExcel
{
    class LoadExcelToGrid
    {
        private const string _checkFiasColumn = "Фиас Код не обнаружен!";
        private const string _editAdress = "Проверьте адрес!";
        private DataTable _data;
        private DataTable _newData;
        private Model1 _db;
        private MainWindow mv;
        Dictionary<KeyAddrob, addrob30> _cacheAdrr;
        Dictionary<KeyHouse, house30> _cacheHouse;
        private string _firstColumn = string.Empty;
        private string _secondColumn = string.Empty;
        private string[] _adress;
        private string _postcode = string.Empty;
        private string _country = string.Empty;
        private string _state = string.Empty;
        private string _city = string.Empty;
        private string _street = string.Empty;
        private string _house = string.Empty;
        private string _fiasCode = string.Empty;
        private string _corpus = string.Empty;

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
            string test = string.Empty;
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
                "улица", "ул", "у", "ул.", "улица.", "у.", "пер", "переулок", "пер.","проспект","пр-кт","проспект.","п-к","площадь","пл","пл.","проезд","п-д","снт."," снт","снт. ", "снт ","снт, "," снт,"};
            List<string> house = new List<string>() {
                "дом.", "д.", "дом", "д",
                " дом.", "  д."," д.", " дом", " д",
                "дом. ", "д. ", "дом ","  д.", "д ","  д.", "участок", " участок", "участок ", " участок ", "  участок "};
            List<string> corpus = new List<string>() { " - корп. ", "- корп.", "-корп.", " корп.,", "корп., ", "корп, ", " корп,", " корп. ", " корп." };
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
                            _house = posLast < 0 ? _house : _house.Remove(posLast, 1);
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
                            _house = posStart < 0 ? _house : _house.Remove(posStart, 1);
                            var posLast = _house.LastIndexOf(" ");
                            _house = posLast < 0 ? _house : _house.Remove(posLast, 1);
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
        /// <param name="vm">ViewModel</param>
        /// <returns></returns>
        public DataTable GetFiasCode(ViewModel vm)
        {
            _db = new Model1();
            _newData = new DataTable();
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                    if (_data.Rows[i][Columns._street] == _editAdress || _data.Rows[i][Columns._street] == string.Empty || _data.Rows[i][Columns._house] == string.Empty)
                {
                    _data.Rows[i].Delete();
                }
            }
            _db.Database.CommandTimeout = 300;
            _cacheAdrr = new Dictionary<KeyAddrob, addrob30>();
            _cacheHouse = new Dictionary<KeyHouse, house30>();
            List<addrob30> _addr = _db.addrob30.ToList();
            int index = 0;
            vm.ProgBarTextDB = "Ожидайте загружаю Улицы";
            foreach (addrob30 x in _addr)
            {
                index++;
                var key = new KeyAddrob(x.AOID, x.AOGUID, x.OFFNAME);
                _cacheAdrr.Add(key, x);
            }
            List<house30> _house30 = _db.house30.ToList();
            index = 0;
            vm.ProgBarMaxValue = _addr.Count;
            foreach (house30 h in _house30)
            {
                vm.ProgBarTextDB = "Загруженно: " + index + " из " + _addr.Count;
                index++;
                var key = h.HOUSENUM == string.Empty ? new KeyHouse(h.AOGUID, "Пустота"+index.ToString()) : new KeyHouse(h.AOGUID, h.HOUSENUM);
                if(_cacheHouse.ContainsKey(key))
                {
                    if(_cacheHouse[key].UPDATEDATE < h.UPDATEDATE)
                    {
                        _cacheHouse.Remove(key);
                        _cacheHouse.Add(key, h);
                    }
                } else { _cacheHouse.Add(key, h); }
            }
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                int x = 0;
                _street = _data.Rows[i][Columns._street].ToString();
                _house = _data.Rows[i][Columns._house].ToString();
                if (_street == _editAdress || _street == string.Empty && _house == string.Empty || _street != null && _house == string.Empty) { _data.Rows[i][Columns._fiasCode] = _checkFiasColumn; }
                else
                {

                    var result = ParseFiasCodeString(_cacheAdrr, _cacheHouse);
                    _data.Rows[i][Columns._fiasCode] = result;
                    vm.ProgBarMaxValue = _data.Rows.Count;
                    vm.ProgBarTextDB = "Фиас код: " + result + "; Улица: " + _street;
                    vm.ProgBarLoadDB = i;
                    vm.ProgBarLoadCount = "Прочитано: " + i + " из " + _data.Rows.Count;
                    if (_fiasCode != _checkFiasColumn || _fiasCode != string.Empty && _house != string.Empty)
                    {
                        x++;
                        vm.CountRows = "Поподания: " + x + " из " + _data.Rows.Count;
                    }
                }
            }
            return _data;
        }

        private string ParseFiasCode(List<addrob30> query)
        {
            _fiasCode = string.Empty;
            string aoguid = string.Empty;
            foreach (addrob30 _st in query)
            {
                if (_st.AOLEVEL == 7)
                {
                    aoguid = _st.AOGUID;
                    break;
                }
            }
            var _query = _db.house30.Where(q => q.AOGUID == aoguid && q.HOUSENUM == _house).ToList();
            if (_query.Count != 0)
            {
                foreach (house30 _hs in _query)
                {
                    _fiasCode = _hs.HOUSEID;
                    break;
                }
            } else { _fiasCode = _checkFiasColumn; }
            return _fiasCode;
        }

        private string ParseFiasCodeString(Dictionary<KeyAddrob, addrob30> query, Dictionary<KeyHouse, house30> query2)
        {
            List<addrob30> _newAdrrob = new List<addrob30>();
            _fiasCode = string.Empty;
            string _aoid = string.Empty;
            string _aoguid = string.Empty;
            KeyAddrob keyA = new KeyAddrob("", "", _street);
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
            foreach (var c in _newAdrrob)
            {
                
                var _parentGuid = c.PARENTGUID;
                var _checkStreet = query.Keys.Where(k => k._aoguid == _parentGuid);
                if (keyA._offname == string.Empty && keyA._aoid == string.Empty && keyA._aoguid == string.Empty)
                {
                    foreach (var p in _checkStreet)
                    {
                        var _checkAOLevel = query[p];
                        if (_checkAOLevel.AOLEVEL == 4)
                        {
                            keyA = new KeyAddrob(c.AOID, c.AOGUID, _street);
                            break;
                        }
                    }
                } else { break; }
            }
            if (query.ContainsKey(keyA))
            {
                var _getStreet = query[keyA];
                _aoguid = _getStreet.AOGUID;
                var key = new KeyHouse(_aoguid, _house);
                if (query2.ContainsKey(key))
                {
                    _fiasCode = query2[key].HOUSEGUID;
                }
                else { _fiasCode = _checkFiasColumn; }
            } else { _fiasCode = _checkFiasColumn; }
            return _fiasCode;
            }
}

    class KeyHouse
    {
        public string _aoguid { get; private set; }
        public string _houseNum { get; private set; }
        public KeyHouse(string AOGUID, string HouseNUM)
        {
            this._aoguid = AOGUID;
            this._houseNum = HouseNUM;
        }
        public override bool Equals(object obj)
        {
            var typedObj = obj as KeyHouse;

            var result = typedObj != null;

            if (result)
            {
                result = typedObj._aoguid.Equals(_aoguid) && typedObj._houseNum.Equals(_houseNum);
            }

            return result;
        }
        public override int GetHashCode()
        {
            var result = _aoguid.GetHashCode() ^ _houseNum.GetHashCode();

            return result;
        }
    }

    class KeyAddrob
    {
        public string _aoid { get; private set; }
        public string _aoguid { get; private set; }
        public string _offname { get; private set; }

        public KeyAddrob(string aoid, string aoguid, string offName )
        {
            this._aoid = aoid;
            this._aoguid = aoguid;
            this._offname = offName;
        }
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
        public override int GetHashCode()
        {
            var result = _aoguid.GetHashCode() ^ _aoid.GetHashCode() ^ _offname.GetHashCode();

            return result;
        }

        public void Clear()
        {
            this._aoguid = string.Empty;
            this._aoid = string.Empty;
            this._offname = string.Empty;
        }

    }
}

