using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FiasView.Operation.WorkWithBase
{
    class Config : IDisposable
    {
        private const string config = "config.txt";
        private readonly string path = @"" + AppDomain.CurrentDomain.BaseDirectory;
        private FileStream _file;

        public void ReadConfig()
        {
            try
            {
                _file = new FileStream(config, FileMode.OpenOrCreate);
                var output = new byte[_file.Length];
                _file.Read(output,0,output.Length);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
           
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
