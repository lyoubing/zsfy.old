using Neusoft.HISFC.Models.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Common
{
    public class Constant : Neusoft.FrameWork.Management.Database
    {
        /// <summary>
        /// 根据类型获得常数所有在用的列
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public  ArrayList QueryConstant(string type)
        {
            string strSql = string.Empty;

            ArrayList al = new ArrayList();
            
            //if (this.Sql.Sql.GetSql("Manager.Constant.2", ref strSql) == -1) return null;

            #region sql

            strSql = @"
select type,
       code,
       name,
       mark,
       spell_code,
       wb_code,
       input_code,
       sort_id,
       VALID_STATE,      
       kind_id
  from com_dictionary
 where type = '{0}'
   and VALID_STATE = '1'
 order by sort_id";

            #endregion

            try
            {
                strSql = string.Format(strSql, type);
            }
            catch (Exception ex)
            {
                this.ErrCode = ex.Message;
                this.Err = "查询常数！" + ex.Message;
                this.WriteErr();
                return null;
            }
            if (this.ExecQuery(strSql) == -1) return null;
            //Neusoft.FrameWork.Models.NeuObject NeuObject;
           NetScape.AnalysisModel.Constant cons;
            while (this.Reader.Read())
            {
                //NeuObject=new NeuObject();
                cons = new  AnalysisModel.Constant();
                cons.Key = (Reader[0].ToString());
                cons.ID = this.Reader[1].ToString();
                cons.Name = this.Reader[2].ToString();
                cons.Mark = this.Reader[3].ToString();
                cons.SpellCode = this.Reader[4].ToString();
                cons.WbCode = this.Reader[5].ToString();
                cons.InputCode = this.Reader[6].ToString();
                if (!Reader.IsDBNull(7))
                    cons.SortId = Convert.ToInt32(this.Reader[7]);
                cons.ValidState = this.Reader[8].ToString();
                cons.Kind = this.Reader[9].ToString();    
                al.Add(cons);
            }
            this.Reader.Close();
            return al;
        }

        public NetScape.AnalysisModel.Constant  QueryConstItem(string type,string code)
        {
            string sql = @"
select type,
       code,
       name,
       mark,
       spell_code,
       wb_code,
       input_code,
       sort_id,
       VALID_STATE,      
       kind_id
  from com_dictionary
 where type = '{0}'
   and code= '{1}'
   and VALID_STATE = '1'
 order by sort_id ";
            sql = string.Format(sql, type, code);
            if (this.ExecQuery(sql) == -1) return null;
            //Neusoft.FrameWork.Models.NeuObject NeuObject;
            NetScape.AnalysisModel.Constant cons;
            while (this.Reader.Read())
            {
                //NeuObject=new NeuObject();
                cons = new AnalysisModel.Constant();
                cons.Key = (Reader[0].ToString());
                cons.ID = this.Reader[1].ToString();
                cons.Name = this.Reader[2].ToString();
                cons.Mark = this.Reader[3].ToString();
                cons.SpellCode = this.Reader[4].ToString();
                cons.WbCode = this.Reader[5].ToString();
                cons.InputCode = this.Reader[6].ToString();
                if (!Reader.IsDBNull(7))
                    cons.SortId = Convert.ToInt32(this.Reader[7]);
                cons.ValidState = this.Reader[8].ToString();
                cons.Kind = this.Reader[9].ToString();
                return cons;
            }
            this.Reader.Close();
            return null;
        }

    }
}
