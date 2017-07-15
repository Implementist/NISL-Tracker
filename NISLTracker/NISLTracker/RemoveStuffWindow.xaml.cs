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
    /// RemoveStuffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemoveStuffWindow : Window
    {
        private DataWindow parentWindow;
        private Stuff stuff;
        private User headTeacher;
        private User user;

        public RemoveStuffWindow(DataWindow ParentWindow, Stuff Stuff, User HeadTeacher, User User)
        {
            parentWindow = ParentWindow;
            stuff = Stuff;
            headTeacher = HeadTeacher;
            user = User;
            InitializeComponent();
            txtStuffName.Text = stuff.StuffName;
            txtValueOfAssessment.Text = stuff.ValueOfAssessment.ToString();
            txtOwner.Text = stuff.Owner;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtHeadTeacherAuthorizationCode);
        }

        private void btnRemoveStuff_Click(object sender, RoutedEventArgs e)
        {
            string ciphertext = Encrypt.GetCiphertext(txtHeadTeacherAuthorizationCode.Password, headTeacher.SecurityStamp);
            if (!ciphertext.Equals(headTeacher.AuthorizationCode))
            {
                MessageBox.Show("验证失败，请检查您的授权码是否正确并重试。", "验证失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            int result = StuffDAO.DeleteStuff(stuff.StuffId);
            if (result == 1)
            {
                MessageBox.Show("物资删除成功！", "删除成功", MessageBoxButton.OK, MessageBoxImage.None);
                Close();
                parentWindow.UpdateDataGrid("Remove", null, null, null);
            }
            else
            {
                MessageBox.Show("物资删除失败，请稍后重试。", "删除失败", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}
