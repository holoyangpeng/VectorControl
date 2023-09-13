using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// 定义可变换的填充对象
	/// </summary>
	public interface ISVGPaintTransformElement:Interface.ISVGElement
	{
		/// <summary>
		/// 获取变换对象
		/// </summary>
		Interface.CTS.ISVGTransformList PaintTransform{get;}

		/// <summary>
		/// 当对象填充指定路径时，获取其控制路径
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		GraphicsPath GetControlPath(GraphicsPath fillPath);

		/// <summary>
		/// 当对象填充指定路径时，获取其控制点集
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		PointF[] GetControlPoints(GraphicsPath fillPath);

		/// <summary>
		/// 获取绘制画笔
		/// </summary>
		/// <param name="bounds">将要填充的路径边界</param>
		/// <param name="opacity">指定透明度</param>
		Brush GetBrush(SVG.SVGTransformableElement ownerElement,Rectangle bounds,float opacity);

		/// <summary>
		/// 当对象填充指定路径时，获取其变换矩阵
		/// </summary>
		Matrix GetTransform(GraphicsPath fillPath);
	}
}
