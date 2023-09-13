using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����ISVGRect ��һ����Ϊ
	/// </summary>
	public interface ISVGRect:ISVGType
	{
		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�X����ֵ
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�Y����ֵ
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�Width����ֵ
		/// </summary>
		float Width{set;get;}

		/// <summary>
		/// ��ȡ������ָ��Ԫ�ص�Height����ֵ
		/// </summary>
		float Height{set;get;}

        /// <summary>
        /// ��ȡ��GDI���νṹ���ı߽�
        /// </summary>
        System.Drawing.RectangleF GDIRect { get; }
	}
}
