using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AddStuffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddStuffWindow : Window
    {
        private DataWindow parentWindow;
        private User headTeacher;
        private User user;

        public AddStuffWindow(DataWindow ParentWindow, User HeadTeacher, User User)
        {
            parentWindow = ParentWindow;
            headTeacher = HeadTeacher;
            user = User;
            InitializeComponent();
            txtOwner.Text = user.UserName;
        }

        private void btnAddStuff_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"^\d+$");
            string ciphertext = Encrypt.GetCiphertext(txtHeadTeacherAuthorizationCode.Password, headTeacher.SecurityStamp);
            if (!txtStuffName.Text.Equals("") && regex.IsMatch(txtValueOfAssessment.Text) && ciphertext.Equals(headTeacher.AuthorizationCode))
            {
                Stuff stuff = new Stuff()
                {
                    StuffName = txtStuffName.Text,
                    ValueOfAssessment = Int32.Parse(txtValueOfAssessment.Text),
                    State = "Holding",
                    Owner = user.UserName,
                    CurrentHolder = user.UserName
                };

                int result = StuffDAO.InsertStuff(stuff);
                if (result == 1)
                {
                    MessageBox.Show("物资添加成功！", "添加成功", MessageBoxButton.OK, MessageBoxImage.None);
                    Close();
                    parentWindow.UpdateDataGrid("Add", stuff, null, null);
                }
                else
                {
                    MessageBox.Show("物资添加失败，请稍后重试。", "添加失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("输入有误，请检查后重试。", "输入有误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtStuffName);
        }
    }
}
