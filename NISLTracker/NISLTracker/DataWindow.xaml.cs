// ************************************************************************************
//
// 文件名(File Name):            DataWindow.xaml.cs
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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NISLTracker
{
    /// <summary>
    /// DataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataWindow : Window
    {
        /// <summary>
        /// 借入窗口
        /// </summary>
        public static BorrowWindow borrowWindow;

        /// <summary>
        /// 归还窗口
        /// </summary>
        public static ReturnWindow returnWindow;

        /// <summary>
        /// 添加物资窗口
        /// </summary>
        public static AddStuffWindow addStuffWindow;

        /// <summary>
        /// 删除物资窗口
        /// </summary>
        public static RemoveStuffWindow removeStuffWindow;

        /// <summary>
        /// 系统当前使用者用户对象
        /// </summary>
        private User user;

        /// <summary>
        /// 全参构造函数
        /// </summary>
        /// <param name="User">系统当前使用者用户对象</param>
        public DataWindow(User User)
        {
            borrowWindow = null;
            returnWindow = null;
            addStuffWindow = null;
            removeStuffWindow = null;
            user = User;

            //系统初始化组件函数
            InitializeComponent();
        }

        /// <summary>
        /// 全部物资单选按钮被选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbAllStuffs_Checked(object sender, RoutedEventArgs e)
        {
            //从数据库中查找全部物资
            List<Stuff> stuffs = StuffDAO.QueryAllStuffs();

            //为数据网格添加数据源
            dgrdStuffInfo.ItemsSource = stuffs;

            //隐藏添加物资按钮（实为一个可点击的图片）
            imgAddStuff.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 我的物资单选按钮被选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbMyStuffs_Checked(object sender, RoutedEventArgs e)
        {
            //从数据库中查找我的物资
            List<Stuff> stuffs = StuffDAO.QueryStuffByOwner(user.UserName);

            //为数据网格添加数据源
            dgrdStuffInfo.ItemsSource = stuffs;

            //显示添加物资按钮（实为一个可点击的图片）
            imgAddStuff.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 借入物资单选按钮被选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbBorrowedStuffs_Checked(object sender, RoutedEventArgs e)
        {
            //从数据库中查找借入物资
            List<Stuff> stuffs = StuffDAO.QueryStuffByCurrentHolder(user.UserName);

            //为数据网格添加数据源
            dgrdStuffInfo.ItemsSource = stuffs;

            //隐藏添加物资按钮（实为一个可点击的图片）
            imgAddStuff.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 数据网格选中项变更事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgrdStuffInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //获取数据网格当前选中行的行号
            int selectedIndex = dgrdStuffInfo.SelectedIndex;

            //判断选中行的行号，避免切换绑定资源时引发的异常
            if (selectedIndex > -1)
            {
                //取得数据网格当前的物资对象表
                Stuff stuff = dgrdStuffInfo.SelectedItem as Stuff;

                //如果物资处于可借状态——即： 1.用户处于所有物资界面；
                //2.该物资当前被拥有者持有； 3.该物资的拥有者不是当前用户。
                if ((bool)rdbAllStuffs.IsChecked && stuff.State.Equals("Holding") && !stuff.Owner.Equals(user.UserName))
                {
                    //按用户名获取缓存中的当前物资拥有者用户对象
                    User owner = App.GetUserByUserName(stuff.Owner);

                    //如果借入窗口尚未实例化
                    if (null == borrowWindow)
                    {
                        //创建并初始化借入窗口对象
                        borrowWindow = new BorrowWindow(this, stuff, owner, user);
                    }

                    //显示借入窗口
                    borrowWindow.Show();
                }

                //如果物资处于可还状态——即： 1.用户处于借入物资界面；
                //2.该物资当前被借出； 3.该物资的当前持有者是当前用户。
                else if ((bool)rdbBorrowedStuffs.IsChecked && stuff.State.Equals("LentOut") && stuff.CurrentHolder.Equals(user.UserName))
                {
                    //按用户名获取缓存中的当前物资拥有者用户对象
                    User owner = App.GetUserByUserName(stuff.Owner);

                    //如果归还窗口尚未实例化
                    if (null == returnWindow)
                    {
                        //创建并初始化归还物口对象
                        returnWindow = new ReturnWindow(this, stuff, owner, user);
                    }

                    //显示归还窗口
                    returnWindow.Show();
                }

                //如果物资处于可删除状态——即： 1.用户处于我的物资界面；
                //2.该物资的拥有者不是当前用户。
                else if ((bool)rdbMyStuffs.IsChecked && stuff.Owner.Equals(user.UserName))
                {
                    //如果该物资当前被借出
                    if (!stuff.State.Equals("Holding"))
                    {
                        MessageBox.Show("该物资当前处于借出状态，请先执行归还流程。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        return;
                    }

                    //按实验室号获取缓存中的主管老师用户对象
                    User headTeacher = App.GetTeacherByLaboratory(user.Laboratory);

                    //如果未查询到该实验室的主管老师
                    if (null == headTeacher)
                    {
                        MessageBox.Show("未查询到主管老师的相关信息，请通知该主管老师注册本系统或联系系统管理员。", "错误的用户信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        return;
                    }

                    //如果删除物资窗口尚未实例化
                    if (null == removeStuffWindow)
                    {
                        //创建并初始化删除物资窗口
                        removeStuffWindow = new RemoveStuffWindow(this, stuff, headTeacher, user);
                    }

                    //显示删除物资窗口
                    removeStuffWindow.Show();
                }
            }
        }

        /// <summary>
        /// 添加物资按钮（实为一个可点击的图片）点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgAddStuff_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //按实验室号获取缓存中的主管老师用户对象
            User headTeacher = App.GetTeacherByLaboratory(user.Laboratory);

            //如果未查询到该实验室的主管老师
            if (null == headTeacher)
            {
                MessageBox.Show("未查询到主管老师的相关信息，请通知该主管老师注册本系统或联系系统管理员。", "错误的用户信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            //如果添加物资窗口尚未实例化
            if (null == addStuffWindow)
            {
                //创建并初始化添加物资窗口
                addStuffWindow = new AddStuffWindow(this, headTeacher, user);
            }
            //显示添加物资窗口
            addStuffWindow.Show();
        }

        /// <summary>
        /// 根据操作类型更新数据网格视图
        /// </summary>
        /// <param name="Operation">操作类型</param>
        /// <param name="Stuff">物资对象</param>
        /// <param name="State">物资状态</param>
        /// <param name="CurrentHolder">物资的当前持有者</param>
        public void UpdateDataGrid(string Operation, Stuff Stuff, string State, string CurrentHolder)
        {
            //获取数据网格当前选中行的行号
            int selectedIndex = dgrdStuffInfo.SelectedIndex;

            //根据操作类型更新数据网格视图
            switch (Operation)
            {
                case "Borrow":
                    //更新指定行第三列（物资状态）
                    (dgrdStuffInfo.Columns[3].GetCellContent(dgrdStuffInfo.Items[selectedIndex]) as TextBlock).Text = State;

                    //更新指定行第五列（当前持有者）
                    (dgrdStuffInfo.Columns[5].GetCellContent(dgrdStuffInfo.Items[selectedIndex]) as TextBlock).Text = CurrentHolder;
                    break;
                case "Return":
                case "Remove":
                    //获取数据网格物资对象表
                    IList<Stuff> stuffs = dgrdStuffInfo.ItemsSource as List<Stuff>;

                    //删除指定行
                    stuffs.RemoveAt(selectedIndex);

                    //更新数据网格的数据源
                    dgrdStuffInfo.ItemsSource = null;
                    dgrdStuffInfo.ItemsSource = stuffs;
                    break;
                case "Add":
                    //获取数据网格物资对象表
                    IList<Stuff> stuffsOfMine = dgrdStuffInfo.ItemsSource as List<Stuff>;

                    //获取数据库为新行分配的Id号
                    Stuff.StuffId = StuffDAO.QueryMaxOfStuffId();

                    //添加新对象到对象表
                    stuffsOfMine.Add(Stuff);

                    //更新数据网格的数据源
                    dgrdStuffInfo.ItemsSource = null;
                    dgrdStuffInfo.ItemsSource = stuffsOfMine;
                    break;
            }
        }

        /// <summary>
        /// 搜索按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //如果搜索输入框为空
            if (txtSearchInput.Text.Equals(""))
            {
                MessageBox.Show("搜索文本不能为空！", "文本为空", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            //获取数据网格物资对象表
            IList<Stuff> stuffs = dgrdStuffInfo.ItemsSource as List<Stuff>;

            //获取搜索输入框内容
            string input = txtSearchInput.Text;

            //创建搜索结果列表
            IList<Stuff> searchResult = new List<Stuff>();

            //搜索各属性包含（‘或’逻辑）搜索关键字的物资对象
            foreach (Stuff stuff in stuffs)
            {
                if (stuff.StuffName.Contains(input) || stuff.State.Equals(input) || stuff.Owner.Contains(input) || stuff.CurrentHolder.Contains(input))
                    searchResult.Add(stuff);
            }

            //更新数据网格的数据源
            dgrdStuffInfo.ItemsSource = null;
            dgrdStuffInfo.ItemsSource = searchResult;

            MessageBox.Show("共搜索到 " + searchResult.Count + " 条与 " + input + " 相关的物资信息", searchResult.Count + " 条搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 窗口关闭事件的回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, System.EventArgs e)
        {
            //如果各子窗口实例存在，关掉这些子窗口
            if (null != borrowWindow)
                borrowWindow.Close();

            if (null != returnWindow)
                returnWindow.Close();

            if (null != addStuffWindow)
                addStuffWindow.Close();

            if (null != removeStuffWindow)
                removeStuffWindow.Close();
        }
    }
}
