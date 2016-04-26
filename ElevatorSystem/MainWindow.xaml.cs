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

namespace ElevatorSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ElevatorCtrl elevatorctrl = new ElevatorCtrl(5);
        Elevator test = new Elevator();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void addTaskInside(object sender, RoutedEventArgs e)
        {
            //Binding testBinding = new Binding("Size");
            //testBinding.Mode = BindingMode.OneWay;
            //testBinding.Source = elevatorctrl;

            //BindingOperations.SetBinding(ele1_1, Button.ContentProperty, testBinding);
            Button caller = (Button)sender;
            elevatorctrl.addTaskInside(21-Grid.GetRow(caller), Grid.GetColumn(caller));
            caller.IsEnabled = false;
        }
        private void addTaskOutside(object sender, RoutedEventArgs e)
        {
            Button caller = (Button)sender;
            elevatorctrl.addTaskOutside(21 - Grid.GetRow(caller), Grid.GetColumn(caller) == 6);
            caller.IsEnabled = false;
        }
    }
}
