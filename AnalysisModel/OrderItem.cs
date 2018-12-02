using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    public class OrderItem : BaseObj
    {
        private string checkID; //检查流水号

        private string applyNo; //申请单号

        private string orderID; //医嘱序号

        private string sequenceNO;  //医嘱子序号

        private BaseObj item = new BaseObj(); //项目信息

        private BaseObj applyDept = new BaseObj(); //申请科室

        private BaseObj execDept = new BaseObj();//执行科室

        private BaseObj applyOper = new BaseObj();//申请人

        private DateTime operDate; //操作时间

        private decimal totCost = 0m; //金额

        private BaseObj sysClass = new BaseObj(); //项目类别信息

        private BaseObj checkPart = new BaseObj(); //检查相关  设备  部位 描述

        private string combNO;

        private bool isEnhance = false; //是否增强

        private string sample;

        public string CheckID
        {
            get { return checkID; }
            set { checkID = value; }
        }

        public string ApplyNo
        {
            get { return applyNo; }
            set { applyNo = value; }
        }

        public string OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public string SequenceNO
        {
            get
            {
                return this.sequenceNO;
            }
            set
            {
                this.sequenceNO = value;
            }
        }

        public BaseObj Item
        {
            get
            {
                if (item == null)
                {
                    return new BaseObj();
                }
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }

        public BaseObj ApplyDept
        {
            get
            {
                if (applyDept == null)
                {
                    return new BaseObj();
                }
                return applyDept;
            }
            set { applyDept = value; }
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

        public BaseObj ApplyOper
        {
            get
            {
                if (applyOper == null)
                {
                    return new BaseObj();
                }
                return applyOper;
            }
            set { applyOper = value; }
        }

        public DateTime OperDate
        {
            get { return operDate; }
            set { operDate = value; }
        }

        public decimal TotCost
        {
            get { return totCost; }
            set { totCost = value; }
        }

        public BaseObj SysClass
        {
            get
            {
                if (sysClass == null)
                {
                    return new BaseObj();
                }
                return sysClass;
            }
            set { sysClass = value; }
        }


        public string CombNO
        {
            get { return combNO; }
            set { combNO = value; }
        }

        public BaseObj CheckPart
        {
            get
            {
                if (checkPart == null)
                {
                    return new BaseObj();
                }
                return checkPart;
            }
            set { checkPart = value; }
        }

        public bool IsEnhance
        {
            get { return isEnhance; }
            set { isEnhance = value; }
        }

        private string _state;
        /// <summary>
        /// 状态
        /// </summary>
        public string OrderState
        {
            get { return _state; }
            set { _state = value; }
        }

      
        

        private int _qty;

        public int Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        private decimal _unitPrice;

        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set { _unitPrice = value; }
        }

        private string _unit;

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        private DateTime _applyDate;

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyDate
        {
            get { return _applyDate; }
            set { _applyDate = value; }
        }
        

        public string Sample
        {
            get
            {
                return this.sample;
            }
            set
            {
                this.sample = value;
            }
        }



        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new OrderItem Clone()
        {
            return this.MemberwiseClone() as OrderItem;
        }
    }
}
