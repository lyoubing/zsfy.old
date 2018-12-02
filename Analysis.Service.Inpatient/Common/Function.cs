using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Data.OracleClient;
namespace Analysis.Service
{
    public class Function
    {
        public static XElement ResponseXml(string code, string errMsg)
        {
            XElement root = 
                new XElement("ERequestReceiveResult",
                    new XElement("ReceiveResult", code)/*,
                    new XElement("ErrMsg", errMsg)*/
                    );
            return root;

        }
    }

    public class DataAccess : Neusoft.FrameWork.Management.Database
    { }

    public class TestDataAccess
    {
        public static string TestGetDate()
        {
            using (OracleConnection conn = new OracleConnection("data source=HISDB;password=ecg;persist security info=True;user id=ecg"))
            {
                using (OracleCommand command = new OracleCommand())
                {
                    try
                    {
                        if (conn.State == System.Data.ConnectionState.Closed)
                            conn.Open();
                        command.Connection = conn;

                        command.CommandText = "select sysdate from dual";
                        command.CommandType = System.Data.CommandType.Text;
                        OracleDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            return reader[0].ToString();
                        }
                    }
                    catch(Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
            return " gou ri d ";
        }

    }
}