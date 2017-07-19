// ************************************************************************************
//
// 文件名(File Name):            MainWindow.xaml.cs
//
// 数据表(Tables):               None
//
// 作者(Author):                 曹帅(Implementist)
//
// 创建日期(Create Date):        2017年07月15日
//
// 修改记录(Revision History):   
//
// ************************************************************************************

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NISLTracker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //重置授权码窗口
        public static ResetAuthCodeWindow resetAuthCodeWindow;

        //注册窗口
        public static RegisterWindow registerWindow;

        public MainWindow()
        {
            resetAuthCodeWindow = null;
            registerWindow = null;

            //系统初始化组件函数
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载完毕事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //为窗口中第一个文本框组件设置焦点
            Keyboard.Focus(txtUserName);
        }

        /// <summary>
        /// 用户名输入框文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //根据用户名文本框内容长度设置清空用户名按钮（实为一个可点击的图片）的可见性
            imgClearUserName.Visibility = txtUserName.Text.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 清空用户名输入框按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgClearUserName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //清空用户名文本框
            txtUserName.Text = "";
        }

        /// <summary>
        /// 授权码输入框文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAuthCode_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //根据用户名文本框内容长度设置清空用户名按钮（实为一个可点击的图片）的可见性
            imgClearAuthCode.Visibility = txtAuthCode.Password.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 清空授权码输入框按钮左键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgClearAuthCode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //清空授权码文本框
            txtAuthCode.Password = "";
        }

        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //如果用户名或授权码文本框为空
            if (txtUserName.Text.Equals("") || txtAuthCode.Password.Equals(""))
            {
                MessageBox.Show("用户名或授权码不能为空！", "空的用户名或授权码", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //按用户名从缓存中获得用户对象
            User user = App.GetUserByUserName(txtUserName.Text.ToString());

            //验证用户登录并接收结果
            bool verifyResult = null != user && user.AuthorizationCode.Equals(Encrypt.GetCiphertext(txtAuthCode.Password, user.SecurityStamp));

            //如果登录失败
            if (!verifyResult)
            {
                MessageBox.Show("用户名或授权码错误。", "登入系统失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //创建并初始化数据窗口
            DataWindow dataWindow = new DataWindow(user);

            //显示数据窗口
            dataWindow.Show();

            //关闭本窗口
            Close();
        }

        /// <summary>
        /// 忘记授权码面板鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panForget_MouseEnter(object sender, MouseEventArgs e)
        {
            //替换圆圈问号图标的图片资源
            imgForget.Source = new BitmapImage(new Uri("Source/forget_fill.png", UriKind.RelativeOrAbsolute));

            //更换文字标签的字体颜色
            lblForget.Foreground = new SolidColorBrush(Color.FromArgb(255, 99, 187, 243));
        }

        /// <summary>
        /// 忘记授权码面板鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panForget_MouseLeave(object sender, MouseEventArgs e)
        {
            //替换圆圈问号图标的图片资源
            imgForget.Source = new BitmapImage(new Uri("Source/forget.png", UriKind.RelativeOrAbsolute));

            //更换文字标签的字体颜色
            lblForget.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        }

        /// <summary>
        /// 忘记授权码面板的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panForget_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //如果重置授权码窗口尚未实例化
            if (null == resetAuthCodeWindow)
            {
                //创建并构造重置授权码窗口
                resetAuthCodeWindow = new ResetAuthCodeWindow();
            }

            //显示重置授权码窗口
            resetAuthCodeWindow.Show();
        }

        /// <summary>
        /// 用户注册面板鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panRegister_MouseEnter(object sender, MouseEventArgs e)
        {
            //替换圆圈加号图标的图片资源
            imgRegister.Source = new BitmapImage(new Uri("Source/register_fill.png", UriKind.RelativeOrAbsolute));

            //更换文字标签的字体颜色
            lblRegister.Foreground = new SolidColorBrush(Color.FromArgb(255, 99, 187, 243));
        }

        /// <summary>
        /// 用户注册面板鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panRegister_MouseLeave(object sender, MouseEventArgs e)
        {
            //替换圆圈加号图标的图片资源
            imgRegister.Source = new BitmapImage(new Uri("Source/register.png", UriKind.RelativeOrAbsolute));

            //更换文字标签的字体颜色
            lblRegister.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        }

        /// <summary>
        /// 用户注册面板的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panRegister_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //如果注册窗口尚未实例化
            if (null == registerWindow)
            {
                //创建并构造注册窗口
                registerWindow = new RegisterWindow();
            }

            //显示注册窗口
            registerWindow.Show();
        }

        /// <summary>
        /// 窗口关闭事件的回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //如果各子窗口实例存在，关掉这些子窗口
            if (null != resetAuthCodeWindow)
                resetAuthCodeWindow.Close();

            if (null != registerWindow)
                registerWindow.Close();
        }
    }
}
