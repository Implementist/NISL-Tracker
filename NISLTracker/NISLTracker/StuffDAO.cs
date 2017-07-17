// ************************************************************************************
//
// 文件名(File Name):            StuffDAO.cs
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
    abstract class StuffDAO
    {
        /// <summary>
        /// 向数据库中插入物资
        /// </summary>
        /// <param name="Stuff">物资对象</param>
        /// <returns>插入结果</returns>
        public static int InsertStuff(Stuff Stuff)
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
                sqlExpression.Append("INSERT INTO stuff (StuffName,ValueOfAssessment,");
                sqlExpression.Append("State,Owner,CurrentHolder) VALUES ");
                sqlExpression.Append("(@StuffName,@ValueOfAssessment,@State,");
                sqlExpression.Append("@Owner,@CurrentHolder)");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@StuffName", Stuff.StuffName);
                command.Parameters.AddWithValue("@ValueOfAssessment", Stuff.ValueOfAssessment);
                command.Parameters.AddWithValue("@State", Stuff.State);
                command.Parameters.AddWithValue("@Owner", Stuff.Owner);
                command.Parameters.AddWithValue("@CurrentHolder", Stuff.CurrentHolder);

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
        /// 从数据库中删除物资
        /// </summary>
        /// <param name="StuffId">物资Id号</param>
        /// <returns>删除结果</returns>
        public static int DeleteStuff(int StuffId)
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
                sqlExpression.Append("Delete FROM stuff WHERE ");
                sqlExpression.Append("StuffId=@StuffId");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@StuffId", StuffId);

                //执行数据库命令并返回删除结果
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
        /// 按物资Id号更新物资的状态和当前持有者
        /// </summary>
        /// <param name="StuffId">物资Id号</param>
        /// <param name="State">物资状态</param>
        /// <param name="CurrentHolder">当前持有者</param>
        /// <returns></returns>
        public static int UpdateStateAndCurrentHolderByStuffId(int StuffId, string State, string CurrentHolder)
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
                sqlExpression.Append("UPDATE stuff SET ");
                sqlExpression.Append("State=@State,CurrentHolder=@CurrentHolder ");
                sqlExpression.Append("WHERE StuffId=@StuffId");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@State", State);
                command.Parameters.AddWithValue("@CurrentHolder", CurrentHolder);
                command.Parameters.AddWithValue("@StuffId", StuffId);

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
        /// 查询数据库中所有的物资
        /// </summary>
        /// <returns>所有物资的列表</returns>
        public static List<Stuff> QueryAllStuffs()
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            //创建并初始化MySql数据阅读器
            MySqlDataReader reader = null;

            //创建查询结果列表
            List<Stuff> stuffs = new List<Stuff>();

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff");
                command = new MySqlCommand(sqlExpression.ToString(), connection);

                //执行数据库命令并接收查询结果
                reader = command.ExecuteReader();

                //若未读取完所有的查询结果条目
                while (reader.Read())
                {
                    //构造一个对应于当前条目的物资对象
                    Stuff stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };

                    //向结果列表中添加物资对象
                    stuffs.Add(stuff);
                }

                //返回结果列表
                return stuffs;
            }
            catch (Exception)
            {
                //出错返回空的结果列表，不是null但stuffs.Count等于0
                return stuffs;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(reader, command, connection);
            }
        }

        /// <summary>
        /// 按拥有者查询物资
        /// </summary>
        /// <param name="Owner">拥有者用户名</param>
        /// <returns>查询结果列表</returns>
        public static List<Stuff> QueryStuffByOwner(string Owner)
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            //创建并初始化MySql数据阅读器
            MySqlDataReader reader = null;

            //创建查询结果列表
            List<Stuff> stuffs = new List<Stuff>();

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff WHERE ");
                sqlExpression.Append("Owner=@Owner");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection); command.Parameters.AddWithValue("@Owner", Owner);

                //执行数据库命令并接收查询结果
                reader = command.ExecuteReader();

                //若未读取完所有的查询结果条目
                while (reader.Read())
                {
                    //构造一个对应于当前条目的物资对象
                    Stuff stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };

                    //向结果列表中添加物资对象
                    stuffs.Add(stuff);
                }

                //返回结果列表
                return stuffs;
            }
            catch (Exception)
            {
                //出错返回空的结果列表，不是null但stuffs.Count等于0
                return stuffs;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(reader, command, connection);
            }
        }

        /// <summary>
        /// 按当前持有者查询物资
        /// </summary>
        /// <param name="CurrentHolder">当前持有者用户名</param>
        /// <returns>查询结果列表</returns>
        public static List<Stuff> QueryStuffByCurrentHolder(string CurrentHolder)
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            //创建并初始化MySql数据阅读器
            MySqlDataReader reader = null;

            //创建查询结果列表
            List<Stuff> stuffs = new List<Stuff>();

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff WHERE ");
                sqlExpression.Append("CurrentHolder=@CurrentHolder AND Owner<>@Owner");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection); command.Parameters.AddWithValue("@CurrentHolder", CurrentHolder);
                command.Parameters.AddWithValue("@Owner", CurrentHolder);

                //执行数据库命令并接收查询结果
                reader = command.ExecuteReader();

                //若未读取完所有的查询结果条目
                while (reader.Read())
                {
                    //构造一个对应于当前条目的物资对象
                    Stuff stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };

                    //向结果列表中添加物资对象
                    stuffs.Add(stuff);
                }

                //返回结果列表
                return stuffs;
            }
            catch (Exception)
            {
                //出错返回空的结果列表，不是null但stuffs.Count等于0
                return stuffs;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(reader, command, connection);
            }
        }

        /// <summary>
        /// 按物资Id号查询物资
        /// </summary>
        /// <param name="StuffId"></param>
        /// <returns></returns>
        public static Stuff QueryStuffByStuffId(int StuffId)
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            //创建并初始化MySql数据阅读器
            MySqlDataReader reader = null;

            //创建并初始化物资对象
            Stuff stuff = null;

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff WHERE ");
                sqlExpression.Append("StuffId=@StuffId");

                //ADO.NET安全赋值
                command = new MySqlCommand(sqlExpression.ToString(), connection); command.Parameters.AddWithValue("@StuffId", StuffId);

                //执行数据库命令并接收查询结果
                reader = command.ExecuteReader();

                //如果查到了指定的物资
                if (reader.Read())
                {
                    //构造并填充物资对象
                    stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };
                }

                //返回物资对象
                return stuff;
            }
            catch (Exception)
            {
                //出错返回null
                return null;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(reader, command, connection);
            }
        }

        /// <summary>
        /// 查询数据库为新行指定的物资Id号
        /// </summary>
        /// <returns>数据库为新行指定的物资Id号</returns>
        public static int QueryMaxOfStuffId()
        {
            //创建并初始化MySql数据库连接
            MySqlConnection connection = null;

            //创建并初始化MySql数据库命令
            MySqlCommand command = null;

            //创建并初始化MySql数据阅读器
            MySqlDataReader reader = null;

            //创建物资ID号并初始化为1
            int maxOfStuffId = 1;

            try
            {
                //获取数据库连接对象
                connection = DBManager.GetConnection();

                //构造SQL表达式
                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT MAX(stuff.StuffId) FROM stuff");

                command = new MySqlCommand(sqlExpression.ToString(), connection);

                //执行数据库命令并接收查询结果
                reader = command.ExecuteReader();

                //如果查询到了物资Id号
                if (reader.Read())
                {
                    //为结果变量赋查到的值
                    maxOfStuffId = reader.GetInt32(0);
                }

                //返回物资Id号
                return maxOfStuffId;
            }
            catch (Exception)
            {
                //出错返回1
                return 1;
            }
            finally
            {
                //关闭与数据库相关的所有对象
                DBManager.CloseAll(reader, command, connection);
            }
        }
    }
}
