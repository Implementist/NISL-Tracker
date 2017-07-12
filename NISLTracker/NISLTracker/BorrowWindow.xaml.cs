using System;
using System.Collections.Generic;
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
    /// BorrowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BorrowWindow : Window
    {
        private DataWindow parentWindow; 
        private Stuff stuff;
        private User owner;
        private User user;

        public BorrowWindow(DataWindow ParentWindow, Stuff Stuff, User Owner, User User)
        {
            parentWindow = ParentWindow;
            stuff = Stuff;
            owner = Owner;
            user = User;
            InitializeComponent();
            txtStuffName.Text = stuff.StuffName;
            txtValueOfAssessment.Text = stuff.ValueOfAssessment.ToString();
            txtOwner.Text = stuff.Owner;
        }

        private void btnBorrow_Click(object sender, RoutedEventArgs e)
        {
            string ciphertext = Encrypt.GetCiphertext(txtOwnerAuthorizationCode.Password, owner.SecurityStamp);
            if (ciphertext.Equals(owner.AuthorizationCode))
            {
                int result = StuffDAO.UpdateStateAndCurrentHolderByStuffId(stuff.StuffId, "LentOut", user.UserName);
                if (result == 1)
                {
                    MessageBox.Show("物资借入成功！", "借入成功", MessageBoxButton.OK, MessageBoxImage.None);
                    Close();
                    parentWindow.UpdateDataGrid("Borrow", "LentOut", user.UserName);
                }
                else
                { 
                    MessageBox.Show("物资借入失败，请稍后重试。", "借入失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("验证失败，请检查您的授权码是否正确并重试。", "验证失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtOwnerAuthorizationCode);
        }
    }
}
