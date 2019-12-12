using FiasView.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FiasView.Operation
{
    class LoadAllTable
    {
        Model1 _db;
        public Dictionary<int, addrob30> _cacheAdrr;
        public Dictionary<int, house30> _cacheHouse;
        public void LoadAllTables(ViewModel vm)
        {
            _db = new Model1();
            _db.Database.CommandTimeout = 300;
            _cacheAdrr = new Dictionary<int, addrob30>();
            _cacheHouse = new Dictionary<int, house30>();
            vm.ProgBarTextDB = "Выгружаю ADDROB30....";
            List<addrob30> _addr = _db.addrob30.ToList();
            int index = 0;
            foreach (addrob30 x in _addr)
            {
                index++;
                _cacheAdrr.Add(index, x);
            }
            vm.ProgBarTextDB = "Выгружаю HOUSE30....";
            List<house30> _house30 = _db.house30.ToList();
            index = 0;
            foreach (house30 h in _house30)
            {
                index++;
                _cacheHouse.Add(index, h);
            }

        }
    }
}
