using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClosedXML.Excel;
using System.Data;
using System.Threading;
using FiasView.Forms;
using FiasView.Operation.WorkWithExcel;
using FiasView.Operation.OperationWithDBF;
using FiasView.UI;
using FiasView.MVVM;
using System.Collections;

namespace FiasView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();  
        }

        private void ResetColor(object sender, MouseEventArgs e)
        {
            Canvas _canvas = sender as Canvas;
            _canvas.Background =  (SolidColorBrush) new BrushConverter().ConvertFrom("#FF3D689B");
        }

        private void Drag_Windows(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }

}
