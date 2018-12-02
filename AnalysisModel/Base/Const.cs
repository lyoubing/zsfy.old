using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel
{
    public class Const
    {

        public static BaseObj GetFeeState(string _stateId)
        {
            return FeeStates.Where(x => x.ID == _stateId).FirstOrDefault();
        }

        public static BaseObj GetCheckState(string _stateId)
        {
            return CheckStates.Where(x => x.ID == _stateId).FirstOrDefault();
        }

        private static List<BaseObj> _feeStates;

        public static List<BaseObj> FeeStates
        {
            get
            {
                if (_feeStates == null || _feeStates.Count == 0)
                {
                    _feeStates = new List<BaseObj>();
                    _feeStates.Add(new BaseObj()
                    {
                        ID = "1",
                        Name = "未收费"
                    });
                    _feeStates.Add(new BaseObj()
                    {
                        ID = "2",
                        Name = "已收费"
                    });
                    _feeStates.Add(new BaseObj()
                    {
                        ID = "1100",
                        Name = "已申请退费"
                    });
                    _feeStates.Add(new BaseObj()
                    {
                        ID = "1101",
                        Name = "退费已完成"
                    });
                }
                return _feeStates;
            }
            set { _feeStates = value; }
        }

        private static List<BaseObj> _checkStates;

        public static List<BaseObj> CheckStates
        {
            get
            {
                if (_checkStates == null || _checkStates.Count == 0)
                {
                    _checkStates = new List<BaseObj>();
                    _checkStates.Add(new BaseObj()
                    {
                        ID = "1",
                        Name = "未报到"
                    });
                    _checkStates.Add(new BaseObj()
                    {
                        ID = "2",
                        Name = "已报到"
                    });
                    _checkStates.Add(new BaseObj()
                    {
                        ID = "3",
                        Name = "已检查"
                    });
                    _checkStates.Add(new BaseObj()
                    {
                        ID = "4",
                        Name = "报告发布"
                    });
                    _checkStates.Add(new BaseObj()
                    {
                        ID = "5",
                        Name = "退检"
                    });
                }
                return _checkStates;
            }
            set { _checkStates = value; }
        }


    }


    public class Constant:BaseObj
    {
        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _input;

        public string InputCode
        {
            get { return _input; }
            set { _input = value; }
        }

        private string _spell;

        public string SpellCode
        {
            get { return _spell; }
            set { _spell = value; }
        }

        private string _wb;

        public string WbCode
        {
            get { return _wb; }
            set { _wb = value; }
        }

        private string _kind;

        /// <summary>
        /// 小类
        /// </summary>
        public string Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        private string _parent;

        public string ParentCode
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private string _current;

        public string CurrentCode
        {
            get { return _current; }
            set { _current = value; }
        }

        private string _valid;

        public string ValidState
        {
            get { return _valid; }
            set { _valid = value; }
        }

        private string _key;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private int _sort;

        public int SortId
        {
            get { return _sort; }
            set { _sort = value; }
        }

        
    }

}
