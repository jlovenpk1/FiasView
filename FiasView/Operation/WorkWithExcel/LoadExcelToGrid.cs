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

namespace FiasView.Operation.WorkWithExcel
{
    class LoadExcelToGrid
    {
        private DataTable _data;
        private List<string> _pAdress;
        private string _firstColumn = string.Empty;
        private string _secondColumn = string.Empty;
        private string[] _adress;
        private string _postcode = string.Empty;
        private string _country = string.Empty;
        private string _state = string.Empty;
        private string _city = string.Empty;
        private string _street = string.Empty;
        private string _house = string.Empty;
        private struct Columns
        {
            public static string _city = "Город";
            public static string _street = "Улица";
            public static string _house = "Дом";
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
            return _data;
        }

        private void ParsingAdress(string adress)
        {
            int oldCityIndex = 0;
            int oldStreetIndex = 0;
            bool checkCity = false, checkStreet = false;
            _pAdress = adress.Split(new char[] { ',', '.' }).ToList();
            string error = string.Empty;
            List<string> city = new List<string>() { "город.", "г.", "город", "г" };
            List<string> street = new List<string>() { "улица", "ул", "у", "ул.", "улица.", "у.", "пер", "переулок" };
            List<string> house = new List<string>() { "дом.", "д.", "дом", "д" };
            try
            {
                for (int i = 0; i < _pAdress.Count; i++)
                {
                    if (_pAdress.Count < 7) { break; }
                    if (checkCity == false)
                    {
                        if (_pAdress[i] == "Астрахань" && (city.Contains(_pAdress[i + 1]) || city.Contains(_pAdress[i - 1])) || _pAdress[i] == "Астрахань")
                        {
                            _city = _pAdress[i];
                            oldCityIndex = i;
                            i++;
                            checkCity = true;

                        }
                    }
                    if (checkStreet == false)
                    {
                        if (oldCityIndex < i && (street.Contains(_pAdress[i + 1]) || street.Contains(_pAdress[i - 1])))
                        {
                            _street = _pAdress[i];
                            oldStreetIndex = i;
                            i++;
                            checkStreet = true;
                            error = string.Join(", ",_pAdress.ToArray());
                        }
                    }
                    if (house.Contains(_pAdress[i]))
                    {
                        if (oldStreetIndex < i && (house.Contains(_pAdress[i + 1]) || (house.Contains(_pAdress[i - 1]))))
                        {
                            _house = _pAdress[i];
                            break;
                        }
                    }
                    else
                    {
                        {
                            if (oldStreetIndex < i && (house.Contains(_pAdress[i - 1])))
                            {
                                _house = _pAdress[i];
                                break;
                            }
                        }
                    }
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка в: "+error);
            }
            
        }
        private string FiasCode(string[] adress)
        {
            string _code = string.Empty;

            Model1 db = new Model1();
            var result = db.addrob30.Where(x => x.OFFNAME == "Вильнюсская").Select(x => x.AOGUID);

            return _code;
        }
    }
}
