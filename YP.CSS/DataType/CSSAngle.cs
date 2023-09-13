using System;
using System.Text.RegularExpressions;

namespace YP.Base.CSS.DataType
{
	/// <summary>
	/// ʵ��CSS�еĽǶ�
	/// </summary>
	public class CSSAngle:Interface.DataType.ICSSAngle
	{
		static Regex reAngle = new Regex(@"^(?<value>[\+\-]?(\d+\.)?\d+)(?<unit>deg|grad|rad)?$");

		#region ..Constructor
		public CSSAngle(string anglestr)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
		/// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ö�
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
		/// ��ȡ�Ƕ�����
		/// </summary>
		public byte AngleType
		{
			get
			{
				return (byte)this.unitType;
			}
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
