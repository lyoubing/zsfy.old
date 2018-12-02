using System;
using System.Collections;

namespace NetScape.AnalysisToolKit
{
	/// <summary>
	/// NConvert ��ժҪ˵����
	/// </summary>
	public class NConvert {


		/// <summary>
		/// val ������"true",��"false",��"0",��"1".
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static bool ToBoolean(string val) {
			if(val == null) {
				return false;
				//throw new ArgumentNullException(val);
			}
			bool result;
			switch(val.ToLower().Trim()) {
				case "true" :
				case "1":
					result = true;
					break;
				case "false":
				case "0":
					result = false;
					break;
				default :
					result = false;
					break;
			}    
		

			return result;
		}

		public static bool ToBoolean(object val) {
			if(val == null || val.ToString() == string.Empty)
				return false;
			return ToBoolean(val.ToString());
			
		}
		public static bool ToBoolean(int val) {
			if(val == 1)
				return true;
			else
				if(val == 0)
				return false;
			else
				throw new ArgumentException(val.ToString() + " is not equal 1 or 0.");
            
		}


		public static int ToInt32(bool val) {
			if(val == true)
				return 1;
			else
				return 0;
		}

		public static int ToInt32(string val) {
			if( val == null || val == string.Empty || val.ToLower() == "false")
				return 0;
			else
				if(val.ToLower() == "true")
				return 1;

			try {
				return (int)System.Convert.ToDecimal(val);
			}
			catch {
				return 0;
			}

		}

		public static int ToInt32(object val) {
		
			if(  val == null || val.ToString() == string.Empty)
				return 0; {

							  //try
							  return System.Convert.ToInt32(val);
						  } {
								//catch
								//	throw new ArgumentException(val + " is not numbers.");
							}

		}
	 

		public static decimal ToDecimal(object val) {
			if(val == null || val.ToString() == string.Empty)
				return 0;
			return Decimal.Parse(val.ToString());
		}

		public static decimal ToDecimal(string val) {
			if(val == null || val == string.Empty)
				return 0;
			//if(char.IsNumber(
			return Decimal.Parse(val);
		
		}
		

		/// <summary>
		/// ת��ʱ��
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(object val) {
		 
			if(val == null || val.ToString() == string.Empty) return DateTime.MinValue; 
			try
			{
				DateTime d = System.Convert.ToDateTime(val.ToString());
				return d;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		public static DateTime ToDateTime(string val) {
			if(val == null || val == string.Empty) {
				return DateTime.MinValue;     				
			}
			try
			{
				DateTime d = System.Convert.ToDateTime(val.ToString());
				return d;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// ת��arrayList to Array    ����õ����𣿣�
		/// </summary>
		/// <param name="al"></param>
		/// <returns></returns>
		public static object[] ToArray(ArrayList al) {
			return al.ToArray();
		}
		//	public static string ToString(bool

        /// <summary>
        /// ö��ת��
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T ToEnum<T>(string enumString,bool ignoreCase,T defaultValue)
        {
            T tEnum = defaultValue;

            try
            {
                tEnum = (T)System.Enum.Parse(typeof(T), enumString,ignoreCase);
            }
            catch (System.ArgumentException eArgument)
            {
                return defaultValue;
            }
            return tEnum;
        }

        /// <summary>
        /// ö��ת��
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T ToEnum<T>(string enumString, T defaultValue)
        {
            return ToEnum<T>(enumString, false, defaultValue);
        }

		#region  ���Сд���д 
		/// <summary> 
		/// ���Сд���д 
		/// creator : zuowy@Neusoft.com 
		/// 2005.11.23 
		/// </summary> 
		/// <param name="smallnum"></param> 
		/// <returns></returns> 
		public static string ToCapital(decimal smallnum) { 
			string cmoney , cnumber, cnum, cnum_end,cmon ,cno,snum ,sno; 
			int snum_len , sint_len, cbegin, zflag , i; 
			if(smallnum > 1000000000000 || smallnum < -99999999999 || smallnum == 0) 
				return ""; 
			cmoney = "Ǫ��ʰ��Ǫ��ʰ��Ǫ��ʰԪ�Ƿ�" ;// ��д����ҵ�λ�ַ��� 
			cnumber = "Ҽ��������½��ƾ�"          ;// ��д�����ַ��� 
			cnum = ""                               ;// ת����Ĵ�д�����ַ��� 
			cnum_end = ""                           ;// ת����Ĵ�д�����ַ��������һλ 
			cmon = ""                               ;// ȡ��д����ҵ�λ�ַ����е�ĳһλ 
			cno = ""                                ;// ȡ��д�����ַ����е�ĳһλ 
 
 
             
			snum = System.Decimal.Round(smallnum,2).ToString("############.00");  ;// Сд�����ַ��� 
			snum_len = snum.Length                  ;// Сд�����ַ����ĳ��� 
			sint_len = snum_len - 2                 ;// Сд�������������ַ����ĳ��� 
			sno = ""                                ;// Сд�����ַ����е�ĳ�������ַ� 
			cbegin = 15 - snum_len                  ;// ��д����ҵ�λ�еĺ���λ�� 
			zflag = 1                               ;// Сд�����ַ��Ƿ�Ϊ0(0=0)���жϱ�־ 
			i = 0                                   ;// Сд�����ַ����������ַ���λ�� 
 
			if(snum_len > 15) 
				return ""; 
			for(i=0;i<snum_len;i++) { 
				if (i==sint_len-1) 
					continue; 
 
                 
				cmon = cmoney.Substring(cbegin, 1); 
				cbegin = cbegin + 1; 
				sno =snum.Substring(i,1); 
				if (sno=="-") { 
					cnum = cnum + "��"; 
					continue; 
				} 
				else if(sno=="0") { 
					cnum_end = cnum.Substring(cnum.Length-2,1); 
					if(cbegin == 4 || (cbegin == 8 || cnum_end.IndexOf("��")>=0|| cbegin == 12 )) { 
						cnum = cnum + cmon; 
						if (cnumber.IndexOf(cnum_end)>=0 ) 
							zflag = 1; 
						else 
							zflag = 0; 
					} 
					else { 
						zflag = 0; 
					} 
					continue; 
				} 
				else if( sno != "0" && zflag == 0) { 
					cnum = cnum + "��"; 
					zflag = 1; 
				} 
				cno =cnumber.Substring(System.Convert.ToInt32(sno)-1, 1); 
				cnum = cnum + cno + cmon; 
			} 
			if (snum.Substring(snum.Length-2,1)=="0") { 
				return  cnum + "��"; 
			} 
			else 
				return cnum; 
		} 

		#endregion 
	}
}
