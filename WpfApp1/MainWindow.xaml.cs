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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr programHandle;
        public MainWindow()
        {
            InitializeComponent();
            SendMsgToProgman();
            this.Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //throw new NotImplementedException();
                // 设置当前窗口为 Program Manager的子窗口
                Win32Func.SetParent(new WindowInteropHelper(this).Handle, programHandle);
                //设置当前界面的宽高
                this.Left = 0.0;
                this.Top = 0.0;
                this.Height = SystemParameters.WorkArea.Height;
                this.Width = SystemParameters.WorkArea.Width;////得到屏幕工作区域宽度
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void SendMsgToProgman()
        {
            try
            {
                // 桌面窗口句柄，在外部定义，用于后面将我们自己的窗口作为子窗口放入
                programHandle = Win32Func.FindWindow("Progman", null);

                IntPtr result = IntPtr.Zero;
                // 向 Program Manager 窗口发送消息 0x52c 的一个消息，超时设置为2秒
                Win32Func.SendMessageTimeout(programHandle, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 2, result);

                // 遍历顶级窗口
                Win32Func.EnumWindows((hwnd, lParam) =>
                {
                    // 找到第一个 WorkerW 窗口，此窗口中有子窗口 SHELLDLL_DefView，所以先找子窗口
                    if (Win32Func.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                    {
                        // 找到当前第一个 WorkerW 窗口的，后一个窗口，及第二个 WorkerW 窗口。
                        IntPtr tempHwnd = Win32Func.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

                        // 隐藏第二个 WorkerW 窗口
                        Win32Func.ShowWindow(tempHwnd, 0);
                    }
                    return true;
                }, IntPtr.Zero);


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
