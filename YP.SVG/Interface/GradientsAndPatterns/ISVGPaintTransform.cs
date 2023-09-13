using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ����ɱ任��������
	/// </summary>
	public interface ISVGPaintTransformElement:Interface.ISVGElement
	{
		/// <summary>
		/// ��ȡ�任����
		/// </summary>
		Interface.CTS.ISVGTransformList PaintTransform{get;}

		/// <summary>
		/// ���������ָ��·��ʱ����ȡ�����·��
		/// </summary>
		/// <param name="fillPath">��Ҫ����·��</param>
		GraphicsPath GetControlPath(GraphicsPath fillPath);

		/// <summary>
		/// ���������ָ��·��ʱ����ȡ����Ƶ㼯
		/// </summary>
		/// <param name="fillPath">��Ҫ����·��</param>
		PointF[] GetControlPoints(GraphicsPath fillPath);

		/// <summary>
		/// ��ȡ���ƻ���
		/// </summary>
		/// <param name="bounds">��Ҫ����·���߽�</param>
		/// <param name="opacity">ָ��͸����</param>
		Brush GetBrush(SVG.SVGTransformableElement ownerElement,Rectangle bounds,float opacity);

		/// <summary>
		/// ���������ָ��·��ʱ����ȡ��任����
		/// </summary>
		Matrix GetTransform(GraphicsPath fillPath);
	}
}
