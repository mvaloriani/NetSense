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

namespace Gestures
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

            vm = new MainViewModel();
            vm.Intialize();


            this.DataContext = vm;

        }

       

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            vm.Manager.EnableGestureStream();
            vm.Message = "Gesture Enabled";
            this.StartButton.IsEnabled = false;
            this.StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            vm.Manager.DisableGestureStream();
            vm.Message = "Gesture Disabled";

            this.StartButton.IsEnabled = true;
            this.StopButton.IsEnabled = false;
        }
    }
}
