using System;
using System.Globalization;

namespace YP.Base.CSS.DataType
{
	/// <summary>
	/// CSSNumber ��ժҪ˵����
	/// </summary>
	public class CSSNumber:Interface.DataType.ICSSNumber
	{
		#region  ..Constructor
		public CSSNumber(string numberstr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.floatvalue = CSSNumber.ParseNumberStr(numberstr);
		}
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ������ֵ
		/// </summary>
		public float Value
		{
			get
			{
				return this.floatvalue;
			}
		}
		#endregion

		#region ..private fields
		float floatvalue = 0;
		#endregion

		#region ..���������ַ���
		/// <summary>
		/// ���������ַ���
		/// </summary>
		/// <param name="numberstr">�����ַ���</param>
		/// <returns></returns>
		public static float ParseNumberStr(string numberstr)
		{
			if(numberstr.Length > 0)
			{
				NumberFormatInfo format = new NumberFormatInfo();
				format.NumberDecimalSeparator = ".";

				string str = numberstr;
				float val;
				int index = str.IndexOfAny(new Char[]{'E','e'});
				if(index> -1)
				{
					val = (float)Math.Pow(
						Single.Parse(str.Substring(0, index), format), 
						Int32.Parse(str.Substring(index+1))
						);
				}
				else
				{
					val = Single.Parse(str, format);
				}
				return val;
			}
			else
				return 0;
		}
		#endregion
	}
}
