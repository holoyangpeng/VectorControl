using System;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ�ֽǶȲ���
	/// </summary>
	public struct SVGAngle:Interface.DataType.ISVGAngle
	{
		static Regex reAngle = new Regex(@"^(?<value>[\+\-]?(\d+\.)?\d+)(?<unit>deg|grad|rad)?$");

		#region ..˽�б���
		float valueInSpecifiedUnits;
		AngleType unitType;
		string defaultValue;
		#endregion

		#region ..��̬����
		static SVGAngle angle = new SVGAngle();

		public static SVGAngle Empty
		{
			get
			{
				return angle;
			}
		}
		#endregion

		#region ..��������
		public bool IsEmpty
		{
			get
			{
				return this.UnitType == AngleType.SVG_ANGLETYPE_UNKNOWN;
			}
		}

		/// <summary>
		/// ��ȡ�����ýǶ�����
		/// </summary>
		public AngleType UnitType
		{
			get
			{
				return this.unitType;
			}
		}

		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ö�
		/// </summary>
		public float Value
		{
			get
			{
				float ret = 0;
				switch(UnitType)
				{
					case AngleType.SVG_ANGLETYPE_DEG:
					case AngleType.SVG_ANGLETYPE_UNSPECIFIED:
						ret = valueInSpecifiedUnits;
						break;
					case AngleType.SVG_ANGLETYPE_GRAD:
						ret = valueInSpecifiedUnits * 180/200;
						break;
					case AngleType.SVG_ANGLETYPE_RAD:
						ret = valueInSpecifiedUnits * 180/(float)Math.PI;
						break;
				}
				return ret;
			}
			set
			{
				switch(UnitType)
				{
					case AngleType.SVG_ANGLETYPE_DEG:
					case AngleType.SVG_ANGLETYPE_UNSPECIFIED:
						valueInSpecifiedUnits = value;
						break;
					case AngleType.SVG_ANGLETYPE_GRAD:
						valueInSpecifiedUnits = value * 200/180;
						break;
					case AngleType.SVG_ANGLETYPE_RAD:
						valueInSpecifiedUnits = (float)Math.PI/180;
						break;
				}
			}
		}

		/// <summary>
		/// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ñ����ĽǶ�����
		/// </summary>
		public float ValueInSpecifiedUnits
		{
			get
			{
				return this.valueInSpecifiedUnits;
			}
			set
			{
				this.valueInSpecifiedUnits = value;
			}
		}

		/// <summary>
		/// ��ȡ�Ƕ�ֵ���ַ������
		/// </summary>
		public string ValueAsString
		{
			get
			{
				string ret = valueInSpecifiedUnits.ToString();
				switch(UnitType)
				{
					case AngleType.SVG_ANGLETYPE_DEG:
					case AngleType.SVG_ANGLETYPE_UNSPECIFIED:
						ret += "deg";
						break;
					case AngleType.SVG_ANGLETYPE_GRAD:
						ret += "grad";
						break;
					case AngleType.SVG_ANGLETYPE_RAD:
						ret += "rad";
						break;
				}
				return ret;
			}
			set
			{
			}
		}
		#endregion

		#region ..���켰���� 
		public SVGAngle(string s, string defaultValue)
		{
			if(s.Trim().Length == 0)
				s = defaultValue;
			this.defaultValue = defaultValue;
			this.valueInSpecifiedUnits = 0;
			this.unitType = YP.SVG.AngleType.SVG_ANGLETYPE_UNKNOWN;
			ParseAngle(s);	
		}
		#endregion

		#region ..NewValueSpecifiedUnits
		/// <summary>
		/// ����Ƕ�ֵ����ָ���ĽǶ����ͺ�ָ����ֵ
		/// </summary>
		/// <param name="unitType">�Ƕ�����</param>
		/// <param name="angleValue">����������ʾ�Ƕ�ֵ</param>
		public void NewValueSpecifiedUnits (AngleType unitType, float angleValue)
		{
			this.unitType = unitType;
            this.valueInSpecifiedUnits = angleValue;
		}
		#endregion
		
		#region ..ConvertToSpecifiedUnits
		/// <summary>
		/// ���Ƕ�ת��Ϊ�ض��ĽǶ�����
		/// </summary>
		/// <param name="unitType">��ת���ĽǶ�����</param>
		public void ConvertToSpecifiedUnits(AngleType unitType)
		{
			switch(UnitType)
			{
				case AngleType.SVG_ANGLETYPE_DEG:
				case AngleType.SVG_ANGLETYPE_UNSPECIFIED:
					if(unitType == AngleType.SVG_ANGLETYPE_GRAD) valueInSpecifiedUnits *= 400/360;
					else if(unitType == AngleType.SVG_ANGLETYPE_RAD) valueInSpecifiedUnits *= (float)Math.PI/180;
					break;
				case AngleType.SVG_ANGLETYPE_GRAD:
					if(unitType == AngleType.SVG_ANGLETYPE_DEG ||
						unitType == AngleType.SVG_ANGLETYPE_UNSPECIFIED) valueInSpecifiedUnits *= 360/400;
					else if(unitType == AngleType.SVG_ANGLETYPE_RAD) valueInSpecifiedUnits *= (float)Math.PI/200;
					break;
				case AngleType.SVG_ANGLETYPE_RAD:
					if(unitType == AngleType.SVG_ANGLETYPE_DEG ||
						unitType == AngleType.SVG_ANGLETYPE_UNSPECIFIED) valueInSpecifiedUnits *= 180 / (float)Math.PI;
					else if(unitType == AngleType.SVG_ANGLETYPE_GRAD) valueInSpecifiedUnits *= 200/(float)Math.PI;
					break;
			}
			this.unitType = unitType;
		}
		#endregion

		#region ..�����ַ���
		/// <summary>
		/// �����ַ���
		/// </summary>
		/// <param name="anglestr">�Ƕ��ַ���</param>
		void ParseAngle(string anglestr)
		{
			Match match = reAngle.Match(anglestr.Trim());
			if(match.Success)
			{
				AngleType unit;
				switch(match.Groups["unit"].Value)
				{
					case "grad":
						unit = AngleType.SVG_ANGLETYPE_GRAD;
						break;
					case "rad":
						unit = AngleType.SVG_ANGLETYPE_RAD;
						break;
					case "deg":
						unit = AngleType.SVG_ANGLETYPE_DEG;
						break;
					default:
						unit = AngleType.SVG_ANGLETYPE_UNSPECIFIED;
						break;
				}
				this.unitType = unit;
				this.valueInSpecifiedUnits = SVGNumber.ParseNumberStr(match.Groups["value"].Value);
			}
			match = null;
		}
		#endregion

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ValueAsString;
		}
		#endregion
	}
}
