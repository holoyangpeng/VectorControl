using System;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现角度操作
	/// </summary>
	public struct SVGAngle:Interface.DataType.ISVGAngle
	{
		static Regex reAngle = new Regex(@"^(?<value>[\+\-]?(\d+\.)?\d+)(?<unit>deg|grad|rad)?$");

		#region ..私有变量
		float valueInSpecifiedUnits;
		AngleType unitType;
		string defaultValue;
		#endregion

		#region ..静态变量
		static SVGAngle angle = new SVGAngle();

		public static SVGAngle Empty
		{
			get
			{
				return angle;
			}
		}
		#endregion

		#region ..公共属性
		public bool IsEmpty
		{
			get
			{
				return this.UnitType == AngleType.SVG_ANGLETYPE_UNKNOWN;
			}
		}

		/// <summary>
		/// 获取或设置角度类型
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
		/// 获取用浮点数表达的角度值，度量类型采用度
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
		/// 获取用浮点数表达的角度值，度量类型采用本来的角度类型
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
		/// 获取角度值的字符串表达
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

		#region ..构造及消除 
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
		/// 重设角度值，用指定的角度类型和指定的值
		/// </summary>
		/// <param name="unitType">角度类型</param>
		/// <param name="angleValue">浮点数，表示角度值</param>
		public void NewValueSpecifiedUnits (AngleType unitType, float angleValue)
		{
			this.unitType = unitType;
            this.valueInSpecifiedUnits = angleValue;
		}
		#endregion
		
		#region ..ConvertToSpecifiedUnits
		/// <summary>
		/// 将角度转化为特定的角度类型
		/// </summary>
		/// <param name="unitType">需转换的角度类型</param>
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

		#region ..解析字符串
		/// <summary>
		/// 解析字符串
		/// </summary>
		/// <param name="anglestr">角度字符串</param>
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

		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ValueAsString;
		}
		#endregion
	}
}
