using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// 实现可变换的填充对象
	/// </summary>
	public abstract class SVGPaintTransformElement:YP.SVG.SVGStyleable,Interface.GradientsAndPatterns.ISVGPaintTransformElement
	{
		#region ..构造及消除
		public SVGPaintTransformElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		public GraphicsPath paintPath = new GraphicsPath();
		public PointF[] controlPoints = new PointF[0];
		public DataType.SVGTransformList paintTransform = null;
		public Brush brush = null;
		public RectangleF preBounds = RectangleF.Empty;
		public float preFloat = 1f;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取变换对象
		/// </summary>
		public Interface.CTS.ISVGTransformList PaintTransform
		{
			get
			{
				return this.paintTransform;
			}
		}
		#endregion

		#region ..当对象填充指定路径时，获取其控制路径
		/// <summary>
		/// 当对象填充指定路径时，获取其控制路径
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public abstract GraphicsPath GetControlPath(GraphicsPath fillPath);
		#endregion

		#region ..当对象填充指定路径时，获取其控制点集
		/// <summary>
		/// 当对象填充指定路径时，获取其控制点集
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public abstract PointF[] GetControlPoints(GraphicsPath fillPath);
		#endregion

		#region ..当对象填充指定路径时，绘制画笔
		/// <summary>
		/// 当对象填充指定路径时，获取绘制画笔
		/// </summary>
		/// <param name="bounds">将要填充的路径边界</param>
		/// <param name="opacity">指定透明度</param>
        public abstract Brush GetBrush(SVG.SVGTransformableElement ownerElement, Rectangle bounds, float opacity);
		#endregion

		#region ..当对象填充指定路径时，获取其变换矩阵
		/// <summary>
		/// 当对象填充指定路径时，获取其变换矩阵
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public abstract Matrix GetTransform(GraphicsPath fillPath);
		#endregion
	}
}
