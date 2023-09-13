using System;
using System.Globalization;

namespace YP.Base.CSS.DataType
{
	/// <summary>
	/// CSSNumber 的摘要说明。
	/// </summary>
	public class CSSNumber:Interface.DataType.ICSSNumber
	{
		#region  ..Constructor
		public CSSNumber(string numberstr)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.floatvalue = CSSNumber.ParseNumberStr(numberstr);
		}
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取浮点数值
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

		#region ..解析数字字符串
		/// <summary>
		/// 解析数字字符串
		/// </summary>
		/// <param name="numberstr">数字字符串</param>
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
