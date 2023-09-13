using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// H
	/// </summary>
	public class SVGPathSegLinetoHorizontalAbs:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoHorizontalAbs
	{
		#region ..���켰����
		public SVGPathSegLinetoHorizontalAbs(float x):base(x,0)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS;
			this.pathSegTypeAsLetter = "H";
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
			return new PointF(X, prevPoint.Y);
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
				return "H" + this.X.ToString();
			}
		}
		#endregion
	}
}
