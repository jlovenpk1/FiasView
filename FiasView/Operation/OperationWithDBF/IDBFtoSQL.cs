using System;

namespace FiasView.Operation.OperationWithDBF
{
    interface IDBFtoSQL
    {
        string CountRows { get; set; }
        string ProgBarLoadCount { get; set; }
        int ProgBarMaxValue { get; set; }
        string ProgBarTextDB { get; set; }
        int ProgBarValue { get; set; }

        event Action<string> ParametrChange;

        void Dispose();
        void GetSQLData();
    }
}