using FiasView.MVVM;
using FiasView.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;
using Z.BulkOperations;

namespace FiasView.Operation.OperationWithDBF
{

    class DBFtoSQL
    {
        private OdbcConnection _odbc; // ODBC коннект, сюда мы кинем наш путь подключения
        private OpenFileDialog openFile; // Работа с проводником для открытия файла
        private OdbcCommand _command; // SQL команду пихнем сюда
        private OdbcDataReader _reader; // Для считывания
        private StringBuilder _str;
        private progressBar _progress;
        private Model1 db;
        private ViewModel mv;
        private List<addrob30> _addrob30;
        private List<house30> _house30;
        private List<room30> _room30;
        private DataTable _dt;
        private MySqlConnection _mySql;
        private OdbcDataAdapter _adapter;
        private string _select = "SELECT * from "; // Запрос SQL
        private string _connectLineS = @"Driver={Microsoft dBase Driver (*.dbf)}; SourceType=DBF;DefaultDir="; // старт строки подключения
        private string _connectLineE = @";Exclusive=No; Collate=Machine;NULL=NO;DELETED=NO; BACKGROUNDFETCH=NO"; // конец строки подключения
        private  string[] _dbfName = new string[3] { "ADDROB30.DBF", "HOUSE30.DBF", "ROOM30.DBF" }; // Имена баз, для проверки и подключения
        private string connString = "Server= localhost;Database=fias;port=3306 ;User Id= root ;password=password ;";
        private string dbfDirectory; // папка где хранится DBF файл
        private string[] dbfName; // Имя файла
        private string _tableName = string.Empty; // Имя без .DBF
        private string sqlDirectory = string.Empty; // путь до папки сохранения
        private string sqlName = string.Empty; // имя файла
        private string _columnName = string.Empty; // Имя колонки
        private string _columnType = string.Empty; // Тип колонки(int, varchar, DateTime)
        private string _insertText = string.Empty; // текст Insert операции
        private string _newText = string.Empty;
        private bool _result = false;

        /// <summary>
        /// Открыть файлы для загрузки таблиц
        /// </summary>
        /// <returns></returns>
        private bool OpenFiles()
        {
            openFile = new OpenFileDialog(); // создаем экземпляр OpenFileDialog
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == true && openFile.FileNames.Count() == 3) // если окно проводника открыто и файл выбран
            {
                _result = new Operation().CheckFiles(openFile.SafeFileNames.ToArray());
                if (_result)
                {
                    dbfDirectory = System.IO.Path.GetDirectoryName(openFile.FileName); // Загоняем путь к файлу в переменую
                    dbfName = openFile.SafeFileNames.ToArray(); // загоняем имя файла в переменную 
                }
                else { MessageBox.Show("Были выбранны не правильные файлы: " + openFile.FileNames[0] + " " + openFile.FileNames[1] + " " + openFile.FileNames[2]); }
                
            }
            else { MessageBox.Show("Выбире 3-и файла ADDROB30.DBF, HOUSE30.DBF, ROOM30.DBF или обратитесь к разработчику в АСУП"); }
            openFile.Reset(); // обнуляемся
            return _result;
        }

        /// <summary>
        /// Запустить загрузку или обновление базы данных
        /// </summary>
        async public void GetSQLData()
        {
            _progress = new progressBar();

            if (OpenFiles())
            {
                _odbc = new OdbcConnection(_connectLineS + dbfDirectory + _connectLineE ); // Подключение к DBF
                for (int i = 0; i < dbfName.Length; i++)
                {
                    _odbc.Close(); // закрывает подключение перед открытием нового подключения, иначе кутак пасс
                    try
                    {
                        if (dbfName[i] == _dbfName[0] ) { await Task.Run(new Action(() => { LoadToAddrob30(dbfName[i]); })); }
                        if (dbfName[i] == _dbfName[1]) { await Task.Run(new Action(() => { LoadToHouse30(dbfName[i]); })); }
                        if (dbfName[i] ==  _dbfName[2]) { await Task.Run(new Action(() => { LoadToRoom30(dbfName[i]); })); }
                        
                    }
                    catch (Exception er) // Мало ли
                    {
                        MessageBox.Show("Ошибка, внимательно читаем: " + er.ToString());
                    }
                }
                MessageBox.Show("Обновление закончено!");
            }
        }

        /// <summary>
        /// Загрузка таблицы ADDROB30
        /// </summary>
        /// <param name="_tableName">Имя таблицы из которой читаем</param>
        private void LoadToAddrob30(string _tableName)
         {
            
            _progress.Dispatcher.BeginInvoke(new Action(() =>{ _progress.Show(); })); // вызываем прогрес бар
            _odbc = new OdbcConnection(_connectLineS + dbfDirectory + _connectLineE); // подключение к базе данных DBF
            _tableName = _tableName.Remove(_tableName.IndexOf('.')); // получаем имя таблицы
            _str = new StringBuilder(); // Экземпляр StringBuilder 
            _odbc.Open(); // подключение к базе данных DBF
            _dt = new DataTable();
            _mySql = new MySqlConnection(connString);
            _mySql.Open();
            mv = new ViewModel()
            {
                ProgBarTextDB = "Подготовка данных",
            };
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; _progress._progbar.IsIndeterminate = true; }));
            _adapter = new OdbcDataAdapter(_select + _tableName, _odbc);
            _dt.TableName = _tableName;
            _adapter.Fill(_dt);
            var bulk = new BulkOperation(_mySql);
            mv = new ViewModel()
            {
                ProgBarTextDB = "Загружаю ADDROB30. Операция может занять до 20 минут!",
            };
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; }));
            bulk.BulkInsert(_dt);
            _mySql.Close();
            #region Old 
            //_reader = _command.ExecuteReader(); // Считываем ответ
            //_addrob30 = new List<addrob30>();

            #endregion
            #region Old Extension EF6
            //    while (_reader.Read())
            //     {
            ///*БУЛК ИНСЕТР ЕПТА MySQLBulkLoader*/
            //                #region Заполняем данные для Addrob30
            //                _addrob30.Add(new addrob30()
            //                {
            //                    AOID = _reader[2].Equals(System.DBNull.Value) ? "" : _reader.GetString(2),
            //                    ACTSTATUS = int.Parse(_reader.GetDouble(0).ToString()),
            //                    AOGUID = _reader[1].Equals(System.DBNull.Value) ? "" : _reader.GetString(1),
            //                    AOLEVEL = int.Parse(_reader.GetDouble(3).ToString()),
            //                    AREACODE = _reader[4].Equals(System.DBNull.Value) ? "" : _reader.GetString(4),
            //                    AUTOCODE = _reader[5].Equals(System.DBNull.Value) ? "" : _reader.GetString(5),
            //                    CENTSTATUS = int.Parse(_reader.GetDouble(6).ToString()),
            //                    CITYCODE = _reader[7].Equals(System.DBNull.Value) ? "" : _reader.GetString(7),
            //                    CODE = _reader[8].Equals(System.DBNull.Value) ? "" : _reader.GetString(8),
            //                    CURRSTATUS = int.Parse(_reader.GetDouble(9).ToString()),
            //                    ENDDATE = _reader.GetDateTime(10),
            //                    FORMALNAME = _reader[11].Equals(System.DBNull.Value) ? "" : _reader.GetString(11),
            //                    IFNSFL = _reader[12].Equals(System.DBNull.Value) ? "" : _reader.GetString(12),
            //                    IFNSUL = _reader[13].Equals(System.DBNull.Value) ? "" : _reader.GetString(13),
            //                    NEXTID = _reader[14].Equals(System.DBNull.Value) ? "" : _reader.GetString(14),
            //                    OFFNAME = _reader[15].Equals(System.DBNull.Value) ? "" : _reader.GetString(15),
            //                    OKATO = _reader[16].Equals(System.DBNull.Value) ? "" : _reader.GetString(16),
            //                    OKTMO = _reader[17].Equals(System.DBNull.Value) ? "" : _reader.GetString(17),
            //                    OPERSTATUS = int.Parse(_reader.GetDouble(18).ToString()),
            //                    PARENTGUID = _reader[19].Equals(System.DBNull.Value) ? "" : _reader.GetString(19),
            //                    PLACECODE = _reader[20].Equals(System.DBNull.Value) ? "" : _reader.GetString(20),
            //                    PLAINCODE = _reader[21].Equals(System.DBNull.Value) ? "" : _reader.GetString(21),
            //                    POSTALCODE = _reader[22].Equals(System.DBNull.Value) ? "" : _reader.GetString(22),
            //                    PREVID = _reader[23].Equals(System.DBNull.Value) ? "" : _reader.GetString(23),
            //                    REGIONCODE = _reader[24].Equals(System.DBNull.Value) ? "" : _reader.GetString(24),
            //                    SHORTNAME = _reader[25].Equals(System.DBNull.Value) ? "" : _reader.GetString(25),
            //                    STARTDATE = _reader.GetDateTime(26),
            //                    STREETCODE = _reader[27].Equals(System.DBNull.Value) ? "" : _reader.GetString(27),
            //                    TERRIFNSFL = _reader[28].Equals(System.DBNull.Value) ? "" : _reader.GetString(28),
            //                    TERRIFNSUL = _reader[29].Equals(System.DBNull.Value) ? "" : _reader.GetString(29),
            //                    UPDATEDATE = _reader.GetDateTime(30),
            //                    CTARCODE = _reader[31].Equals(System.DBNull.Value) ? "" : _reader.GetString(31),
            //                    EXTRCODE = _reader[32].Equals(System.DBNull.Value) ? "" : _reader.GetString(32),
            //                    SEXTCODE = _reader[33].Equals(System.DBNull.Value) ? "" : _reader.GetString(33),
            //                    LIVESTATUS = int.Parse(_reader.GetDouble(34).ToString()),
            //                    NORMDOC = _reader[35].Equals(System.DBNull.Value) ? "" : _reader.GetString(35),
            //                    PLANCODE = _reader[36].Equals(System.DBNull.Value) ? "" : _reader.GetString(36),
            //                    CADNUM = _reader[37].Equals(System.DBNull.Value) ? "" : _reader.GetString(37),
            //                    DIVTYPE = int.Parse(_reader.GetDouble(38).ToString()),
            //                });
            //                 #endregion

            //     }
            #endregion
            #region old
            //db = new Model1();
            //db.Dispose();
            //_addrob30.Clear();
            #endregion
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress._progbar.IsIndeterminate = false; }));
        }

        /// <summary>
        /// Загрузка таблицы HOUSE30
        /// </summary>
        /// <param name="_tableName">Имя таблицы из которой читаем</param>
        private void LoadToHouse30(string _tableName)
        {      
            _odbc = new OdbcConnection(_connectLineS + dbfDirectory + _connectLineE);
            _tableName = _tableName.Remove(_tableName.IndexOf('.'));
            _str = new StringBuilder(); // Экземпляр StringBuilder 
            _odbc.Open();
            _command = new OdbcCommand(_select + _tableName, _odbc); // Отпавляем команду
            _reader = _command.ExecuteReader(); // Считываем ответ
            _house30 = new List<house30>();
            mv = new ViewModel()
                {
                    ProgBarTextDB = "Подготовка HOUSE30!",
                };
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; _progress._progbar.IsIndeterminate = true; }));
            _dt = new DataTable();
            _mySql = new MySqlConnection(connString);
            _mySql.Open();
            _adapter = new OdbcDataAdapter(_select + _tableName, _odbc);
            _dt.TableName = _tableName;
            _adapter.Fill(_dt);
            var bulk = new BulkOperation(_mySql);
            mv = new ViewModel()
            {
                ProgBarTextDB = "Загружаю HOUSE30. Ожидайте, операция может занять до 20 минут!",
            };
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; }));
            bulk.BulkInsert(_dt);
            _mySql.Close();
            #region old Extension EF6
            //while (_reader.Read())
            // {
            //        _house30.Add(new house30() {
            //            HOUSEID = _reader[5].Equals(System.DBNull.Value) ? "" : _reader.GetString(5),
            //            AOGUID = _reader[0].Equals(System.DBNull.Value) ? "" : _reader.GetString(0),
            //            BUILDNUM = _reader[1].Equals(System.DBNull.Value) ? "" : _reader.GetString(1),
            //            ENDDATE = _reader.GetDateTime(2),
            //            ESTSTATUS = int.Parse(_reader.GetDouble(3).ToString()),
            //            HOUSEGUID = _reader[4].Equals(System.DBNull.Value) ? "" : _reader.GetString(4),
            //            HOUSENUM = _reader[6].Equals(System.DBNull.Value) ? "" : _reader.GetString(6),
            //            STATSTATUS = int.Parse(_reader.GetDouble(7).ToString()),
            //            IFNSFL = _reader[8].Equals(System.DBNull.Value) ? "" : _reader.GetString(8),
            //            IFNSUL = _reader[9].Equals(System.DBNull.Value) ? "" : _reader.GetString(9),
            //            OKATO = _reader[10].Equals(System.DBNull.Value) ? "" : _reader.GetString(10),
            //            OKTMO = _reader[11].Equals(System.DBNull.Value) ? "" : _reader.GetString(11),
            //            POSTALCODE = _reader[12].Equals(System.DBNull.Value) ? "" : _reader.GetString(12),
            //            STARTDATE = _reader.GetDateTime(13),
            //            STRUCNUM = _reader[14].Equals(System.DBNull.Value) ? "" : _reader.GetString(14),
            //            STRSTATUS = int.Parse(_reader.GetDouble(15).ToString()),
            //            TERRIFNSFL = _reader[16].Equals(System.DBNull.Value) ? "" : _reader.GetString(16),
            //            TERRIFNSUL = _reader[17].Equals(System.DBNull.Value) ? "" : _reader.GetString(17),
            //            UPDATEDATE = _reader.GetDateTime(18),
            //            NORMDOC = _reader[19].Equals(System.DBNull.Value) ? "" : _reader.GetString(19),
            //            COUNTER = int.Parse(_reader.GetDouble(20).ToString()),
            //            CADNUM = _reader[21].Equals(System.DBNull.Value) ? "" : _reader.GetString(21),
            //            DIVTYPE = int.Parse(_reader.GetDouble(22).ToString())
            //        });
            //}
            #endregion
            #region old
            //db = new Model1();
            //db.BulkInsert(_house30);
            //db.Dispose();
            //_house30.Clear();
            #endregion
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress._progbar.IsIndeterminate = false; }));
        }

        /// <summary>
        /// Загрузка таблицы ROOM30
        /// </summary>
        /// <param name="_tableName">Имя таблицы из которой читаем</param>
        private void LoadToRoom30(string _tableName)
        {
            _odbc = new OdbcConnection(_connectLineS + dbfDirectory + _connectLineE);
            _tableName = _tableName.Remove(_tableName.IndexOf('.'));
            _str = new StringBuilder(); // Экземпляр StringBuilder 
            _odbc.Open();
            _command = new OdbcCommand(_select + _tableName, _odbc); // Отпавляем команду
            _reader = _command.ExecuteReader(); // Считываем ответ
            _dt = new DataTable();
            _mySql = new MySqlConnection(connString);
            _mySql.Open();
            _adapter = new OdbcDataAdapter(_select + _tableName, _odbc);
            _dt.TableName = _tableName;
            mv = new ViewModel()
            {
                ProgBarTextDB = "Подготовка ROOM30",
            };
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; }));
            _adapter.Fill(_dt);
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; _progress._progbar.IsIndeterminate = true; }));
            mv = new ViewModel()
            {
                ProgBarTextDB = "Загружаю ROOM30",
            };
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress.DataContext = mv; }));
            var bulk = new BulkOperation(_mySql);
            bulk.BulkInsert(_dt);
            _mySql.Close();
            _room30 = new List<room30>();
            #region old Extesion EF6
            //while (_reader.Read())
            //{
            //    #region Заполняем данные для Addrob30
            //    _room30.Add(new room30()
            //    {
            //        ROOMID = _reader[0].Equals(System.DBNull.Value) ? "" : _reader.GetString(0),
            //        ROOMGUID = _reader[1].Equals(System.DBNull.Value) ? "" : _reader.GetString(1),
            //        HOUSEGUID = _reader[2].Equals(System.DBNull.Value) ? "" : _reader.GetString(2),
            //        REGIONCODE = _reader[3].Equals(System.DBNull.Value) ? "" : _reader.GetString(3),
            //        FLATNUMBER = _reader[4].Equals(System.DBNull.Value) ? "" : _reader.GetString(4),
            //        FLATTYPE = int.Parse(_reader.GetDouble(5).ToString()),
            //        ROOMNUMBER = _reader[6].Equals(System.DBNull.Value) ? "" : _reader.GetString(6),
            //        ROOMTYPE = _reader[7].Equals(System.DBNull.Value) ? "" : _reader.GetString(7),
            //        CADNUM = _reader[8].Equals(System.DBNull.Value) ? "" : _reader.GetString(8),
            //        ROOMCADNUM = _reader[9].Equals(System.DBNull.Value) ? "" : _reader.GetString(9),
            //        POSTALCODE = _reader[10].Equals(System.DBNull.Value) ? "" : _reader.GetString(10),
            //        UPDATEDATE = _reader.GetDateTime(11),
            //        PREVID = _reader[12].Equals(System.DBNull.Value) ? "" : _reader.GetString(12),
            //        NEXTID = _reader[13].Equals(System.DBNull.Value) ? "" : _reader.GetString(13),
            //        OPERSTATUS = int.Parse(_reader.GetDouble(14).ToString()),
            //        STARTDATE = _reader.GetDateTime(15),
            //        ENDDATE = _reader.GetDateTime(16),
            //        LIVESTATUS = int.Parse(_reader.GetDouble(17).ToString()),
            //        NORMDOC = _reader[18].Equals(System.DBNull.Value) ? "" : _reader.GetString(18)
            //    });
            //    #endregion
            //}
            #endregion
            #region Old
            //db = new Model1();
            //db.room30.BulkInsert(_room30);
            //db.Dispose();
            //_room30.Clear();
            #endregion
            _progress.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { _progress._progbar.IsIndeterminate = false;  _progress.Close(); }));
        }
    }
 }

