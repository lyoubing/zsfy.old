using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
{
    public class PID : BaseObj
    {
        /// <summary>
        /// 患者索引号
        /// </summary>
        public PID()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //

        }

        /// <summary>
        /// 门诊卡号
        /// </summary>
        private string cardNO;

        /// <summary>
        /// 病历号
        /// </summary>
        private string caseNO;

        /// <summary>
        /// 健康档案号(体检号)
        /// </summary>
        private string healthNO;

        /// <summary>
        /// 门诊卡号
        /// </summary>
        public string CardNO
        {
            get
            {
                return this.cardNO;
            }
            set
            {
                this.cardNO = value;
            }
        }

        /// <summary>
        /// 病历号
        /// </summary>
        public string CaseNO
        {
            get
            {
                return this.caseNO;
            }
            set
            {
                this.caseNO = value;
            }
        }

        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNO
        {
            get
            {
                return this.ID;
            }
            set
            {
                this.ID = value;
            }
        }

        /// <summary>
        /// 健康档案号(体检号)
        /// </summary>
        public string HealthNO
        {
            get
            {
                return this.healthNO;
            }
            set
            {
                this.healthNO = value;
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new PID Clone()
        {
            return this.MemberwiseClone() as PID;
        }
    }
}