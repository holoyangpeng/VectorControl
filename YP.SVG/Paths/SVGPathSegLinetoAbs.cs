using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// L
	/// </summary>
	public class SVGPathSegLinetoAbs:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoAbs 
	{
		#region ..���켰����
		public SVGPathSegLinetoAbs(float x,float y):base(x,y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_ABS;
			this.pathSegTypeAsLetter = "L";
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
			return new PointF(this.X,this.Y);
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
				return "L" + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
