using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义ISVGColor的一般行为
	/// </summary>
	public interface ISVGColor:ISVGType
	{
		/// <summary>
		/// 获取颜色类型
		/// </summary>
		ColorType ColorType{get;}

		/// <summary>
		/// 获取用RGBColor色彩空间的颜色值
		/// </summary>
		IRgbColor RgbColor{get;}

		/// <summary>
		/// 获取ICCColor颜色空间内的颜色值
		/// </summary>
		ISVGIccColor IccColor{get;}

//		void SetRGBColor (string rgbColor);
//		void SetRGBColorICCColor ( string rgbColor, string iccColor );
//		void SetColor ( ColorType colorType, string rgbColor, string iccColor );
	}
}
