using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace NISLTracker
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static IList<User> Users;
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MessageBox.Show("正在获取数据，请稍候。", "请稍候", MessageBoxButton.OK, MessageBoxImage.Information);
            Users = UserDAO.QueryAll();
        }

        public static User GetUserByUserName(string UserName)
        {
            foreach(User user in Users)
            {
                if (user.UserName.Equals(UserName))
                    return user;
            }
            return null;
        }

        public static User GetTeacherByLaboratory(int Laboratory)
        {
            foreach (User user in Users)
            {
                if (user.Identity.Equals("Teacher") && user.Laboratory == Laboratory)
                    return user;
            }
            return null;
        }
    }
}
