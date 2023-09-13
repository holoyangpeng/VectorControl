using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// V
	/// </summary>
	public class SVGPathSegLinetoVerticalAbs:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoVerticalAbs
	{
		#region ..���켰����
		public SVGPathSegLinetoVerticalAbs(float y):base(0,y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS;
			this.pathSegTypeAsLetter = "V";
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
			return new PointF(prevPoint.X, Y);
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
				return "V" + this.Y.ToString();
			}
		}
		#endregion
	}
}
