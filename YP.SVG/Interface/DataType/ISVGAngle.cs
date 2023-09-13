using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义角度的一般行为
	/// </summary>
	public interface ISVGAngle:ISVGType
	{
		/// <summary>
		/// 获取角度的类型
		/// </summary>
		AngleType UnitType{get;}

		/// <summary>
		/// 获取用浮点数表达的角度值，度量类型采用度
		/// </summary>
		float Value{get;set;}

		/// <summary>
		/// 获取用浮点数表达的角度值，度量类型采用本来的角度类型
		/// </summary>
		float ValueInSpecifiedUnits{get;set;}

		/// <summary>
		/// 获取表示角度值的字符串
		/// </summary>
		string ValueAsString{get;set;}

		/// <summary>
		/// 重设角度值，用指定的角度类型和指定的值
		/// </summary>
		/// <param name="unitType">角度类型</param>
		/// <param name="angleValue">浮点数，表示角度值</param>
		void NewValueSpecifiedUnits(AngleType unitType, float angleValue);
		
		/// <summary>
		/// 将角度转化为特定的角度类型
		/// </summary>
		/// <param name="unitType">需转换的角度类型</param>
		void ConvertToSpecifiedUnits(AngleType unitType);
	}
}
