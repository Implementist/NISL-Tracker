using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NISLTracker
{
    abstract class StuffDAO
    {
        public static int UpdateStateAndCurrentHolderByStuffId(int StuffId, string State, string CurrentHolder)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("UPDATE stuff SET ");
                sqlExpression.Append("State=@State,CurrentHolder=@CurrentHolder ");
                sqlExpression.Append("WHERE StuffId=@StuffId");

                command = new MySqlCommand(sqlExpression.ToString(), connection);
                command.Parameters.AddWithValue("@State", State);
                command.Parameters.AddWithValue("@CurrentHolder", CurrentHolder);
                command.Parameters.AddWithValue("@StuffId", StuffId);

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

        public static List<Stuff> QueryAllStuffs()
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            List<Stuff> stuffs = new List<Stuff>();
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff");

                command = new MySqlCommand(sqlExpression.ToString(), connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Stuff stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };
                    stuffs.Add(stuff);
                }
                return stuffs;
            }
            catch (Exception)
            {
                return stuffs;
            }
            finally
            {
                DBManager.CloseAll(reader, command, connection);
            }
        }

        public static List<Stuff> QueryStuffByOwner(string Owner)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            List<Stuff> stuffs = new List<Stuff>();
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff WHERE ");
                sqlExpression.Append("Owner=@Owner");

                command = new MySqlCommand(sqlExpression.ToString(), connection); command.Parameters.AddWithValue("@Owner", Owner);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Stuff stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };
                    stuffs.Add(stuff);
                }
                return stuffs;
            }
            catch (Exception)
            {
                return stuffs;
            }
            finally
            {
                DBManager.CloseAll(reader, command, connection);
            }
        }

        public static List<Stuff> QueryStuffByCurrentHolder(string CurrentHolder)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            List<Stuff> stuffs = new List<Stuff>();
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff WHERE ");
                sqlExpression.Append("CurrentHolder=@CurrentHolder AND Owner<>@Owner");

                command = new MySqlCommand(sqlExpression.ToString(), connection); command.Parameters.AddWithValue("@CurrentHolder", CurrentHolder);
                command.Parameters.AddWithValue("@Owner", CurrentHolder);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Stuff stuff = new Stuff()
                    {
                        StuffId = reader.GetInt32("StuffId"),
                        StuffName = reader.GetString("StuffName"),
                        ValueOfAssessment = reader.GetInt32("ValueOfAssessment"),
                        State = reader.GetString("State"),
                        Owner = reader.GetString("Owner"),
                        CurrentHolder = reader.GetString("CurrentHolder")
                    };
                    stuffs.Add(stuff);
                }
                return stuffs;
            }
            catch (Exception)
            {
                return stuffs;
            }
            finally
            {
                DBManager.CloseAll(reader, command, connection);
            }
        }

        public static Stuff QueryStuffByStuffId(int StuffId)
        {
            MySqlConnection connection = null;
            MySqlCommand command = null;
            MySqlDataReader reader = null;
            Stuff stuff = null;
            try
            {
                connection = DBManager.GetConnection();

                StringBuilder sqlExpression = new StringBuilder();
                sqlExpression.Append("SELECT * FROM stuff WHERE ");
                sqlExpression.Append("StuffId=@StuffId");

                command = new MySqlCommand(sqlExpression.ToString(), connection); command.Parameters.AddWithValue("@StuffId", StuffId);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
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
                return stuff;
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
    }
}
