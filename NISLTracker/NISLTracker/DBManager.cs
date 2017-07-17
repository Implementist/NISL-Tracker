// ************************************************************************************
//
// 文件名(File Name):            DBManager.cs
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

namespace NISLTracker
{
    class DBManager
    {
        /// <summary>
        /// 远程数据库连接字符串
        /// </summary>
        private const string CONNECTION_STRING = "Host=47.94.200.146;UserId=root;Password=;Database=nisl_tracker;Charset=utf8";

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>已打开的数据库连接</returns>
        public static MySqlConnection GetConnection()
        {
            //获取MySql数据库连接
            MySqlConnection connection = new MySqlConnection(CONNECTION_STRING);

            //打开数据库连接
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 关闭所有数据库相关对象
        /// </summary>
        /// <param name="dataReader">数据阅读器对象</param>
        /// <param name="command">数据库命令对象</param>
        /// <param name="connection">数据库连接对象</param>
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
