using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;

namespace Xuld.Robot
{
    public class LostXmas : Robot
    {

        OleDbConnection conn = new OleDbConnection();

        public LostXmas()
        {
            conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Path.GetFullPath("Robot.mdb");

            conn.Open();
        }

        string QueryDatabase(string message, string who)
        {
            using (OleDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Answer FROM Consts WHERE Message=?";
                cmd.Parameters.AddWithValue("@Message", message);
                string data = (string)cmd.ExecuteScalar();
                if (data != null)
                {
                    return data.Replace("[我]", who);
                }

                cmd.CommandText = "SELECT Answer FROM Keywords WHERE InStr(@Message, Message) > 0";
                data = (string)cmd.ExecuteScalar();
                if (data != null)
                {
                    return data.Replace("[我]", who);
                }

                return data;
            }
        }

        bool UpdateDataBaseInternal(string message, string who, string table, string splitter)
        {
            if (message.IndexOf(splitter) > 0)
            {
                int i = message.IndexOf(splitter);
                string from = message.Substring(0, i).TrimEnd();
                string to = message.Substring(i + splitter.Length).TrimStart();

                if (String.IsNullOrEmpty(from))
                    return false;

                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    if (String.IsNullOrEmpty(to))
                    {
                        cmd.CommandText = "DELETE * FROM " + table + " WHERE Message=@Message";
                        cmd.Parameters.AddWithValue("@Message", from);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }
                    else
                    {

                        cmd.CommandText = "UPDATE " + table + " SET Answer=@Answer, Teacher=@Teacher, [Time]=@Time WHERE Message=@Message";
                        cmd.Parameters.AddWithValue("@Answer", to);
                        cmd.Parameters.AddWithValue("@Teacher", who);
                        cmd.Parameters.AddWithValue("@Time", DateTime.Now.ToOADate());
                        cmd.Parameters.AddWithValue("@Message", from);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                        cmd.CommandText = "INSERT INTO " + table + "(Answer, Teacher, [Time], Message) VALUES(@Answer, @Teacher, @Time, @Message)";


                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                    }

                }
            }


            return false;
        }

        string TryUpdateDatabase(string message, string who)
        {
            return UpdateDataBaseInternal(message, who, "Consts", "=>") ||
                UpdateDataBaseInternal(message, who, "KeyWords", "->") ? "我学会了  ^-^" : null;
        }

        bool shutdown = false;

        bool xuewo = false;

        string QuerySpecialKeyword(string message, string who)
        {
            if (message.Contains("闭嘴") || message.Contains("安静"))
            {
                shutdown = true;
                return "我不说话了";
            }
            else if (message.Contains("跟嘴"))
            {
                if (message.Contains("不") || message.Contains("don't"))
                {
                    xuewo = false;
                }
                else
                {
                    xuewo = true;
                }
                return "我知道了";
            }
            return null;
        }

        string XueWo(string message, string who)
        {
            return xuewo ? message : null;
        }

        string Compute(string message, string who)
        {
            if (message.StartsWith("="))
            {
                string exp = message.Substring(1);
                try
                {
                    return CorePlus.RunTime.Arithmetic.ComputeExpression(exp).ToString();
                }
                catch
                {

                }
            }


            return null;
        }

        string Guess(string message, string who)
        {


            if (message.EndsWith("？"))
            {
                return "我不知道~";
            }

            if (message.EndsWith("..."))
            {
                return "这个世界很无语";
            }

            if (message.Contains("魂淡") && who != "09")
            {
                return who + "似魂淡";
            }

            if (message == "小三")
            {
                return "大家好，我是小三。可以发送\n问题文字=>回答文字\n教我如何回答。";
            }

            if (message.Contains("！") && message.Contains("我"))
            {
                return message.Replace("我", who);
            }

            return null;
        }

        string ProcessInternal(string message, string who)
        {
            if (shutdown)
            {
                if (message.Contains("小三") || message.Contains("说话"))
                {
                    shutdown = false;
                    return "小三来了";
                }
                return null;
            }
            message = message.Trim();
            return QuerySpecialKeyword(message, who)
                ?? TryUpdateDatabase(message, who)
                ?? QueryDatabase(message, who)
                ?? Compute(message, who)
                ?? Guess(message, who)
                ?? XueWo(message, who);
        }

        public override string Process(string message, string who)
        {
            return ProcessInternal(message, who);
        }

    }
}
