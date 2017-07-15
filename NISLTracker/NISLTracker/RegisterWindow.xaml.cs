using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private static string[] IDENTITY = { "Student", "Teacher", "Manager" };

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtUserName);
            cmbxIdentity.SelectedIndex = 0;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!txtUserName.Text.Equals("") && !txtAuthCode.Password.Equals("") && !txtRepeat.Password.Equals("") && !txtLaboratory.Text.Equals(""))
            {
                User user = App.GetUserByUserName(txtUserName.Text);
                User manager = App.GetManager();
                string ciphertextOfManager = Encrypt.GetCiphertext(txtManagerAuthCode.Password, manager.SecurityStamp);

                //用于匹配数字的正则表达式
                Regex regex = new Regex(@"^\d+$");

                //如果已存在该用户
                if (null != user)
                {
                    MessageBox.Show("该用户已注册，请直接登录。", "已注册", MessageBoxButton.OK, MessageBoxImage.Asterisk);
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

                //如果实验室输入框的输入值不全是数字字符
                if (!regex.IsMatch(txtLaboratory.Text))
                {
                    MessageBox.Show("实验室号输入有误，请输入整数数值。", "输入有误", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtLaboratory.Text = "";
                    return;
                }

                //管理员授权码验证结果
                bool managerVerified = false;

                switch (cmbxIdentity.SelectedIndex)
                {
                    //如果注册身份是学生，可以免去管理员验证
                    case 0:
                        managerVerified = true;
                        break;
                    //如果注册身份是老师或管理员，必须经过管理员验证
                    case 1:
                    case 2:
                        managerVerified = ciphertextOfManager.Equals(manager.AuthorizationCode);
                        break;
                }

                //如果管理员授权码不正确
                if (!managerVerified)
                {
                    MessageBox.Show("管理员授权码错误，请重新输入。", "授权码错误", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                //生成安全戳和授权码密文
                string securityStamp = Encrypt.GetSecurityStamp();
                string ciphertextOfUser = Encrypt.GetCiphertext(txtAuthCode.Password, securityStamp);

                //构造新用户对象
                user = new User()
                {
                    UserName = txtUserName.Text,
                    AuthorizationCode = ciphertextOfUser,
                    SecurityStamp = securityStamp,
                    Identity = IDENTITY[cmbxIdentity.SelectedIndex],
                    Laboratory = Int32.Parse(txtLaboratory.Text)
                };

                //如果注册身份时管理员，改变原管理员的身份为学生
                if (cmbxIdentity.SelectedIndex == 2)
                {
                    UserDAO.UpdateIdentityByUserName(manager.UserName, "Student");
                    App.UpdateIdentityByUserName(manager.UserName, "Student");
                }

                //更新数据库中的数据
                int result = UserDAO.InsertUser(user);

                if (result == 1)
                {
                    //更新缓存中的数据
                    App.InsertUser(user);
                    MessageBox.Show("用户注册成功！", "注册成功", MessageBoxButton.OK, MessageBoxImage.None);
                }
                else
                    MessageBox.Show("用户注册失败，请稍后重试。", "注册失败", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            else
                MessageBox.Show("当前输入不完整，请检查并补充后重试。", "输入不完整", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void cmbxIdentity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbxIdentity.SelectedIndex)
            {
                case 0:
                    lblManagerAuthCode.Visibility = Visibility.Hidden;
                    txtManagerAuthCode.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    lblManagerAuthCode.Visibility = Visibility.Visible;
                    txtManagerAuthCode.Visibility = Visibility.Visible;
                    break;
                case 2:
                    lblManagerAuthCode.Visibility = Visibility.Visible;
                    txtManagerAuthCode.Visibility = Visibility.Visible;
                    MessageBox.Show("警告：同时只能存在一名管理员，此操作只限换届或交接时执行。", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }
    }
}
