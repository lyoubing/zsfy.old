using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NetScape.AnalysisWork.Base
{
    public class Order : BaseObj
    {
        public Order()
        {

        }

        private PatientInfo patient = new PatientInfo();

        private BaseObj reciptDoctor = new BaseObj();

        private BaseObj reciptDept = new BaseObj();

        private DateTime applyDate;

        private string recipeNO;

        private DateTime dtMOTime;

        private string status;

        private string checkNO;

        private BaseObj execDept = new BaseObj();

        private BaseObj healthHistory = new BaseObj();

        public BaseObj HealthHistory
        {
            get { return healthHistory; }
            set { healthHistory = value; }
        }

        public PatientInfo Patient
        {
            get
            {
                return this.patient;
            }
            set
            {
                this.patient = value;
            }
        }

        public DateTime MOTime
        {
            get
            {
                return this.dtMOTime;
            }
            set
            {
                this.dtMOTime = value;
            }
        }

        public string ReciptNO
        {
            get
            {
                return this.recipeNO;
            }
            set
            {
                this.recipeNO = value;
            }
        }

        public BaseObj ReciptDoctor
        {
            get
            {
                return this.reciptDoctor;
            }
            set
            {
                this.reciptDoctor = value;
            }
        }

        public BaseObj ExecDept
        {
            get { return execDept; }
            set { execDept = value; }
        }

        public BaseObj ReciptDept
        {
            get { return reciptDept; }
            set { reciptDept = value; }
        }

        public DateTime ApplyDate
        {
            get { return applyDate; }
            set { applyDate = value; }
        }

        public string CheckNO
        {
            get { return checkNO; }
            set { checkNO = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }


        private ArrayList orderItems;

        public ArrayList OrderItems
        {
            get { return orderItems; }
            set { orderItems = value; }
        }

        #region 方法

        #region 克隆

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Order Clone()
        {
            // TODO:  添加 Order.Clone 实现
            Order obj = new Order();
            //Order obj = base.Clone() as Order;
            //obj.Combo = this.Combo.Clone();
            //obj.DcReason = this.DcReason.Clone();

            //obj.Frequency = (Frequency)this.Frequency.Clone();

            //try { obj.ExeDept = this.ExeDept.Clone(); }
            //catch { };
            //try { obj.InDept = this.InDept.Clone(); }
            //catch { };
            //try { obj.NurseStation = this.NurseStation.Clone(); }
            //catch { };
            //try { obj.ReciptDept = this.ReciptDept.Clone(); }
            //catch { };
            //try { obj.DoctorDept = this.DoctorDept.Clone(); }
            //catch { };
            //try { obj.StockDept = this.StockDept.Clone(); }
            //catch { };

            //obj.Item = this.Item.Clone();
            //obj.Nurse = this.Nurse.Clone();

            //try { obj.Patient = this.Patient.Clone(); }
            //catch { };

            //obj.Usage = this.Usage.Clone();
            //obj.oper = this.oper.Clone();
            //obj.execOper = this.execOper.Clone();
            //obj.dcOper = this.dcOper.Clone();

            //obj.compound = this.compound.Clone();

            return obj;
        }


        #endregion

        #endregion
    }
}
