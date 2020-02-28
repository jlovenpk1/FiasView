using FiasView.Operation.OperationWithDBF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FiasView.Operation.WorkWithBase
{
    class UpdateBase : IUpdateBase
    {
        private IDBFtoSQL _dbftosql;
        public UpdateBase(IDBFtoSQL _dbftosql)
        {
            this._dbftosql = _dbftosql;
        }
        public void Update()
        {

            using (var db = new Model1())
            {
                if (db.Database.Exists() == false)
                {
                    if (MessageBox.Show("База отсутствует, создать новую?", "База данных не обнаруженна", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("Создано");
                        db.Database.Create();
                        _dbftosql.GetSQLData();
                    }
                }

                if (db.Database.Exists() == true)
                {
                    if (MessageBox.Show("База данных существует, обновить?", "База данных обнаруженна", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("Обновляемся");
                        db.Database.Delete();
                        db.Database.Create();
                        _dbftosql.GetSQLData();
                    }
                }
            }
        }
    }
}
