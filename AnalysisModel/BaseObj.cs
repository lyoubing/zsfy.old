using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    public class BaseObj
    {
        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string memo;

        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _ext1;

        public string Ext1
        {
            get { return _ext1; }
            set { _ext1 = value; }
        }


        private string _ext2;

        public string Ext2
        {
            get { return _ext2; }
            set { _ext2 = value; }
        }

        private string _ext3;

        public string Ext3
        {
            get { return _ext3; }
            set { _ext3 = value; }
        }

        private string _mark;

        public string Mark
        {
            get { return _mark; }
            set { _mark = value; }
        }
        
        

        public new BaseObj Clone()
        {
            return this.MemberwiseClone() as BaseObj;
        }
    }
}
