using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// v
	/// </summary>
	public class SVGPathSegLinetoVerticalRel:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoVerticalRel
	{
		#region ..���켰����
		public SVGPathSegLinetoVerticalRel(float y):base(0,y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_VERTICAL_REL;
			this.pathSegTypeAsLetter = "v";
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
			return new PointF(prevPoint.X, prevPoint.Y + Y);
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
				return "v" + this.Y.ToString();
			}
		}
		#endregion
	}
}
