using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现SVG中浮点操作
	/// </summary>
	public struct SVGNumber:Interface.DataType.ISVGNumber
	{
		#region ..静态变量
		public static string NumberPattern = @"(?<number>(\+|-)?\d*\.?\d+((e|E)(\+|-)?\d+)?)";
		public static Regex reNumber = new Regex("^" + NumberPattern + "$");
		#endregion

		#region ..构造及消除
		public SVGNumber(float floatvalue)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.floatvalue = floatvalue;
			this.valuestr = floatvalue.ToString();
			this.defaultValue = this.valuestr;
			this.isEmpty = false;
		}

		public SVGNumber(string numberstr,string defaultValue)
		{
			this.isEmpty = false;
			
			this.defaultValue = defaultValue;
			if(numberstr.Trim().Length == 0)
				numberstr = this.defaultValue;
			this.floatvalue = 0;
			this.valuestr = numberstr;
			try
			{
				this.floatvalue = SVGNumber.ParseNumberStr(numberstr);
			}
			catch
			{
			}
		}
		#endregion

		#region ..静态变量
		static SVGNumber number = new SVGNumber();

		public static SVGNumber Empty
		{
			get
			{
				number.isEmpty = true;
				return number;
			}
		}
		#endregion

		#region ..私有变量
		float floatvalue;
		bool isEmpty;
		string valuestr ;
		string defaultValue;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置用浮点值表达的值
		/// </summary>
		public float Value
		{
			get
			{
				return this.floatvalue;
			}
			set
			{
				this.floatvalue = value;
			}
		}
		
		/// <summary>
		/// 判断对象是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		/// <summary>
		/// 获取对象的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// 获取构造此长度的原始字符串
		/// </summary>
		public string OriValueStr
		{
			get
			{
				return this.valuestr;
			}
		}
		#endregion

		#region ..解析数字字符串
		/// <summary>
		/// 解析数字字符串
		/// </summary>
		/// <param name="numberstr">数字字符串</param>
		/// <returns></returns>
		public static float ParseNumberStr(string numberstr)
		{
			if(numberstr.Length > 0 &&string.Compare(numberstr,"none") != 0 &&string.Compare(numberstr,"inherit") != 0)
			{
				NumberFormatInfo format = new NumberFormatInfo();
				format.NumberDecimalSeparator = ".";

				float val;
				val = Single.Parse(numberstr,System.Globalization.NumberStyles.Any,format);
				format = null;
				return (float)Math.Round(val,6);
			}
			else
				return 0;
		}
		#endregion

		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Value.ToString();
		}
		#endregion
	}
}
