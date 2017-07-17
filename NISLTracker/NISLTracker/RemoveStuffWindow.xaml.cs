// ************************************************************************************
//
// 文件名(File Name):            RemoveStuffWindow.xaml.cs
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
    /// RemoveStuffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemoveStuffWindow : Window
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
        /// 主管老师用户对象
        /// </summary>
        private User headTeacher;

        /// <summary>
        /// 系统当前用户对象
        /// </summary>
        private User user;

        /// <summary>
        /// 全参构造函数
        /// </summary>
        /// <param name="ParentWindow">数据窗口对象</param>
        /// <param name="Stuff">当前物资对象</param>
        /// <param name="HeadTeacher">主管老师用户对象</param>
        /// <param name="User">系统当前用户对象</param>
        public RemoveStuffWindow(DataWindow ParentWindow, Stuff Stuff, User HeadTeacher, User User)
        {
            parentWindow = ParentWindow;
            stuff = Stuff;
            headTeacher = HeadTeacher;
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
            Keyboard.Focus(txtHeadTeacherAuthCode);
        }

        /// <summary>
        /// 删除物资按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveStuff_Click(object sender, RoutedEventArgs e)
        {
            //获取输入的主管老师授权码的密文
            string ciphertext = Encrypt.GetCiphertext(txtHeadTeacherAuthCode.Password, headTeacher.SecurityStamp);

            //如果主管老师授权码验证失败
            if (!ciphertext.Equals(headTeacher.AuthorizationCode))
            {
                MessageBox.Show("验证失败，请检查您的授权码是否正确并重试。", "验证失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            //按物资Id号从数据库中删除物资并接收删除结果
            int result = StuffDAO.DeleteStuff(stuff.StuffId);

            //如果删除成功，即数据库受影响行数为1
            if (result == 1)
            {
                parentWindow.UpdateDataGrid("Remove", null, null, null);
                MessageBox.Show("物资删除成功！", "删除成功", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                MessageBox.Show("物资删除失败，请稍后重试。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Error);
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
            //初始化数据窗口的删除物资窗口值
            DataWindow.removeStuffWindow = null;
        }
    }
}
