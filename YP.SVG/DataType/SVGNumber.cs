using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��SVG�и������
	/// </summary>
	public struct SVGNumber:Interface.DataType.ISVGNumber
	{
		#region ..��̬����
		public static string NumberPattern = @"(?<number>(\+|-)?\d*\.?\d+((e|E)(\+|-)?\d+)?)";
		public static Regex reNumber = new Regex("^" + NumberPattern + "$");
		#endregion

		#region ..���켰����
		public SVGNumber(float floatvalue)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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

		#region ..��̬����
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

		#region ..˽�б���
		float floatvalue;
		bool isEmpty;
		string valuestr ;
		string defaultValue;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�������ø���ֵ����ֵ
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
		/// �ж϶����Ƿ�Ϊ��
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		/// <summary>
		/// ��ȡ�����Ĭ��ֵ
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// ��ȡ����˳��ȵ�ԭʼ�ַ���
		/// </summary>
		public string OriValueStr
		{
			get
			{
				return this.valuestr;
			}
		}
		#endregion

		#region ..���������ַ���
		/// <summary>
		/// ���������ַ���
		/// </summary>
		/// <param name="numberstr">�����ַ���</param>
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

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Value.ToString();
		}
		#endregion
	}
}
