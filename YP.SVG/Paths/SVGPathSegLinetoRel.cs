using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// l
	/// </summary>
	public class SVGPathSegLinetoRel:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoRel
	{
		#region ..���켰����
		public SVGPathSegLinetoRel(float x,float y):base(x,y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_REL;
			this.pathSegTypeAsLetter = "l";
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
			SVGPathSeg pre = (SVGPathSeg)svgPathList.PreviousSibling(this);
			PointF p = PointF.Empty;
			if(pre != null)
			{
				p = pre.GetLastPoint(svgPathList);
			}
			return new PointF(p.X + this.X,p.Y + this.Y);
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
				return "l" + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
