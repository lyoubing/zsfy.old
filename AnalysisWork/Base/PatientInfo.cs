using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetScape.AnalysisWork.Base
{
    public class PatientInfo : BaseObj
    {
        /// <summary>
		///患者类
		/// </summary>
        public PatientInfo()
		{
		
		}

		#region 变量

		/// <summary>
		/// 患者各种编号
		/// </summary>
		private PID pid = new PID();

		/// <summary>
		/// 出生日期
		/// </summary>
		private System.DateTime birthday;

		/// <summary>
		/// 年龄
		/// </summary>
		private string age;

		/// <summary>
		/// 性别
		/// </summary>
		private string sex;

		/// <summary>
		/// 家庭地址
		/// </summary>
		private string address;

		/// <summary>
		/// 家庭电话
		/// </summary>
		private string phoneNumber;

		/// <summary>
		/// 婚姻状态 
		/// </summary>
		private string maritalStatus;

		/// <summary>
		/// 身份证
		/// </summary>
		private string idCard;

		/// <summary>
		/// 职业
		/// </summary>
		private string profession;

		/// <summary>
		/// 籍贯
		/// </summary>
		private string dist;

        private string patientType;

        private string bedNO;

        private string inSource;

		/// <summary>
		/// 合同单位
		/// </summary>
		private string pact;

		/// <summary>
		/// 最后结算序号
		/// </summary>
		private int balanceNO;

		/// <summary>
		/// 门诊诊断
		/// </summary>
		private string clinicDiagnose;

		/// <summary>
		/// 开据医师
		/// </summary>
		private string doctorReceiver;

		/// <summary>
		/// 患者住院主诊断
		/// </summary>
		private string mainDiagnose;

        /// <summary>
        /// 证件类型
        /// </summary>
        private string cardType;

        private string nation;

        public string Nation
        {
            get { return nation; }
            set { nation = value; }
        }

        private string homeZIP;

        public string HomeZIP
        {
            get { return homeZIP; }
            set { homeZIP = value; }
        }

        private string degree;

        public string Degree
        {
            get { return degree; }
            set { degree = value; }
        }


        private BaseObj nurseStation;

        public BaseObj NurseStation
        {
            get { return nurseStation; }
            set { nurseStation = value; }
        }
        private BaseObj dept;

        public BaseObj Dept
        {
            get { return dept; }
            set { dept = value; }
        }

        #endregion

        #region 属性

        /// <summary>
		/// 患者各种编号
		/// </summary>
		public PID PID
		{
			get
			{
				return this.pid;
			}
			set
			{
				this.pid = value;
			}
		}

        /// <summary>
        /// 患者姓名
        /// </summary>
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

		/// <summary>
		/// 出生日期
		/// </summary>
		public System.DateTime Birthday
		{
			get
			{
				return this.birthday;
			}
			set
			{
				this.birthday = value;
			}
		}

		/// <summary>
		/// 年龄
		/// </summary>
		public string Age
		{
			get
			{
				return this.age;
			}
			set
			{
				this.age = value;
			}
		}

		/// <summary>
		/// 性别
		/// </summary>
		public String Sex
		{
			get
			{
				return this.sex;
			}
			set
			{
				this.sex = value;
			}
		}

		/// <summary>
		/// 家庭地址
		/// </summary>
		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		/// <summary>
		/// 家庭电话
		/// </summary>
        public string PhoneNumber
		{
			get
			{
				return this.phoneNumber;
			}
			set
			{
				this.phoneNumber = value;
			}
		}

		/// <summary>
		/// 婚姻状态
		/// </summary>
		public string MaritalStatus
		{
			get
			{
				return this.maritalStatus;
			}
			set
			{
				this.maritalStatus = value;
			}
		}

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CardType
        {
            get
            {
                return cardType;
            }
            set
            {
                cardType = value;
            }
        }

		/// <summary>
		/// 证件号
		/// </summary>
		public string IDCard
		{
			get
			{
				return this.idCard;
			}
			set
			{
				this.idCard = value;
			}
		}

		/// <summary>
		/// 职业
		/// </summary>
		public string Profession
		{
			get
			{
				return this.profession;
			}
			set
			{
				this.profession = value;
			}
		}

		/// <summary>
		/// 籍贯
		/// </summary>
		public string DIST
		{
			get
			{
				return this.dist;
			}
			set
			{
				this.dist = value;
			}
		}

        public string PatientType
        {
            get { return patientType; }
            set { patientType = value; }
        }

        public string BedNO
        {
            get { return bedNO; }
            set { bedNO = value; }
        }

        public string InSource
        {
            get { return inSource; }
            set { inSource = value; }
        }

		/// <summary>
		/// 合同单位
		/// </summary>
		public string Pact
		{
			get
			{
				return this.pact;
			}
			set
			{
				this.pact = value;
			}
		}

        

		/// <summary>
		/// 患者住院主诊断
		/// </summary>
		public string MainDiagnose
		{
			get
			{
				return this.mainDiagnose;
			}
			set
			{
				this.mainDiagnose = value;
			}
		}

        /// <summary>
        /// 门诊诊断
        /// </summary>
        public string ClinicDiagnose
        {
            get
            {
                return this.clinicDiagnose;
            }
            set
            {
                this.clinicDiagnose = value;
            }
        }

        /// <summary>
        ///  住院证开据医师
        /// </summary>
        public string DoctorReceiver
        {
            get
            {
                return this.doctorReceiver;
            }
            set
            {
                this.doctorReceiver = value;
            }
        }

        /// <summary>
        /// 最后结算序号
        /// </summary>
        public int BalanceNO
        {
            get
            {
                return this.balanceNO;
            }
            set
            {
                this.balanceNO = value;
            }
        }

        #endregion

        #region 方法

        /// <summary>
		/// 克隆
		/// </summary>
		/// <returns></returns>
		public new PatientInfo Clone()
		{
            return this.MemberwiseClone() as PatientInfo;
		}

		#endregion
    }
}
