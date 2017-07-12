using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NISLTracker
{
    class DBManager
    {
        private const string CONNECTION_STRING = "Host=47.94.200.146;UserId=root;Password=;Database=nisl_tracker";

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>已打开的数据库连接</returns>
        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(CONNECTION_STRING);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 关闭所有数据库相关对象
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        public static void CloseAll(MySqlDataReader dataReader, MySqlCommand command, MySqlConnection connection)
        {
            if (null != dataReader)
                dataReader.Close();
            if (null != command)
                command.Cancel();
            if (null != connection)
                connection.Close();
        }
    }
}
