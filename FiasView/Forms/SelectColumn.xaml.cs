using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void _setInfo_Click(object sender, RoutedEventArgs e)
        {
            if(_adresColumn.Text == "")
            {
                _adresColumn.BorderBrush = new SolidColorBrush(Colors.Red);
                _error.Foreground = new SolidColorBrush(Colors.Red);
                _error.FontSize = 12;
                _error.Text = "Обязательное поле для ввода!";
            } else { this.Close(); }
           
        }
    }
}
