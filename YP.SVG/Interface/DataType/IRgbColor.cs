using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// SVG�е���ɫ
	/// </summary>
	public interface IRgbColor:ISVGType
	{
		/// <summary>
		/// ��ȡ��ɫ��Red��ɫ����ֵ
		/// </summary>
		short R{get;}

		/// <summary>
		/// ��ȡ��ɫ��Green��ɫ����ֵ
		/// </summary>
		short G{get;}

		/// <summary>
		/// ��ȡ��ɫ��Blue��ɫ����ֵ
		/// </summary>
		short B{get;}

		System.Drawing.Color GDIColor{get;}
	}
}
