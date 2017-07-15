using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NISLTracker
{
    abstract class UserDAO
    {
        /// <summary>
        /// 向数据库中插入
        /// </summary>
        /// <param name="user">用户数据对象</param>
        /// <returns></returns>
        public static int InsertUser(User user)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("INSERT INTO user VALUES ");
                sqlExpression.Append("(@UserName,@AuthorizationCode,@SecurityStamp,");
                sqlExpression.Append("@Identity,@Laboratory)");

                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@AuthorizationCode", user.AuthorizationCode);
                command.Parameters.AddWithValue("@SecurityStamp", user.SecurityStamp);
                command.Parameters.AddWithValue("@Identity", user.Identity);
                command.Parameters.AddWithValue("@Laboratory", user.Laboratory);

                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                DBManager.CloseAll(null, command, connection);
            }
        }

        public static int UpdateAuthCodeAndSecStampByUserName(string UserName, string AuthCode, string SecStamp)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("UPDATE user SET ");
                sqlExpression.Append("AuthorizationCode=@AuthorizationCode,SecurityStamp=@SecurityStamp ");
                sqlExpression.Append("WHERE UserName=@UserName");

                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@AuthorizationCode", AuthCode);
                command.Parameters.AddWithValue("@SecurityStamp", SecStamp);
                command.Parameters.AddWithValue("@UserName", UserName);

                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                DBManager.CloseAll(null, command, connection);
            }
        }

        public static int UpdateIdentityByUserName(string UserName, string Identity)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("UPDATE user SET ");
                sqlExpression.Append("Identity=@Identity ");
                sqlExpression.Append("WHERE UserName=@UserName");

                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@Identity", Identity);
                command.Parameters.AddWithValue("@UserName", UserName);

                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                DBManager.CloseAll(null, command, connection);
            }
        }

        /// <summary>
        /// 通过用户名查询用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>封装好的用户对象</returns>
        public static User QueryUserByUserName(string userName)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            User user = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM user WHERE ");
                sqlExpression.Append("UserName=@UserName");

                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@UserName", userName);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User()
                    {
                        UserName = reader.GetString("UserName"),
                        AuthorizationCode = reader.GetString("AuthorizationCode"),
                        SecurityStamp = reader.GetString("SecurityStamp"),
                        Identity = reader.GetString("Identity"),
                        Laboratory = reader.GetInt32("Laboratory")
                    };
                }
                return user;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                DBManager.CloseAll(reader, command, connection);
            }
        }

        public static User QueryTeacherByLaboratory(int Laboratory)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            User user = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM user WHERE ");
                sqlExpression.Append("Identity=@Identity AND Laboratory=@Laboratory");

                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@Identity", "Teacher");
                command.Parameters.AddWithValue("@Laboratory", Laboratory);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User()
                    {
                        UserName = reader.GetString("UserName"),
                        AuthorizationCode = reader.GetString("AuthorizationCode"),
                        SecurityStamp = reader.GetString("SecurityStamp"),
                        Identity = reader.GetString("Identity"),
                        Laboratory = reader.GetInt32("Laboratory")
                    };
                }
                return user;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                DBManager.CloseAll(reader, command, connection);
            }
        }

        public static IList<User> QueryAll()
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            IList<User> users = new List<User>();
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM user");

                command = new MySqlCommand(sqlExpression.ToString(), connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User()
                    {
                        UserName = reader.GetString("UserName"),
                        AuthorizationCode = reader.GetString("AuthorizationCode"),
                        SecurityStamp = reader.GetString("SecurityStamp"),
                        Identity = reader.GetString("Identity"),
                        Laboratory = reader.GetInt32("Laboratory")
                    };
                    users.Add(user);
                }
                return users;
            }
            catch (Exception)
            {
                return users;
            }
            finally
            {
                DBManager.CloseAll(reader, command, connection);
            }
        }
    }
}
