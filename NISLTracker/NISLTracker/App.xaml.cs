// ************************************************************************************
//
// 文件名(File Name):            App.xaml.cs
//
// 数据表(Tables):               Users : User 用户表 可读可写
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

namespace NISLTracker
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 用作缓存的全局用户表
        /// </summary>
        public static IList<User> Users;

        /// <summary>
        /// 应用启动事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MessageBox.Show("正在获取数据，请稍候。", "请稍候", MessageBoxButton.OK, MessageBoxImage.Information);

            //查表以获取缓存
            Users = UserDAO.QueryAll();
        }

        /// <summary>
        /// 向缓存中插入用户对象
        /// </summary>
        /// <param name="User">用户对象</param>
        public static void InsertUser(User User)
        {
            Users.Add(User);
        }

        /// <summary>
        /// 按用户名更新缓存中用户的授权码和安全戳
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="AuthCode">授权码密文</param>
        /// <param name="SecStamp">安全戳</param>
        public static void UpdateAuthCodeAndSecStampByUserName(string UserName, string AuthCode, string SecStamp)
        {
            foreach (User user in Users)
            {
                if (user.UserName.Equals(UserName))
                {
                    user.AuthorizationCode = AuthCode;
                    user.SecurityStamp = SecStamp;
                }
            }
        }

        /// <summary>
        /// 按用户名更新缓存中用户的身份
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Identity">身份</param>
        public static void UpdateIdentityByUserName(string UserName, string Identity)
        {
            foreach (User user in Users)
            {
                if (user.UserName.Equals(UserName))
                {
                    user.Identity = Identity;
                }
            }
        }

        /// <summary>
        /// 通过用户名获取缓存中的用户对象
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns>查表得到的用户对象</returns>
        public static User GetUserByUserName(string UserName)
        {
            foreach (User user in Users)
            {
                if (user.UserName.Equals(UserName))
                    return user;
            }
            return null;
        }

        /// <summary>
        /// 通过实验室号获取缓存中的主管老师用户对象
        /// </summary>
        /// <param name="Laboratory">实验室号</param>
        /// <returns>查表得到的主管老师用户对象</returns>
        public static User GetTeacherByLaboratory(int Laboratory)
        {
            foreach (User user in Users)
            {
                if (user.Identity.Equals("Teacher") && user.Laboratory == Laboratory)
                    return user;
            }
            return null;
        }

        /// <summary>
        /// 获得缓存中的系统管理员用户对象
        /// </summary>
        /// <returns>查表得到的系统管理员用户对象</returns>
        public static User GetManager()
        {
            foreach (User user in Users)
            {
                if (user.Identity.Equals("Manager"))
                    return user;
            }
            return null;
        }
    }
}
