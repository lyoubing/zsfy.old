using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Common
{
    public class ConstManager
    {
        // public static List<NetScape.AnalysisModel.Constant> _ectItems;

        Constant dbConst = new Constant();

        private static List<NetScape.AnalysisModel.Constant> _ectItems;

        public static List<NetScape.AnalysisModel.Constant> EcgItems
        {
            get
            {
                if (_ectItems == null)
                {
                    _ectItems = new Constant().QueryConstant("ECGITEMS").Cast<NetScape.AnalysisModel.Constant>().ToList();
                }
                return _ectItems;
            }
            set { _ectItems = value; }
        }

        private string _execDept;

        public string ExecDept
        {
            get
            {
                if (string.IsNullOrEmpty(_execDept))
                    _execDept = string.Join("||||", EcgItems.Select(x => x.InputCode).Distinct().ToArray());
                return _execDept;
            }
            set { _execDept = value; }
        }

        /// <summary>
        /// 根据编码判断项目是否属心电类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool CheckEcgItemsExists(string id)
        {
            return EcgItems.Where(x => x.ID == id).Count() > 0;
        }

        /// <summary>
        /// 根据执行科室判断
        /// </summary>
        /// <param name="deptCode"></param>
        /// <returns></returns>
        public static bool CheckEcgExecDeptExists(string deptCode)
        {
            return EcgItems.Where(x => x.Mark.Trim() == deptCode).Count() > 0;
        }

        /// <summary>
        /// 判断项目是否需要发送给MUSE系统
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool CheckItemNeedSendMuse(string id)
        {
            return EcgItems.Where(x => x.ID == id && x.Kind == "1").Count() > 0;
        }


        public static string GetMuseSexCode(string code)
        {
            //string sexCode = string.Empty;
            if ("男|1|M".Contains(code))
                return "M";
            else if ("女||2||F".Contains(code))
                return "F";
            else if ("其他||3||O".Contains(code))
                return "O";
            else//("未知||9||U".Contains(code))
                return "U";
        }

        public static string GetMuseExecDept()
        {
            return string.Join(",", EcgItems.Where(x => x.Mark != "").Select(x => x.Mark).Distinct().ToArray<string>());
        }

        public static NetScape.AnalysisModel.Constant GetDeptItem(string code)
        {

            NetScape.AnalysisModel.Constant _const = new Constant().QueryConstItem("ECGDEPT", code);
            if (_const != null)
                return _const;
            else
                return new NetScape.AnalysisModel.Constant();
        }

        public static NetScape.AnalysisModel.Constant GetEmployee(string code)
        {
            NetScape.AnalysisModel.Constant _const = new Constant().QueryConstItem("ECGEMPL", code);
            if (_const != null)
                return _const;
            else return new NetScape.AnalysisModel.Constant();
        }

        public static string GetPatientType(string code)
        {
            string type = string.Empty;
            switch (code)
            {
                case "0":
                    type = "门诊";
                    break;
                case "1":
                    type = "急诊";
                    break;
                case "2":
                    type = "住院";
                    break;
                case "3":
                    type = "体检";
                    break;
                default:
                    break;
            }
            return type;
        }
    }
}
