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
        }

        private void rdbMyStuffs_Checked(object sender, RoutedEventArgs e)
        {
            List<Stuff> stuffs = StuffDAO.QueryStuffByOwner(user.UserName);
            dgrdStuffInfo.ItemsSource = stuffs;
        }

        private void rdbBorrowedStuffs_Checked(object sender, RoutedEventArgs e)
        {
            List<Stuff> stuffs = StuffDAO.QueryStuffByCurrentHolder(user.UserName);
            dgrdStuffInfo.ItemsSource = stuffs;
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
                    User user = UserDAO.QueryUserByUserName(stuff.Owner);
                    BorrowWindow borrowWindow = new BorrowWindow(this, stuff, user, this.user);
                    borrowWindow.Show();
                }
                else if ((bool)rdbBorrowedStuffs.IsChecked && stuff.State.Equals("LentOut") && stuff.CurrentHolder.Equals(user.UserName))
                {
                    User user = UserDAO.QueryUserByUserName(stuff.Owner);
                    ReturnWindow returnWindow = new ReturnWindow(this, stuff, user, this.user);
                    returnWindow.Show();
                }

            }
        }

        public void UpdateDataGrid(string operation, string State, string CurrentHolder)
        {
            int selectedIndex = dgrdStuffInfo.SelectedIndex;
            if (operation.Equals("Borrow"))
            {
                (dgrdStuffInfo.Columns[3].GetCellContent(dgrdStuffInfo.Items[selectedIndex]) as TextBlock).Text = State;
                (dgrdStuffInfo.Columns[5].GetCellContent(dgrdStuffInfo.Items[selectedIndex]) as TextBlock).Text = CurrentHolder;
            }
            else
            {
                List<Stuff> stuffs = dgrdStuffInfo.ItemsSource as List<Stuff>;
                stuffs.RemoveAt(selectedIndex);
                dgrdStuffInfo.ItemsSource = null;
                dgrdStuffInfo.ItemsSource = stuffs;
            }
        }
    }
}
