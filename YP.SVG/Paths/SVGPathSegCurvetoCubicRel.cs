using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// c
	/// </summary>
	public class SVGPathSegCurvetoCubicRel:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoCubicRel,Interface.Paths.ISVGPathSegCubic
	{
		#region ..���켰����
		public SVGPathSegCurvetoCubicRel(float x,float y,float x1,float y1,float x2,float y2):base(x,y,x1,y1,x2,y2)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegTypeAsLetter = "c";
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_CUBIC_REL;
			this.Relative = true;
		}
		#endregion

		#region ..��ȡ��һ�����Ƶ�
		/// <summary>
		/// ��ȡ��һ�����Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetFirstControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			System.Drawing.PointF p = System.Drawing.PointF.Empty;
			Interface.Paths.ISVGPathSeg pre = svgPathList.PreviousSibling(this);
			if(pre != null)
				p = pre.GetLastPoint(svgPathList);
			return new System.Drawing.PointF(p.X + this.X1,p.Y + this.Y1);
		}
		#endregion

		#region ..��ȡ�ڶ������Ƶ�
		/// <summary>
		/// ��ȡ�ڶ������Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetSecondControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			System.Drawing.PointF p = System.Drawing.PointF.Empty;
			Interface.Paths.ISVGPathSeg pre = svgPathList.PreviousSibling(this);
			if(pre != null)
				p = pre.GetLastPoint(svgPathList);
			return new System.Drawing.PointF(p.X + this.X2,p.Y + this.Y2);
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ·�����ݵ��ı����
		/// </summary>
		public override string PathString
		{
			get
			{
				return "c" + this.X1.ToString() + " " + this.Y1.ToString() + " " + this.X2.ToString() + " " + this.Y2.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
