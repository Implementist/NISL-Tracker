// ************************************************************************************
//
// 文件名(File Name):            RegisterWindow.xaml.cs
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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NISLTracker
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        /// <summary>
        /// 用户身份常量数组
        /// </summary>
        private static string[] IDENTITY = { "Student", "Teacher", "Manager" };

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public RegisterWindow()
        {
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

            //初始化下拉列表选中项
            cmbxIdentity.SelectedIndex = 0;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            //如果所有输入框都非空
            if (!txtUserName.Text.Equals("") && !txtAuthCode.Password.Equals("") && !txtRepeat.Password.Equals("") && !txtLaboratory.Text.Equals(""))
            {
                //按用户名从缓存中获取系统当前用户对象
                User user = App.GetUserByUserName(txtUserName.Text);

                //从缓存中获取系统管理员对象
                User manager = App.GetManager();

                //获取输入的系统管理员授权码的密文
                string ciphertextOfManager = Encrypt.GetCiphertext(txtManagerAuthCode.Password, manager.SecurityStamp);

                //用于匹配数字的正则表达式
                Regex regex = new Regex(@"^\d+$");

                //如果已存在该用户
                if (null != user)
                {
                    MessageBox.Show("该用户已注册，请直接登录。", "已注册", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    Close();
                    return;
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

                //生成新用户的安全戳和授权码密文
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

                //向数据库中的插入新用户并接收插入结果
                int result = UserDAO.InsertUser(user);

                //如果插入成功，即即数据库受影响行数为1
                if (result == 1)
                {
                    //更新缓存中的数据
                    App.InsertUser(user);
                    MessageBox.Show("用户注册成功！", "注册成功", MessageBoxButton.OK, MessageBoxImage.None);

                    //如果注册身份时管理员
                    if (cmbxIdentity.SelectedIndex == 2)
                    {
                        //更新数据库中原管理员的身份为学生
                        UserDAO.UpdateIdentityByUserName(manager.UserName, "Student");

                        //更新缓存中原管理员的身份为学生
                        App.UpdateIdentityByUserName(manager.UserName, "Student");
                    }
                }
                else
                    MessageBox.Show("用户注册失败，请稍后重试。", "注册失败", MessageBoxButton.OK, MessageBoxImage.Error);

                //关闭本窗口
                Close();
            }
            else
                MessageBox.Show("当前输入不完整，请检查并补充后重试。", "输入不完整", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        /// <summary>
        /// 用户身份下拉列表选中项改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbxIdentity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //根据下拉列表选中项改变管理员授权码文本框的可见性
            switch (cmbxIdentity.SelectedIndex)
            {
                //case 学生
                case 0:
                    lblManagerAuthCode.Visibility = Visibility.Hidden;
                    txtManagerAuthCode.Visibility = Visibility.Hidden;
                    break;

                //case 老师
                case 1:
                    lblManagerAuthCode.Visibility = Visibility.Visible;
                    txtManagerAuthCode.Visibility = Visibility.Visible;
                    break;

                //case 管理员
                case 2:
                    lblManagerAuthCode.Visibility = Visibility.Visible;
                    txtManagerAuthCode.Visibility = Visibility.Visible;

                    MessageBox.Show("警告：同时只能存在一名管理员，此操作只限换届或交接时执行。", "警告", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        /// <summary>
        /// 窗口关闭事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //初始化主窗口的注册窗口值
            MainWindow.registerWindow = null;
        }
    }
}
