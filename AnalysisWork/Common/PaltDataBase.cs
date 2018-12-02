using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Common
{
    public class PaltDatabase
    {

        public string Err { get; set; }
        // public string ConnStr { get { return "data source=PALTDATA;password=ECG_gate;persist security info=True;user id=ECG_gate"; } set{}}

        private string _connStr;

        public string ConnStr
        {
            get
            {
                if (string.IsNullOrEmpty(_connStr))
                {
                   // _connStr = "data source=HISDB;password=ecg;persist security info=True;user id=ecg";
                    try
                    {
                        _connStr = NetScape.AnalysisModel.Profile.ConfigSetting.PaltConnStr;
                    }
                    catch(Exception ex)
                    {
                        _connStr = "data source=(DESCRIPTION = (ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 168.168.252.119)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = hiup)));password=ecg_gate;persist security info=True;user id=ecg_gate";
                    }
                   finally
                    {
                        if (string.IsNullOrEmpty(_connStr))
                        {
                            _connStr = "data source=(DESCRIPTION = (ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = 168.168.252.119)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = hiup)));password=ecg_gate;persist security info=True;user id=ecg_gate";
                        }
                    }
                }
                return _connStr;
            }
            set { _connStr = value; }
        }


        /// <summary>
        /// 更新数据库的Blob数据类型,需指定sql参数为length=1的参数
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="ImageData"></param>
        /// <returns></returns>
        public int InputBlob(string strSql, byte[] ImageData)
        {

            //			string block="INSERT INTO test_image(id,name, image) VALUES (2,'a', :blobtodb)";
            using (OracleConnection conn = new OracleConnection())
            {
                conn.ConnectionString = this.ConnStr;
                using (OracleCommand command = new OracleCommand())
                {
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();
                        command.Connection = conn;

                        command.CommandText = strSql + "";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Clear();
                        string strParam = "";
                        int i = strSql.IndexOf(":", 0);
                        if (i <= 0)
                        {
                            this.Err = "未指定参数！" + strSql;
                            return -1;
                        }
                        strParam = strSql.Substring(i + 1, 1);
                        OracleParameter param = command.Parameters.Add(strParam, OracleType.Clob);
                        param.Direction = System.Data.ParameterDirection.Input;

                        // Assign Byte Array to Oracle Parameter
                        param.Value = ImageData;

                        // Step 5
                        // Execute the Anonymous PL/SQL Block

                        command.ExecuteNonQuery();
                    }
                    catch (OracleException ex)
                    {
                        this.Err = "执行产生错误!" + ex.Message;
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        this.Err = ex.Message;
                        return -1;
                    }
                    finally
                    {
                        if (conn.State == System.Data.ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

            return 0;
        }

        public int ExecNoQuery(string strSql)
        {
            using (OracleConnection conn = new OracleConnection())
            {
                conn.ConnectionString = this.ConnStr;
                using (OracleCommand command = new OracleCommand())
                {
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();
                        command.Connection = conn;

                        command.CommandText = strSql + "";
                        command.CommandType = System.Data.CommandType.Text;
                        return command.ExecuteNonQuery();
                    }
                    catch (OracleException ex)
                    {
                        this.Err = "执行产生错误！" + ex.Message;
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        this.Err = ex.Message;
                        return -1;
                    }
                    finally
                    {
                        if (conn.State == System.Data.ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

        }


        /// <summary>
        /// 执行查询语句,返回Reader
        /// </summary>
        /// <param name="strSql">执行sql语句</param>
        /// <returns>0 success -1 fail</returns>
        public int ExecQuery(string strSql, ref OracleDataReader Reader)
        {
             OracleConnection conn = new OracleConnection();
            {
                conn.ConnectionString = this.ConnStr;
                 OracleCommand command = new OracleCommand();
                {

                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();
                        command.Connection = conn;
                        command.CommandText = strSql + "";
                        command.CommandType = System.Data.CommandType.Text;
                        Reader = command.ExecuteReader();
                    }

                    catch (Exception ex)
                    {
                        Err = "执行产生错误!" + ex.Message;
                        //ErrorException = ex.InnerException + "+ " + ex.Source;
                        //ErrCode = strSql;
                        //WriteErr();
                    }
                    finally
                    {
                        //if (conn.State == System.Data.ConnectionState.Open)
                        //    conn.Close();
                    }
                }
            }
            return 0;
        }

        public OracleDataReader Reader;

        public int ExecQuery(string strSql)
        {
            return ExecQuery(strSql,ref this.Reader);
        }
    }
}
