using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
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

        public new BaseObj Clone()
        {
            BaseObj obj = new BaseObj().Clone();

            return obj;
        }
    }
}
