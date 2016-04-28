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
using System.ComponentModel;

namespace ElevatorSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ElevatorCtrl elevatorctrl = new ElevatorCtrl(5);
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
     
        public MainWindow()
        {
            InitializeComponent();
            bindings();
            backgroundWorker.DoWork += DoWorkHandler;
            backgroundWorker.RunWorkerAsync();
        }

        private void DoWorkHandler(object sender, DoWorkEventArgs e)
        {
            elevatorctrl.run();
        }
        //bindings
        private void addTaskInside(object sender, RoutedEventArgs e)
        {
            Button caller = (Button)sender;
            elevatorctrl.addTaskInside(21 - Grid.GetRow(caller), Grid.GetColumn(caller));
            //caller.IsEnabled = false;
        }
        private void addTaskOutside(object sender, RoutedEventArgs e)
        {
            Button caller = (Button)sender;
            elevatorctrl.addTaskOutside(21 - Grid.GetRow(caller), Grid.GetColumn(caller) == 5);
            //caller.IsEnabled = false;
        }
        private void Stop(object sender, RoutedEventArgs e)
        {
            Button caller = (Button)sender;
            if (elevatorctrl.Elevator[Grid.GetColumn(caller)].State == STATE.STOP)
                elevatorctrl.Elevator[Grid.GetColumn(caller)].run();
            else elevatorctrl.stop(Grid.GetColumn(caller));
        }
        private void bindings()
        {
            //绑定Floor
            Binding[] floorBinding = new Binding[5];
            ConverterFloor2Row convertfloor2row = new ConverterFloor2Row();
            for (int i = 0; i < 5; i++)
            {
                floorBinding[i] = new Binding("Elevator[" + i.ToString() + "].Floor");
                floorBinding[i].Mode = BindingMode.OneWay;
                floorBinding[i].Source = elevatorctrl;
                floorBinding[i].Converter = convertfloor2row;
                Border p = (Border)FindName("border" + i.ToString());
                p.SetBinding(Grid.RowProperty, floorBinding[i]);
            }

            //绑定按键
            //停止键
            Binding[] stopBinding = new Binding[5];
            ConverterRun2Alert convertrun2alert = new ConverterRun2Alert();
            for(int i=0;i<5;i++)
            {
                stopBinding[i] = new Binding("Elevator[" + i.ToString() + "].State");
                stopBinding[i].Mode = BindingMode.OneWay;
                stopBinding[i].Source = elevatorctrl;
                stopBinding[i].Converter = convertrun2alert;
                Button p = (Button)FindName("alert" + i.ToString());
                p.SetBinding(Button.ContentProperty, stopBinding[i]);
            }
            //内部楼层键
            Binding[,] floorBtnBinding = new Binding[5, 20];
            ConverterTask2Btn converttask2btn = new ConverterTask2Btn();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    floorBtnBinding[i, j] = new Binding("Elevator[" + i.ToString() + "].TaskInside[" + j.ToString() + "].task");
                    floorBtnBinding[i, j].Mode = BindingMode.OneWay;
                    floorBtnBinding[i, j].Source = elevatorctrl;
                    floorBtnBinding[i, j].Converter = converttask2btn;
                    string btn_name = (j > 8 ? (j + 1).ToString() : "0" + (j + 1).ToString());
                    Button p = (Button)FindName("btn" + i.ToString() + btn_name);
                    p.SetBinding(Button.IsEnabledProperty, floorBtnBinding[i, j]);
                }
            }
            //外部楼层键
            Binding[] UpBinding = new Binding[19];//上行
            for (int i = 0; i < 19; i++)
            {
                UpBinding[i] = new Binding("TaskCacheUp[" + i.ToString() + "].task");
                UpBinding[i].Mode = BindingMode.OneWay;
                UpBinding[i].Source = elevatorctrl;
                UpBinding[i].Converter = converttask2btn;
                string btn_name = (i > 8 ? "" : "0") + (i + 1).ToString();
                Button p = (Button)FindName("btn5" + btn_name);
                p.SetBinding(Button.IsEnabledProperty, UpBinding[i]);
            }
            Binding[] DownBinding = new Binding[19];//下行
            for (int i = 0; i < 19; i++)
            {
                DownBinding[i] = new Binding("TaskCacheDown[" + i.ToString() + "].task");
                DownBinding[i].Mode = BindingMode.OneWay;
                DownBinding[i].Source = elevatorctrl;
                DownBinding[i].Converter = converttask2btn;
                string btn_name = (i > 7 ? "" : "0") + (i + 2).ToString();
                Button p = (Button)FindName("btn6" + btn_name);
                p.SetBinding(Button.IsEnabledProperty, DownBinding[i]);
            }
        }

    }
}
