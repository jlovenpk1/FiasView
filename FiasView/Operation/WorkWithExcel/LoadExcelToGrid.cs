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

namespace FiasView.Operation.WorkWithExcel
{
    class LoadExcelToGrid
    {
        private DataTable _data;
        private Model1 _db;
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
        private struct Columns
        {
            public static string _city = "Город";
            public static string _street = "Улица";
            public static string _house = "Дом";
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
            _data.Columns.Add(Columns._fiasCode);
            for (int i = 0; i < _data.Rows.Count; i++)
            {
               _adress = _data.Rows[i][_firstColumn].ToString().Split(new char[] {','});
                ParsingAdress(_data.Rows[i][_firstColumn].ToString());
                _data.Rows[i][Columns._city] = _city;
                _data.Rows[i][Columns._street] = _street;
                _data.Rows[i][Columns._house] = _house;
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
                " улица", " ул", " у", " ул.", " улица.", " у.", " пер", " пер.", " переулок", " проспект"," пр-кт"," проспект."," п-к"," площадь"," пл"," пл."," проезд"," п-д",
                "улица ", "ул ", "у ", "ул. ", "улица. ", " у. ", "пер ", "пер. ", "переулок ","проспект ","пр-кт ","проспект. ","п-к ","площадь ","пл ","пл. ","проезд ","п-д ",
                "улица", "ул", "у", "ул.", "улица.", "у.", "пер", "переулок", "пер.","проспект","пр-кт","проспект.","п-к","площадь","пл","пл.","проезд","п-д" };
            List<string> house = new List<string>() {
                "дом.", "д.", "дом", "д",
                " дом.", " д.", " дом", " д",
                "дом. ", "д. ", "дом ", "д " };
            #region Магия Индии и Китая в одном флаконе
            try
            {
                _street = "Проверьте адрес!";
                for (int i = 0; i < _pAdress.Count; i++)
                {
                    
                    for (int x = 0; x < city.Count; x++)
                    {
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
                            _street = posLast < 0 || posLast < 0 && _pAdress[i].Length > 15 ? _street : _street.Remove(posLast, 1);
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
                            _street = posLast < 0 || posLast < 0 && _pAdress[i].Length > 15 ? _street :_street.Remove(posLast, 1);
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
                }
                //MessageBox.Show(test);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex);
            }
            #endregion
        }

        public DataTable GetFiasCode(ViewModel vm)
        {
            _db = new Model1();
            for (int i = 0; i < _data.Rows.Count; i++)
            {
                
                _street = _data.Rows[i][Columns._street].ToString();
                _house = _data.Rows[i][Columns._house].ToString();
                var query = _db.addrob30.Where(q => q.OFFNAME == _street).ToList();
                var result = query.Count > 0 ? ParseFiasCode(query) : "Фиас Код не обнаружен!";
                _data.Rows[i][Columns._fiasCode] = result;
                vm.ProgBarMaxValue = _data.Rows.Count;
                vm.ProgBarTextDB = "Фиас код: " + result + "; Улица: " + _street;
                vm.ProgBarLoadDB = i;
                vm.ProgBarLoadCount = "Прочитано: " + i + " из " + _data.Rows.Count;
                //_progress.Dispatcher.BeginInvoke(new Action(() => { _progress._progbar.DataContext = vm; }));
            }
            return _data;
        }

        private string ParseFiasCode(List<addrob30> query)
        {
            string AOGUID = string.Empty;
            foreach (addrob30 _st in query)
            {
                if (_st.AOLEVEL == 7)
                {
                    AOGUID = _st.AOGUID;
                    break;
                }
            }
            var _query = _db.house30.Where(q => q.AOGUID == AOGUID).ToList();
            if (_query.Count != 0)
            {
                foreach (house30 _hs in _query)
                {
                    if (_hs.HOUSENUM  == _house)
                    {
                        _fiasCode = _hs.HOUSEID;
                    }
                }
            }
            return _fiasCode;
        }
    }
}
