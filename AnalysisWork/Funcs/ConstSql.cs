using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Funcs
{
    public static class ConstSql
    {
        #region  QueryOrderByApplyNo

        public static string QueryOrderByApplyNo = @"SELECT DISTINCT P.SERIALNUM,--1
            P.PATIENTID,--2
            P.CARDNO,--3
            P.CLINICCODE,--4
            P.IDCARDNO,--5
            P.IDENNO,--6
            P.NAME,--7
            P.SPELLCODE,--8
            P.WBCODE,--9
            P.SEX,--10
            P.BIRTHYDAY,--11
            P.HOMETEL,--12
            P.MOBILETEL,--13
            P.HOMEADRRESS,--14
            P.PATIENTTYPE,--15
            P.COUNTRY,--16
            P.NATION,--17
            P.OPERCODE,--18
            P.OPERDATE,--19
            P.MARK,--20
            P.EXT1,--21
            P.EXT2,--22
            P.EXT3,--23
            P.EXT4,--24
            P.STATE, --25 检查状态
            P.FEESTATE,--26 费用状态
            P.APPLYNO --27 申请单号
            FROM ECGPATIENTINFO P
            WHERE P.APPLYNO='{0}'
            AND P.OPERDATE>SYSDATE -30  ";

        #endregion

        #region  QueryOrderByPatientId

        public static string QueryOrderByPatientId = @"SELECT DISTINCT P.SERIALNUM,--1
            P.PATIENTID,--2
            P.CARDNO,--3
            P.CLINICCODE,--4
            P.IDCARDNO,--5
            P.IDENNO,--6
            P.NAME,--7
            P.SPELLCODE,--8
            P.WBCODE,--9
            P.SEX,--10
            P.BIRTHYDAY,--11
            P.HOMETEL,--12
            P.MOBILETEL,--13
            P.HOMEADRRESS,--14
            P.PATIENTTYPE,--15
            P.COUNTRY,--16
            P.NATION,--17
            P.OPERCODE,--18
            P.OPERDATE,--19
            P.MARK,--20
            P.EXT1,--21
            P.EXT2,--22
            P.EXT3,--23
            P.EXT4,--24
            P.STATE, --25 检查状态
            P.FEESTATE,--26 费用状态
            P.APPLYNO --27 申请单号
            FROM ECGPATIENTINFO P
            WHERE (P.PATIENTID='{0}' OR P.CLINICCODE='{}')
            AND P.OPERDATE>SYSDATE -30  
            ORDER BY  P.OPERDATE DESC ";

        #endregion

        #region PatientBaseSql

        public static string PatientBaseSql = @"
SELECT DISTINCT P.SERIALNUM,--1
P.PATIENTID,--2
P.CARDNO,--3
P.CLINICCODE,--4
P.IDCARDNO,--5
P.IDENNO,--6
P.NAME,--7
P.SPELLCODE,--8
P.WBCODE,--9
P.SEX,--10
P.BIRTHYDAY,--11
P.HOMETEL,--12
P.MOBILETEL,--13
P.HOMEADRRESS,--14
P.PATIENTTYPE,--15
P.COUNTRY,--16
P.STATE, --17
P.NATION,--17
P.OPERCODE,--18
P.OPERDATE,--19
P.MARK,--20
P.EXT1,--21
P.EXT2,--22
P.EXT3,--23
P.EXT4,--24
P.FEESTATE,--25
P.APPLYNO
FROM ECGPATIENTINFO P 

";

        #endregion
    }
}
