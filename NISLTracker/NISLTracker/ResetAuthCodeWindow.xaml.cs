// ************************************************************************************
//
// 文件名(File Name):            ResetAuthCodeWindow.xaml.cs
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

using System.Windows;
using System.Windows.Input;

namespace NISLTracker
{
    /// <summary>
    /// ResetAuthCodeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ResetAuthCodeWindow : Window
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ResetAuthCodeWindow()
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
        }

        /// <summary>
        /// 重置授权码按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetAuthCode_Click(object sender, RoutedEventArgs e)
        {
            //如果所有输入框皆不为空
            if (!txtUserName.Text.Equals("") && !txtAuthCode.Password.Equals("") && !txtRepeat.Password.Equals("") && !txtManagerAuthCode.Password.Equals(""))
            {
                //按用户名从缓存中获取系统当前用户对象
                User user = App.GetUserByUserName(txtUserName.Text);

                //从缓存中获取系统管理员对象
                User manager = App.GetManager();

                //获取输入的系统管理员授权码的密文
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

                    //清空两个授权码文本框
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

                //按用户名更新数据库中用户的授权码和安全戳
                int result = UserDAO.UpdateAuthCodeAndSecStampByUserName(user.UserName, ciphertextOfUser, securityStamp);

                //如果更新成功，即数据库受影响行数为1
                if (result == 1)
                {
                    //按用户名更新缓存中用户的授权码和安全戳
                    App.UpdateAuthCodeAndSecStampByUserName(user.UserName, ciphertextOfUser, securityStamp);
                    MessageBox.Show("授权码重置成功！", "重置成功", MessageBoxButton.OK, MessageBoxImage.None);
                }
                else
                    MessageBox.Show("授权码重置失败，请稍后重试。", "重置失败", MessageBoxButton.OK, MessageBoxImage.Error);

                //关闭本窗口
                Close();
            }
            else
                MessageBox.Show("当前输入不完整，请检查并补充后重试。", "输入不完整", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        /// <summary>
        /// 窗口关闭事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, System.EventArgs e)
        {
            //初始化主窗口的重置授权码窗口值
            MainWindow.resetAuthCodeWindow = null;
        }
    }
}
