using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����һ�����������󶼾߱���SVGPathSegList�Ľṹ������SVGPathElement ��SVGAnimateMotionElement
	/// </summary>
	public interface ISVGPathSegListElement:ISVGElement
	{
		/// <summary>
		/// ��ȡ��SVGPathSegList����
		/// </summary>
		Interface.Paths.ISVGPathSegList SVGPathSegList{get;}

		/// <summary>
		/// ��ȡ·��
		/// </summary>
		System.Drawing.Drawing2D.GraphicsPath SegPath{get;}
	}
}
