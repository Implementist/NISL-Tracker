using System;
using System.Collections.Generic;
using System.Data;
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
    /// DataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataWindow : Window
    {
        private User user;

        public DataWindow(User User)
        {
            user = User;
            InitializeComponent();
        }

        private void rdbAllStuffs_Checked(object sender, RoutedEventArgs e)
        {
            List<Stuff> stuffs = StuffDAO.QueryAllStuffs();
            dgrdStuffInfo.ItemsSource = stuffs;
            imgAddStuff.Visibility = Visibility.Hidden;
        }

        private void rdbMyStuffs_Checked(object sender, RoutedEventArgs e)
        {
            List<Stuff> stuffs = StuffDAO.QueryStuffByOwner(user.UserName);
            dgrdStuffInfo.ItemsSource = stuffs;
            imgAddStuff.Visibility = Visibility.Visible;
        }

        private void rdbBorrowedStuffs_Checked(object sender, RoutedEventArgs e)
        {
            List<Stuff> stuffs = StuffDAO.QueryStuffByCurrentHolder(user.UserName);
            dgrdStuffInfo.ItemsSource = stuffs;
            imgAddStuff.Visibility = Visibility.Hidden;
        }

        private void dgrdStuffInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = dgrdStuffInfo.SelectedIndex;

            //判断选中行的行号，避免切换绑定资源时引发的异常
            if (selectedIndex > -1)
            {
                Stuff stuff = dgrdStuffInfo.SelectedItem as Stuff;

                //如果物资处于可借状态——即： 1.用户处于所有物资界面；
                //2.该物资当前被拥有者持有； 3.该物资的拥有者不是当前用户。
                if ((bool)rdbAllStuffs.IsChecked && stuff.State.Equals("Holding") && !stuff.Owner.Equals(user.UserName))
                {
                    User owner = App.GetUserByUserName(stuff.Owner);
                    BorrowWindow borrowWindow = new BorrowWindow(this, stuff, owner, user);
                    borrowWindow.Show();
                }

                //如果物资处于可还状态——即： 1.用户处于借入物资界面；
                //2.该物资当前被借出； 3.该物资的当前持有者是当前用户。
                else if ((bool)rdbBorrowedStuffs.IsChecked && stuff.State.Equals("LentOut") && stuff.CurrentHolder.Equals(user.UserName))
                {
                    User owner = App.GetUserByUserName(stuff.Owner);
                    ReturnWindow returnWindow = new ReturnWindow(this, stuff, owner, user);
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

                    User headTeacher = App.GetTeacherByLaboratory(user.Laboratory);

                    //如果未查询到该实验室的主管老师
                    if (null == headTeacher)
                    {
                        MessageBox.Show("未查询到主管老师的相关信息，请通知该主管老师注册本系统或联系系统管理员。", "错误的用户信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        return;
                    }

                    RemoveStuffWindow removeStuffWindow = new RemoveStuffWindow(this, stuff, headTeacher, user);
                    removeStuffWindow.Show();
                }
            }
        }

        private void imgAddStuff_MouseUp(object sender, MouseButtonEventArgs e)
        {
            User headTeacher = App.GetTeacherByLaboratory(user.Laboratory);
            if (null == headTeacher)
            {
                MessageBox.Show("未查询到主管老师的相关信息，请通知该主管老师注册本系统或联系系统管理员。", "错误的用户信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            AddStuffWindow addStuffWindow = new AddStuffWindow(this, headTeacher, user);
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
            int selectedIndex = dgrdStuffInfo.SelectedIndex;
            switch (Operation)
            {
                case "Borrow":
                    (dgrdStuffInfo.Columns[3].GetCellContent(dgrdStuffInfo.Items[selectedIndex]) as TextBlock).Text = State;
                    (dgrdStuffInfo.Columns[5].GetCellContent(dgrdStuffInfo.Items[selectedIndex]) as TextBlock).Text = CurrentHolder;
                    break;
                case "Return":
                case "Remove":
                    IList<Stuff> stuffs = dgrdStuffInfo.ItemsSource as List<Stuff>;
                    stuffs.RemoveAt(selectedIndex);
                    dgrdStuffInfo.ItemsSource = null;
                    dgrdStuffInfo.ItemsSource = stuffs;
                    break;
                case "Add":
                    IList<Stuff> stuffsOfMine = dgrdStuffInfo.ItemsSource as List<Stuff>;
                    Stuff.StuffId = StuffDAO.QueryMaxOfStuffId();
                    stuffsOfMine.Add(Stuff);
                    dgrdStuffInfo.ItemsSource = null;
                    dgrdStuffInfo.ItemsSource = stuffsOfMine;
                    break;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearchInput.Text.Equals(""))
            {
                MessageBox.Show("搜索文本不能为空！", "文本为空", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            IList<Stuff> stuffs = dgrdStuffInfo.ItemsSource as List<Stuff>;
            string input = txtSearchInput.Text;
            IList<Stuff> searchResult = new List<Stuff>();

            foreach(Stuff stuff in stuffs)
            {
                if (stuff.StuffName.Contains(input) || stuff.State.Equals(input) || stuff.Owner.Contains(input) || stuff.CurrentHolder.Contains(input))
                    searchResult.Add(stuff);
            }

            dgrdStuffInfo.ItemsSource = null;
            dgrdStuffInfo.ItemsSource = searchResult;

            MessageBox.Show("共搜索到 " + searchResult.Count + " 条与 " + input + " 相关的物资信息", searchResult.Count + " 条搜索结果", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
