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

                if ((bool)rdbAllStuffs.IsChecked && stuff.State.Equals("Holding") && !stuff.Owner.Equals(user.UserName))
                {
                    User owner = App.GetUserByUserName(stuff.Owner);
                    BorrowWindow borrowWindow = new BorrowWindow(this, stuff, owner, user);
                    borrowWindow.Show();
                }
                else if ((bool)rdbBorrowedStuffs.IsChecked && stuff.State.Equals("LentOut") && stuff.CurrentHolder.Equals(user.UserName))
                {
                    User owner = App.GetUserByUserName(stuff.Owner);
                    ReturnWindow returnWindow = new ReturnWindow(this, stuff, owner, user);
                    returnWindow.Show();
                }
                else if ((bool)rdbMyStuffs.IsChecked && stuff.Owner.Equals(user.UserName))
                {
                    if (stuff.State.Equals("Holding"))
                    {
                        User headTeacher = App.GetTeacherByLaboratory(user.Laboratory);
                        if (null != headTeacher)
                        {
                            RemoveStuffWindow removeStuffWindow = new RemoveStuffWindow(this, stuff, headTeacher, user);
                            removeStuffWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show("未查询到主管老师的相关信息，请通知该主管老师注册本系统或联系系统管理员。", "错误的用户信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                    }
                    else
                    {
                        MessageBox.Show("该物资当前处于借出状态，请先执行归还流程。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
        }

        private void imgAddStuff_MouseUp(object sender, MouseButtonEventArgs e)
        {
            User headTeacher = App.GetTeacherByLaboratory(user.Laboratory);
            if (null != headTeacher)
            {
                AddStuffWindow addStuffWindow = new AddStuffWindow(this, headTeacher, user);
                addStuffWindow.Show();
            }
            else
            {
                MessageBox.Show("未查询到主管老师的相关信息，请通知该主管老师注册本系统或联系系统管理员。", "错误的用户信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
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
                    List<Stuff> stuffs = dgrdStuffInfo.ItemsSource as List<Stuff>;
                    stuffs.RemoveAt(selectedIndex);
                    dgrdStuffInfo.ItemsSource = null;
                    dgrdStuffInfo.ItemsSource = stuffs;
                    break;
                case "Add":
                    List<Stuff> stuffsOfMine = dgrdStuffInfo.ItemsSource as List<Stuff>;
                    Stuff.StuffId = StuffDAO.QueryMaxOfStuffId();
                    stuffsOfMine.Add(Stuff);
                    dgrdStuffInfo.ItemsSource = null;
                    dgrdStuffInfo.ItemsSource = stuffsOfMine;
                    break;
            }
        }
    }
}
