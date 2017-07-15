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
using System.Windows.Shapes;

namespace NISLTracker
{
    /// <summary>
    /// ResetAuthCodeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ResetAuthCodeWindow : Window
    {
        public ResetAuthCodeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口加载完毕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //给第一个输入框赋予焦点
            Keyboard.Focus(txtUserName);
        }

        /// <summary>
        /// 重置授权码按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetAuthCode_Click(object sender, RoutedEventArgs e)
        {
            //如果四个输入框皆不为空
            if (!txtUserName.Text.Equals("") && !txtAuthCode.Password.Equals("") && !txtRepeat.Password.Equals("") && !txtManagerAuthCode.Password.Equals(""))
            {
                User user = App.GetUserByUserName(txtUserName.Text);
                User manager = App.GetManager();
                string ciphertextOfManager = Encrypt.GetCiphertext(txtManagerAuthCode.Password, manager.SecurityStamp);

                //如果未查找到该用户
                if (null == user)
                {
                    MessageBox.Show("该用户不存在，请直接注册。", "用户不存在", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    Close();
                }

                //如果两次输入不匹配
                if (!txtAuthCode.Password.Equals(txtRepeat.Password))
                {
                    MessageBox.Show("两次输入不匹配，请重新输入。", "输入不匹配", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    txtAuthCode.Password = "";
                    txtRepeat.Password = "";
                    return;
                }

                //如果管理员授权码不正确
                if (!ciphertextOfManager.Equals(manager.AuthorizationCode))
                {
                    MessageBox.Show("管理员授权码错误，请重新输入。", "授权码错误", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                //生成新的安全戳和授权码密文
                string securityStamp = Encrypt.GetSecurityStamp();
                string ciphertextOfUser = Encrypt.GetCiphertext(txtAuthCode.Password, securityStamp);

                //更新数据库中的数据
                int result = UserDAO.UpdateAuthCodeAndSecStampByUserName(user.UserName, ciphertextOfUser, securityStamp);

                if (result == 1)
                {
                    //更新缓存中的数据
                    App.UpdateAuthCodeAndSecStampByUserName(user.UserName, ciphertextOfUser, securityStamp);
                    MessageBox.Show("授权码重置成功！", "重置成功", MessageBoxButton.OK, MessageBoxImage.None);
                }
                else
                    MessageBox.Show("授权码重置失败，请稍后重试。", "重置失败", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            else
                MessageBox.Show("当前输入不完整，请检查并补充后重试。", "输入不完整", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
