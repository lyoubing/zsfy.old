using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
{
    public class OrderItem : BaseObj
    {
        private string applyNo; //申请单号

        private string orderID; //医嘱序号

        private string sequenceNO;  //医嘱子序号

        private BaseObj item = new BaseObj();

        private string combNO;

        private BaseObj checkPart = new BaseObj(); //检查相关  设备  部位 描述

        private bool isEnhance = false; //是否增强

        private decimal totCost = 0m;

        private string sample;


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
                return this.item;
            }
            set
            {
                this.item = value;
            }
        }

        public string CombNO
        {
            get { return combNO; }
            set { combNO = value; }
        }

        public BaseObj CheckPart
        {
            get { return checkPart; }
            set { checkPart = value; }
        }

        public bool IsEnhance
        {
            get { return isEnhance; }
            set { isEnhance = value; }
        }

        public decimal TotCost
        {
            get { return totCost; }
            set { totCost = value; }
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
    }
}
