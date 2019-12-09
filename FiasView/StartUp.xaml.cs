using FiasView.MVVM;
using FiasView.Operation;
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

namespace FiasView
{
    /// <summary>
    /// Логика взаимодействия для StartUp.xaml
    /// </summary>
    public partial class StartUp : Window
    {
        ViewModel vm;
         public StartUp()
        {
            InitializeComponent();
            vm = new ViewModel();
            vm.LoadStartUp(vm);
            DataContext = vm;
        }
    }
}
