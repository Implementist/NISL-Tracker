// ************************************************************************************
//
// 文件名(File Name):            AddStuffWindow.xaml.cs
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
using System.Windows.Input;

namespace NISLTracker
{
    /// <summary>
    /// AddStuffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddStuffWindow : Window
    {
        /// <summary>
        /// 数据窗口对象
        /// </summary>
        private DataWindow parentWindow;

        /// <summary>
        /// 主管老师用户对象
        /// </summary>
        private User headTeacher;

        /// <summary>
        /// 系统当前使用者用户对象
        /// </summary>
        private User user;

        /// <summary>
        /// 全参构造函数
        /// </summary>
        /// <param name="ParentWindow">数据窗口对象</param>
        /// <param name="HeadTeacher">主管老师用户对象</param>
        /// <param name="User">系统当前使用者用户对象</param>
        public AddStuffWindow(DataWindow ParentWindow, User HeadTeacher, User User)
        {
            parentWindow = ParentWindow;
            headTeacher = HeadTeacher;
            user = User;

            //系统初始化组件函数
            InitializeComponent();

            //初始化文本框组件内容
            txtOwner.Text = user.UserName;
        }

        /// <summary>
        /// 窗口加载完毕事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //为窗口中第一个文本框组件设置焦点
            Keyboard.Focus(txtStuffName);
        }

        /// <summary>
        /// 添加物资按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddStuff_Click(object sender, RoutedEventArgs e)
        {
            //如果主管老师信息为空
            if (null == headTeacher)
            {
                MessageBox.Show("未查询到你所在实验室主管老师的账户信息，请先联系该老师注册本系统。", "主管老师不存在", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }

            //判断整数数值的正则表达式
            Regex regex = new Regex(@"^\d+$");

            //获取输入的主管老师授权码的密文
            string ciphertext = Encrypt.GetCiphertext(txtHeadTeacherAuthCode.Password, headTeacher.SecurityStamp);

            //如果输入合法，即：
            //1. 物资名称不为空； 2.物资估值为纯数字； 3.主管老师授权码验证成功
            if (!txtStuffName.Text.Equals("") && regex.IsMatch(txtValueOfAssessment.Text) && ciphertext.Equals(headTeacher.AuthorizationCode))
            {
                //构造新增物资对象
                Stuff stuff = new Stuff()
                {
                    StuffName = txtStuffName.Text,
                    ValueOfAssessment = Int32.Parse(txtValueOfAssessment.Text),
                    State = "Holding",
                    Owner = user.UserName,
                    CurrentHolder = user.UserName
                };

                //向数据库中插入物资信息并接收插入操作结果
                int result = StuffDAO.InsertStuff(stuff);

                //如果插入成功，即数据库受影响行数为1
                if (result == 1)
                {
                    //更新数据网格视图
                    parentWindow.UpdateDataGrid("Add", stuff, null, null);
                    MessageBox.Show("物资添加成功！", "添加成功", MessageBoxButton.OK, MessageBoxImage.None);
                }
                else
                {
                    MessageBox.Show("物资添加失败，请稍后重试。", "添加失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //关闭本窗口
                Close();
            }
            else
            {
                MessageBox.Show("输入有误，请检查后重试。", "输入有误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 窗口关闭事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //初始化数据窗口的添加物资窗口值
            DataWindow.addStuffWindow = null;
        }
    }
}
