using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Hide
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HideWindow : Window
    {
        //注册外部事件
        ShowSelected showselectedcommand = null;
        ExternalEvent showselectedEvent = null;

        KeepHide keephidecommand = null;
        ExternalEvent keephideEvent = null;

        ClearAll clearallcommand = null;
        ExternalEvent clearallevent = null;

        ObservableCollection<datagridres> datagridreslist = new ObservableCollection<datagridres>();
        public HideWindow()
        {
            
            for (int y=0;y< IdSave.Timerec.Count();y++)
            {
                if (IdSave.Instance[y].Count() < 20)
                {
                    datagridreslist.Add(new datagridres() { id = y + 1, name = IdSave.Instance[y], plan= IdSave.ViewList[y],time = IdSave.Timerec[y] });
                }
                else
                {
                    List<string> namewjudge = IdSave.Instance[y].Take(20).ToList();
                    int numname = IdSave.Instance[y].Count();
                    namewjudge.Add("...等"+ numname+"个构件");
                    datagridreslist.Add(new datagridres() { id = y + 1, name = namewjudge, plan = IdSave.ViewList[y],time = IdSave.Timerec[y] });
                }
                
            }
            InitializeComponent();
            hidewindow.ItemsSource = datagridreslist;
            //初始化外部事件
            showselectedcommand = new ShowSelected();
            showselectedEvent = ExternalEvent.Create(showselectedcommand);

            keephidecommand = new KeepHide();
            keephideEvent = ExternalEvent.Create(keephidecommand);

            clearallcommand = new ClearAll();
            clearallevent= ExternalEvent.Create(clearallcommand);
        }

        private static HideWindow _instance;

        public static HideWindow Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new HideWindow();
                }
                return _instance;
            }

        }

        //关闭按钮时触发
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            showselectedcommand.IdIndex = hidewindow.SelectedIndex;
            if (IdSave.IDS.Count() == 0)
            {
                TaskDialog.Show("PinkSecret提示", "当前管理器为空");
            }
            else
            {
                if (hidewindow.SelectedIndex == -1)
                {
                    TaskDialog.Show("PinkSecret提示", "你未选中需要恢复显示的列表行");
                }
                else
                {
                    showselectedEvent.Raise();
                }
            }                       
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            keephidecommand.IdIndex = hidewindow.SelectedIndex;
            if(IdSave.IDS.Count() == 0)
            {
                TaskDialog.Show("PinkSecret提示", "当前管理器为空");
            }
            else
            {
                if (hidewindow.SelectedIndex == -1)
                {
                    TaskDialog.Show("PinkSecret提示", "你未选中需要恢复显示的列表行");
                }
                else
                {
                    keephideEvent.Raise();
                }
            }           
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (IdSave.IDS.Count() == 0)
            {
                TaskDialog.Show("PinkSecret提示", "当前管理器为空");
            }
            else
            {
                hidewindow.ItemsSource = null;
                clearallevent.Raise();
            }
                


        }
    }
}
