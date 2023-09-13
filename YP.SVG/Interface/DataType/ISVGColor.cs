using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����ISVGColor��һ����Ϊ
	/// </summary>
	public interface ISVGColor:ISVGType
	{
		/// <summary>
		/// ��ȡ��ɫ����
		/// </summary>
		ColorType ColorType{get;}

		/// <summary>
		/// ��ȡ��RGBColorɫ�ʿռ����ɫֵ
		/// </summary>
		IRgbColor RgbColor{get;}

		/// <summary>
		/// ��ȡICCColor��ɫ�ռ��ڵ���ɫֵ
		/// </summary>
		ISVGIccColor IccColor{get;}

//		void SetRGBColor (string rgbColor);
//		void SetRGBColorICCColor ( string rgbColor, string iccColor );
//		void SetColor ( ColorType colorType, string rgbColor, string iccColor );
	}
}
