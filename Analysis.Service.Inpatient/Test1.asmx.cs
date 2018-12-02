using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Analysis.Service.Inpatient
{
    /// <summary>
    /// Test1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Test1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            Request req = new Request();
            req.requestBody = @"<List>
    <RequestReceive>
        <SystemIn>1</SystemIn>
        <AppointmentState>0</AppointmentState>
        <SheetID>20801520341</SheetID>
        <JZLSH>ZY010001107525</JZLSH>
        <NeedSchedule>0</NeedSchedule>
        <PatientStyle>2</PatientStyle>
        <GHStyle>2</GHStyle>
        <DetailPatientStyle/>
        <OutHospitalID/>
        <InHospitalID>0001107525</InHospitalID>
        <PatientName>温文助</PatientName>
        <PatientSex>1</PatientSex>
        <PatientKH/>
        <Patientage>19880220000000</Patientage>
        <PatientTel>15118984576</PatientTel>
        <PatientAddress>广东省普宁市高埔镇黄竹坑村47号</PatientAddress>
        <InHospitaltime/>
        <PatientBedNum>9936024</PatientBedNum>
        <Nativeplace/>
        <Occupation/>
        <binglizhaiyao>    患者及家属诉约半年前无明显诱因出现声音嘶哑，无呛咳，无发热、咳嗽、咳痰，3月前在珠江医院检查，病理提示鳞状细胞癌，后患者自动出院口服中药2月，无效，今为进一步诊治来我院就诊，门诊以“喉癌”收住。</binglizhaiyao>
        <jianyanjianchajieguo/>
        <LinChuangZhenDuan>喉癌</LinChuangZhenDuan>
        <DepartMentID>1104</DepartMentID>
        <DepartMent>耳鼻咽喉二区</DepartMent>
        <ReqSheetDoctorID>002513</ReqSheetDoctorID>
        <ReqSheetDoctor>文卫平</ReqSheetDoctor>
        <ReqSheetDate>20180723</ReqSheetDate>
        <ReqSheetTime>175717</ReqSheetTime>
        <ExamKSDM>7432</ExamKSDM>
        <ExamKSMC>心电生理室</ExamKSMC>
        <ExamReady/>
        <ExamPlace/>
        <RequestState>0</RequestState>
        <inhospitaltimes>1</inhospitaltimes>
        <zylsh/>
        <IsEmergent/>
        <HTH>193</HTH>
        <IdentityNO>440527195502205359</IdentityNO>
        <PAYRESULT>2</PAYRESULT>
        <RequisitionStatus>1</RequisitionStatus>
        <ExamList>
            <Exam>
                <id>0</id>
                <SystemIn>2</SystemIn>
                <SheetID>20801520341</SheetID>
                <ExamID>1300004645</ExamID>
                <Description/>
                <qucaidengjin/>
                <ExamModality>其它</ExamModality>
                <ExamBodyPart>动态心电图</ExamBodyPart>
                <ExamInstruction>动态心电图</ExamInstruction>
                <ExamBodyNum/>
                <ExamDetails/>
                <ExamFilmHave/>
                <ExamState>1</ExamState>
                <FeeState/>
                <RequestSheetStyle/>
                <isvalid/>
            </Exam>
        </ExamList>
        <FeeList>
            <Fee>
                <id>0</id>
                <SystemIn>2</SystemIn>
                <SheetID>20801520341</SheetID>
                <FEEID>-495</FEEID>
                <FEENAME>动态心电图</FEENAME>
                <FeePrice>448.38</FeePrice>
                <FeeUnit/>
                <FEENum>1</FEENum>
                <ItemCode>11742</ItemCode>
                <DrugFlag/>
                <ExamID>1300004645</ExamID>
                <InputCode>-495</InputCode>
                <FeeState/>
            </Fee>
        </FeeList>
        <RisList/>
        <requisitiontype>8</requisitiontype>
        <IsCure>0</IsCure>
    </RequestReceive>
</List>
";
            new RequestNote().RequestDataReceive(req);
            return req.requestBody;
        }

        [WebMethod]
        public string TestConn()
        {
            DataAccess dataMgr = new DataAccess();
            return dataMgr.GetSysDateTime().ToString() ;
        }
    }
}
