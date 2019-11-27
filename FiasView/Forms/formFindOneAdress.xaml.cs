using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FiasView.Forms
{
    /// <summary>
    /// Логика взаимодействия для formFindOneAdress.xaml
    /// </summary>
    public partial class formFindOneAdress : Window
    {
        public formFindOneAdress()
        {
            InitializeComponent();
        }

        private void _formCloseAdress_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
