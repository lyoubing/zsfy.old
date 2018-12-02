using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisModel.Base
{
    public class Sex : BaseObj
    {
        public static Sex GetSex(string _sexId)
        {
            return SexList.Where(x => x.ID == _sexId).FirstOrDefault();
        }
        private static List<Sex> _sdic;

        public static List<Sex> SexList
        {
            get
            {
                if (_sdic == null)
                {
                    _sdic = new List<Sex>();
                    _sdic.Add(new Sex()
                    {
                        ID = "M",
                        Name = "男"
                    });
                    _sdic.Add(new Sex()
                    {
                        ID = "F",
                        Name = "女"
                    });
                    _sdic.Add(new Sex()
                    {
                        ID = "O",
                        Name = "其他"
                    });
                    _sdic.Add(new Sex()
                    {
                        ID = "U",
                        Name = "未知"
                    });
                    _sdic.Add(new Sex()
                    {
                        ID = "1",
                        Name = "男"
                    });
                    _sdic.Add(new Sex()
                    {
                        ID = "2",
                        Name = "女"
                    });
                    _sdic.Add(new Sex()
                    {
                        ID = "3",
                        Name = "其他"
                    });
                }
                return _sdic;
            }
        }

    }

    //public class SexDictionary
    //{
    //    private static List<Sex> _sdic;

    //    public static List<Sex> SexList
    //    {
    //        get {
    //            if (_sdic == null)
    //            {
    //                _sdic = new List<Sex>();
    //                _sdic.Add(new Sex()
    //                {
    //                    ID = "M",
    //                    Name = "男"
    //                });
    //                _sdic.Add(new Sex()
    //                {
    //                    ID = "F",
    //                    Name = "女"
    //                });
    //                _sdic.Add(new Sex()
    //                {
    //                    ID = "O",
    //                    Name = "其他"
    //                });
    //                _sdic.Add(new Sex()
    //                {
    //                    ID = "U",
    //                    Name = "未知"
    //                });
    //            }
    //            return _sdic; }
    //    }

    //}
}
