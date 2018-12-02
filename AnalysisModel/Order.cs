using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NetScape.AnalysisModel
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

        private string applyTime;

        private string recipeNO;

        private DateTime dtMOTime;

        private string status;

        private string checkNO;

        private BaseObj execDept = new BaseObj();

        private BaseObj healthHistory = new BaseObj();

        public BaseObj HealthHistory
        {
            get
            {
                if (healthHistory == null)
                {
                    return new BaseObj();
                } 
                return healthHistory;
            }
            set { healthHistory = value; }
        }

        public PatientInfo Patient
        {
            get
            {
                if (patient == null)
                {
                    return new PatientInfo();
                }
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
                if (reciptDoctor == null)
                {
                    return new BaseObj();
                }
                return this.reciptDoctor;
            }
            set
            {
                this.reciptDoctor = value;
            }
        }

        public BaseObj ExecDept
        {
            get
            {
                if (execDept == null)
                {
                    return new BaseObj();
                }
                return execDept;
            }
            set { execDept = value; }
        }

        public BaseObj ReciptDept
        {
            get
            {
                if (reciptDept == null)
                {
                    return new BaseObj();
                }
                return reciptDept;
            }
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

        private BaseObj metalInfo = new BaseObj();

        public BaseObj MetalInfo
        {
            get
            {
                if (metalInfo == null)
                {
                    return new BaseObj();
                }
                return metalInfo;
            }
            set { metalInfo = value; }
        }

        private BaseObj execOper = new BaseObj();

        public BaseObj ExecOper
        {
            get
            {
                if (execOper == null)
                {
                    return new BaseObj();
                }
                return execOper;
            }
            set { execOper = value; }
        }

        private ArrayList orderItems = new ArrayList();

        public ArrayList OrderItems
        {
            get
            {
                if (orderItems == null)
                {
                    return new ArrayList();
                }
                return orderItems;
            }
            set { orderItems = value; }
        }

        private bool isUrgent;

        public bool IsUrgent
        {
            get { return isUrgent; }
            set { isUrgent = value; }
        }

        public string ApplyTime
        {
            get { return applyTime; }
            set { applyTime = value; }
        }

        private string _feeState;

        public string FeeState
        {
            get { return _feeState; }
            set { _feeState = value; }
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
