using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;

namespace NISLTracker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtUserName);
        }

        /// <summary>
        /// 用户名输入框文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            imgClearUserName.Visibility = txtUserName.Text.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 清空用户名输入框按钮鼠标左键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgClearUserName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtUserName.Text = "";
        }

        /// <summary>
        /// 授权码输入框文本变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAuthorizationCode_PasswordChanged(object sender, RoutedEventArgs e)
        {
            imgClearAuthorizationCode.Visibility = txtAuthorizationCode.Password.Length > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 清空授权码输入框按钮左键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgClearAuthorizationCode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtAuthorizationCode.Password = "";
        }

        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!txtUserName.Text.Equals("") && !txtAuthorizationCode.Password.Equals(""))
            {
                User user = App.GetUserByUserName(txtUserName.Text.ToString());
                bool verifyResult = null != user && user.AuthorizationCode.Equals(Encrypt.GetCiphertext(txtAuthorizationCode.Password, user.SecurityStamp));
                if (verifyResult)
                {
                    DataWindow dataWindow = new DataWindow(user);
                    dataWindow.Show();
                    Close();
                }
                else
                    MessageBox.Show("用户名或授权码错误。", "登入系统失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("用户名或授权码不能为空！", "空的用户名或授权码", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void panForget_MouseEnter(object sender, MouseEventArgs e)
        {
            imgForget.Source = new BitmapImage(new Uri("Source/forget_fill.png", UriKind.RelativeOrAbsolute));
            lblForget.Foreground = new SolidColorBrush(Color.FromArgb(255, 99, 187, 243));
        }

        private void panForget_MouseLeave(object sender, MouseEventArgs e)
        {
            imgForget.Source = new BitmapImage(new Uri("Source/forget.png", UriKind.RelativeOrAbsolute));
            lblForget.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        }

        private void panForget_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetAuthCodeWindow resetAuthWindow = new ResetAuthCodeWindow();
            resetAuthWindow.Show();
        }

        private void panRegister_MouseEnter(object sender, MouseEventArgs e)
        {
            imgRegister.Source = new BitmapImage(new Uri("Source/register_fill.png", UriKind.RelativeOrAbsolute));
            lblRegister.Foreground = new SolidColorBrush(Color.FromArgb(255, 99, 187, 243));
        }

        private void panRegister_MouseLeave(object sender, MouseEventArgs e)
        {
            imgRegister.Source = new BitmapImage(new Uri("Source/register.png", UriKind.RelativeOrAbsolute));
            lblRegister.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        }

        private void panRegister_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
        }
    }
}
