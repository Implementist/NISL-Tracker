// ************************************************************************************
//
// 文件名(File Name):            UserDAO.cs
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

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace NISLTracker
{
    abstract class UserDAO
    {
        /// <summary>
        /// 向数据库中插入用户
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>插入结果</returns>
        public static int InsertUser(User user)
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("INSERT INTO user VALUES ");
                sqlExpression.Append("(@UserName,@AuthorizationCode,@SecurityStamp,");
                sqlExpression.Append("@Identity,@Laboratory)");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@AuthorizationCode", user.AuthorizationCode);
                command.Parameters.AddWithValue("@SecurityStamp", user.SecurityStamp);
                command.Parameters.AddWithValue("@Identity", user.Identity);
                command.Parameters.AddWithValue("@Laboratory", user.Laboratory);

                //执行数据库命令并返回插入结果
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //出现错误返回-1
                return -1;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(null, command, connection);
            }
        }

        /// <summary>
        /// 按用户名更新用户的授权码和安全戳
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="AuthCode">授权码密文</param>
        /// <param name="SecStamp">安全戳</param>
        /// <returns></returns>
        public static int UpdateAuthCodeAndSecStampByUserName(string UserName, string AuthCode, string SecStamp)
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("UPDATE user SET ");
                sqlExpression.Append("AuthorizationCode=@AuthorizationCode,");
                sqlExpression.Append("SecurityStamp=@SecurityStamp ");
                sqlExpression.Append("WHERE UserName=@UserName");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@AuthorizationCode", AuthCode);
                command.Parameters.AddWithValue("@SecurityStamp", SecStamp);
                command.Parameters.AddWithValue("@UserName", UserName);

                //执行数据库命令并返回更新结果
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //出现错误返回-1
                return -1;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(null, command, connection);
            }
        }

        /// <summary>
        /// 按用户名更新用户身份
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Identity">用户身份</param>
        /// <returns>更新结果</returns>
        public static int UpdateIdentityByUserName(string UserName, string Identity)
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("UPDATE user SET ");
                sqlExpression.Append("Identity=@Identity ");
                sqlExpression.Append("WHERE UserName=@UserName");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@Identity", Identity);
                command.Parameters.AddWithValue("@UserName", UserName);

                //执行数据库命令并返回更新结果
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //出现错误返回-1
                return -1;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(null, command, connection);
            }
        }

        /// <summary>
        /// 查询所有用户
        /// </summary>
        /// <returns>用户列表</returns>
        public static IList<User> QueryAll()
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            //创建并初始化MySql数据阅读器
            MySqlDataReader reader = null;

            //用户列表
            IList<User> users = new List<User>();

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM user");

                command = new MySqlCommand(sqlExpression.ToString(), connection);

                //执行数据库命令并接收查询结果
                reader = command.ExecuteReader();

                //若未读取完所有的查询结果条目
                while (reader.Read())
                {
                    //构造一个对应于当前条目的用户对象
                    User user = new User()
                    {
                        UserName = reader.GetString("UserName"),
                        AuthorizationCode = reader.GetString("AuthorizationCode"),
                        SecurityStamp = reader.GetString("SecurityStamp"),
                        Identity = reader.GetString("Identity"),
                        Laboratory = reader.GetInt32("Laboratory")
                    };

                    //向用户列表中添加用户对象
                    users.Add(user);
                }

                //返回用户列表
                return users;
            }
            catch (Exception)
            {
                //出错返回空的结果列表，不是null但users.Count等于0
                return users;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(reader, command, connection);
            }
        }
    }
}
