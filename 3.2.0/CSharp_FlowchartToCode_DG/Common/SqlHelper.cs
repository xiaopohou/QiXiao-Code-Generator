using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
namespace CSharp_FlowchartToCode_DG
{
    public class SqlHelper
    {

        #region ExcuteNonQuery 执行sql语句或者存储过程,返回影响的行数---ExcuteNonQuery
        /// <summary>
        /// 执行sql语句或存储过程，返回受影响的行数,不带参数。
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 有默认值CommandType.Text</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string ConnString,string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType);//参数增加了commandType 可以自己编辑执行方式
                    return cmd.ExecuteNonQuery();
                }
            }

        }
        /// <summary>
        /// 执行sql语句或存储过程，返回受影响的行数。
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 t</param>
        /// <param name="parms">SqlParameter[]参数数组，允许空</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string ConnString,string commandTextOrSpName, CommandType commandType, params SqlParameter[] parms)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType, parms);//参数增加了commandType 可以自己编辑执行方式
                    return cmd.ExecuteNonQuery();
                }
            }

        }
        /// <summary>
        /// 执行sql命令，返回受影响的行数。
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="obj">object[]参数数组，允许空</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string ConnString,string commandTextOrSpName, CommandType commandType, params object[] obj)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType, obj);//参数增加了commandType 可以自己编辑执行方式
                    return cmd.ExecuteNonQuery();
                }
            }

        }
        #endregion

        #region ExecuteScalar 执行sql语句或者存储过程,执行单条语句，返回自增的id---ScalarExecuteScalar
        /// <summary>
        /// 执行sql语句或存储过程 返回ExecuteScalar （返回自增的ID）不带参数
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 有默认值CommandType.Text</param>
        /// <returns></returns>
        public static object ExecuteScalar(string ConnString,string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType);
                    return cmd.ExecuteScalar();
                }

            }
        }
        /// <summary>
        /// 执行sql语句或存储过程 返回ExecuteScalar （返回自增的ID）
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parms">SqlParameter[]参数数组，允许空</param>
        /// <returns></returns>
        public static object ExecuteScalar(string ConnString,string commandTextOrSpName, CommandType commandType, params SqlParameter[] parms)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType, parms);
                    return cmd.ExecuteScalar();
                }

            }
        }
        /// <summary>
        /// 执行sql语句或存储过程 返回ExecuteScalar （返回自增的ID）
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="obj">object[]参数数组，允许空</param>
        /// <returns></returns>
        public static object ExecuteScalar(string ConnString,string commandTextOrSpName, CommandType commandType, params object[] obj)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType, obj);
                    return cmd.ExecuteScalar();
                }
            }
        }
        #endregion

        #region ExecuteReader 执行sql语句或者存储过程,返回DataReader---DaataReader
        /// <summary>
        /// 执行sql语句或存储过程 返回DataReader 不带参数
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 有默认值CommandType.Text</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string ConnString,string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            //sqlDataReader不能用using 会关闭conn 导致不能获取到返回值。注意：DataReader获取值时必须保持连接状态
            try
            {
                SqlConnection conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand();
                PreparCommand(conn, cmd, commandTextOrSpName, commandType);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 执行sql语句或存储过程 返回DataReader
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parms">SqlParameter[]参数数组，允许空</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string ConnString,string commandTextOrSpName, CommandType commandType, params SqlParameter[] parms)
        {
            //sqlDataReader不能用using 会关闭conn 导致不能获取到返回值。注意：DataReader获取值时必须保持连接状态
            try
            {
                SqlConnection conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand();
                PreparCommand(conn, cmd, commandTextOrSpName, commandType, parms);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 执行sql语句或存储过程 返回DataReader
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="obj">object[]参数数组，允许空</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string ConnString,string commandTextOrSpName, CommandType commandType, params object[] obj)
        {
            //sqlDataReader不能用using 会关闭conn 导致不能获取到返回值。注意：DataReader获取值时必须保持连接状态
            try
            {
                SqlConnection conn = new SqlConnection(ConnString);
                SqlCommand cmd = new SqlCommand();
                PreparCommand(conn, cmd, commandTextOrSpName, commandType, obj);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region ExecuteDataset 执行sql语句或者存储过程,返回一个DataSet---DataSet
        /// <summary>
        /// 执行sql语句或存储过程，返回DataSet 不带参数
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 有默认值CommandType.Text</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string ConnString,string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
        }
        /// <summary>
        /// 执行sql语句或存储过程，返回DataSet
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parms">SqlParameter[]参数数组，允许空</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string ConnString,string commandTextOrSpName, CommandType commandType, params SqlParameter[] parms)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType, parms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
        }
        /// <summary>
        /// 执行sql语句或存储过程，返回DataSet
        /// </summary>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 </param>
        /// <param name="obj">object[]参数数组，允许空</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string ConnString,string commandTextOrSpName, CommandType commandType, params object[] obj)
        {
            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PreparCommand(conn, cmd, commandTextOrSpName, commandType, obj);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
        }
        #endregion

        #region ---PreparCommand 构建一个通用的command对象供内部方法进行调用---
        /// <summary>
        /// 不带参数的设置sqlcommand对象
        /// </summary>
        /// <param name="conn">sqlconnection对象</param>
        /// <param name="cmd">sqlcommmand对象</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">语句的类型</param>
        private static void PreparCommand(SqlConnection conn, SqlCommand cmd, string commandTextOrSpName, CommandType commandType)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 20;
        }
        /// <summary>
        /// 设置一个等待执行的SqlCommand对象
        /// </summary>
        /// <param name="conn">sqlconnection对象</param>
        /// <param name="cmd">sqlcommmand对象</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">语句的类型</param>
        /// <param name="parms">参数，sqlparameter类型，需要指出所有的参数名称</param>
        private static void PreparCommand(SqlConnection conn, SqlCommand cmd, string commandTextOrSpName, CommandType commandType, params SqlParameter[] parms)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 20;

            cmd.Parameters.Clear();
            if (parms != null)
            {
                cmd.Parameters.AddRange(parms);
            }
        }
        /// <summary>
        /// PreparCommand方法，可变参数为object需要严格按照参数顺序传参
        /// </summary>
        /// <param name="conn">sqlconnection对象</param>
        /// <param name="cmd">sqlcommmand对象</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">语句的类型</param>
        /// <param name="parms">参数，object类型，需要按顺序赋值</param>
        private static void PreparCommand(SqlConnection conn, SqlCommand cmd, string commandTextOrSpName, CommandType commandType, params object[] parms)
        {

            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 20;

            cmd.Parameters.Clear();
            if (parms != null)
            {
                cmd.Parameters.AddRange(parms);
            }
        }
        //之所以会用object参数方法是为了我们能更方便的调用存储过程，不必去关系存储过程参数名是什么，知道它的参数顺序就可以了 sqlparameter必须指定每一个参数名称
        #endregion

    }
}
