using RealSenseWrapper.Core;
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

namespace GreenScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       

        private MainViewModel vm;
        public MainWindow()
        {
            InitializeComponent();

            vm = new  MainViewModel();
            vm.Intialize();

            
            this.DataContext = vm;

            //vm.Manager.PropertyChanged += Manager_PropertyChanged;

        }

        private void Manager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SegBitmapSource")
                this.SegImage.Source = vm.Manager.SegBitmapSource;

        }
    }
}
