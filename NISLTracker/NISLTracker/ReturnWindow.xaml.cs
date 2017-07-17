// ************************************************************************************
//
// 文件名(File Name):            ReturnWindow.xaml.cs
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
    /// ReturnWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReturnWindow : Window
    {
        /// <summary>
        /// 数据窗口对象
        /// </summary>
        private DataWindow parentWindow;

        /// <summary>
        /// 当前物资对象
        /// </summary>
        private Stuff stuff;

        /// <summary>
        /// 当前物资拥有者用户对象
        /// </summary>
        private User owner;

        /// <summary>
        /// 系统当前使用者用户对象
        /// </summary>
        private User user;

        /// <summary>
        /// 全参构造函数
        /// </summary>
        /// <param name="ParentWindow">数据窗口对象</param>
        /// <param name="Stuff">当前物资对象</param>
        /// <param name="Owner">当前物资拥有者用户对象</param>
        /// <param name="User">系统当前使用者用户对象</param>
        public ReturnWindow(DataWindow ParentWindow, Stuff Stuff, User Owner, User User)
        {
            parentWindow = ParentWindow;
            stuff = Stuff;
            owner = Owner;
            user = User;

            //系统初始化组件函数
            InitializeComponent();

            //初始化文本框组件内容
            txtStuffName.Text = stuff.StuffName;
            txtValueOfAssessment.Text = stuff.ValueOfAssessment.ToString();
            txtOwner.Text = stuff.Owner;
        }

        /// <summary>
        /// 窗口加载完毕事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //为窗口中第一个文本框组件设置焦点
            Keyboard.Focus(txtOwnerAuthCode);
        }

        /// <summary>
        /// 归还按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            //获取输入的当前物资拥有者授权码的密文
            string ciphertext = Encrypt.GetCiphertext(txtOwnerAuthCode.Password, owner.SecurityStamp);

            //如果当前物资拥有者授权码验证失败
            if (!ciphertext.Equals(owner.AuthorizationCode))
            {
                MessageBox.Show("验证失败，请检查您的授权码是否正确并重试。", "验证失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //按物资Id号更新数据库中物资的状态当前持有者并接收更新结果
            int result = StuffDAO.UpdateStateAndCurrentHolderByStuffId(stuff.StuffId, "Holding", owner.UserName);

            //如果更新成功，即数据库受影响行数为1
            if (result == 1)
            {
                parentWindow.UpdateDataGrid("Return", null, null, null);
                MessageBox.Show("物资归还成功！", "归还成功", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                MessageBox.Show("物资归还失败，请稍后重试。", "归还失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //关闭本窗口
            Close();
        }

        /// <summary>
        /// 窗口关闭事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, System.EventArgs e)
        {
            //初始化数据窗口的归还窗口值
            DataWindow.returnWindow = null;
        }
    }
}
