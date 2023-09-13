using System;
using System.Drawing;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// �������α�����һ����Ϊ
	/// </summary>
	public interface ISVGPathSegCubic:ISVGPathSeg
	{
		/// <summary>
		/// ��ȡ��һ�����Ƶ�
		/// </summary>
		/// <param name="svgPathSegList">·����Ԫ���ڵĵ�Ԫ�б�</param>
		PointF GetFirstControl(ISVGPathSegList svgPathSegList);

		/// <summary>
		/// ��ȡ�ڶ������Ƶ�
		/// </summary>
		PointF GetSecondControl(ISVGPathSegList svgPathSegList);
	}
}
