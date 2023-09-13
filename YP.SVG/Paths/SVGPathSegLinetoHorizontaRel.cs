using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// h
	/// </summary>
	public class SVGPathSegLinetoHorizontalRel:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoHorizontalRel
	{
		#region ..���켰����
		public SVGPathSegLinetoHorizontalRel(float x):base(x,0)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL;
			this.pathSegTypeAsLetter = "h";
			this.Relative = true;
		}
		#endregion

		#region ..��ȡ·���յ�
		/// <summary>
		/// ��ȡ·�����յ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList)
		{
			SVGPathSeg prevSeg = (SVGPathSeg)svgPathList.PreviousSibling(this);
			PointF prevPoint;
			if(prevSeg == null) prevPoint = new PointF(0,0);
			else prevPoint = prevSeg.GetLastPoint(svgPathList);
			return new PointF(prevPoint.X + X, prevPoint.Y);
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
				return "h" + this.X.ToString();
			}
		}
		#endregion
	}
}
