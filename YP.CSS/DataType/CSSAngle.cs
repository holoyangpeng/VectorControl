using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS.DataType
{
	/// <summary>
	/// 实现CSS中的角度
	/// </summary>
	public class CSSAngle:Interface.DataType.ICSSAngle
	{
		static Regex reAngle = new Regex(@"^(?<value>[\+\-]?(\d+\.)?\d+)(?<unit>deg|grad|rad)?$");

		#region ..Constructor
		public CSSAngle(string anglestr)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.ParseAngle(anglestr);
		}
		#endregion

		#region ..private fields
		Enum.CSSAngleType unitType = Enum.CSSAngleType.Degree;
		float valueInSpecifiedUnits = 0;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取用浮点数表达的角度值，度量类型采用度
		/// </summary>
		public float Value
		{
			get
			{
				float ret = 0;
				switch(this.unitType)
				{
					case Enum.CSSAngleType.Degree:
					case Enum.CSSAngleType.UnExpected:
						ret = valueInSpecifiedUnits;
						break;
					case Enum.CSSAngleType.Grad:
						ret = valueInSpecifiedUnits * 180/200;
						break;
					case Enum.CSSAngleType.Rad:
						ret = valueInSpecifiedUnits * 180/(float)Math.PI;
						break;
				}
				return ret;
			}
		}

		/// <summary>
		/// 获取角度类型
		/// </summary>
		public byte AngleType
		{
			get
			{
				return (byte)this.unitType;
			}
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
				Enum.CSSAngleType unit;
				switch(match.Groups["unit"].Value)
				{
					case "grad":
						unit = Enum.CSSAngleType.Grad;
						break;
					case "rad":
						unit = Enum.CSSAngleType.Rad;
						break;
					case "deg":
						unit = Enum.CSSAngleType.Degree;
						break;
					default:
						unit = Enum.CSSAngleType.UnExpected;
						break;
				}
				this.unitType = unit;
				this.valueInSpecifiedUnits = CSSNumber.ParseNumberStr(match.Groups["value"].Value);
			}
		}
		#endregion
	}
}
