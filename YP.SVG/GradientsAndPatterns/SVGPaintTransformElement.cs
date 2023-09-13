using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// ʵ�ֿɱ任��������
	/// </summary>
	public abstract class SVGPaintTransformElement:YP.SVG.SVGStyleable,Interface.GradientsAndPatterns.ISVGPaintTransformElement
	{
		#region ..���켰����
		public SVGPaintTransformElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		public GraphicsPath paintPath = new GraphicsPath();
		public PointF[] controlPoints = new PointF[0];
		public DataType.SVGTransformList paintTransform = null;
		public Brush brush = null;
		public RectangleF preBounds = RectangleF.Empty;
		public float preFloat = 1f;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�任����
		/// </summary>
		public Interface.CTS.ISVGTransformList PaintTransform
		{
			get
			{
				return this.paintTransform;
			}
		}
		#endregion

		#region ..���������ָ��·��ʱ����ȡ�����·��
		/// <summary>
		/// ���������ָ��·��ʱ����ȡ�����·��
		/// </summary>
		/// <param name="fillPath">��Ҫ����·��</param>
		public abstract GraphicsPath GetControlPath(GraphicsPath fillPath);
		#endregion

		#region ..���������ָ��·��ʱ����ȡ����Ƶ㼯
		/// <summary>
		/// ���������ָ��·��ʱ����ȡ����Ƶ㼯
		/// </summary>
		/// <param name="fillPath">��Ҫ����·��</param>
		public abstract PointF[] GetControlPoints(GraphicsPath fillPath);
		#endregion

		#region ..���������ָ��·��ʱ�����ƻ���
		/// <summary>
		/// ���������ָ��·��ʱ����ȡ���ƻ���
		/// </summary>
		/// <param name="bounds">��Ҫ����·���߽�</param>
		/// <param name="opacity">ָ��͸����</param>
        public abstract Brush GetBrush(SVG.SVGTransformableElement ownerElement, Rectangle bounds, float opacity);
		#endregion

		#region ..���������ָ��·��ʱ����ȡ��任����
		/// <summary>
		/// ���������ָ��·��ʱ����ȡ��任����
		/// </summary>
		/// <param name="fillPath">��Ҫ����·��</param>
		public abstract Matrix GetTransform(GraphicsPath fillPath);
		#endregion
	}
}
