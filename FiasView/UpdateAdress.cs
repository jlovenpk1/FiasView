using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FiasView
{
    class UpdateAdress
    {
        private Model1 db;
        private addrob30 _adrr;
        private house30 _house;
        private room30 _room;

        /// <summary>
        /// Сформировать адреса из базы ФИАС с номерами квартир 
        /// </summary>
        public void GetAllAdressWithRoom()
        {
            using (db = new Model1())
            {
                
            }
            //db = new Model1();

            foreach (var s in db.addrob30)
                {
                    var x = s.SHORTNAME;
                    var c = s.OFFNAME;
                    var v = _adrr.NORMDOC;
                }
        }
        /// <summary>
        /// Сформировать адреса из базы ФИАС без номеров квартир
        /// </summary>
        public void GetAllAdressWithHouse()
        {

        }
        /// <summary>
        /// Сформировать адреса из базы ФИАС только с улицами
        /// </summary>
        public void GetAllAdressWithStreet()
        {

        }
    }
}
