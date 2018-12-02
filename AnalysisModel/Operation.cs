using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    public class Operation
    {
        private string code;

        public string Code
        {
          get { return code; }
          set { code = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public new Operation Clone()
        {
            return this.MemberwiseClone() as Operation;
        }
    }
}
