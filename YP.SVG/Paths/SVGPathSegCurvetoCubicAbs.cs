using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ��������C
	/// </summary>
	public class SVGPathSegCurvetoCubicAbs:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoCubicAbs,Interface.Paths.ISVGPathSegCubic
	{
		#region ..���켰����
		public SVGPathSegCurvetoCubicAbs(float x,float y,float x1,float y1,float x2,float y2):base(x,y,x1,y1,x2,y2)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegTypeAsLetter = "C";
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS;
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
			return new System.Drawing.PointF(this.X1,this.Y1);
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
			return new System.Drawing.PointF(this.X2,this.Y2);
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
				return "C" + this.X1.ToString() + " " + this.Y1.ToString() + " " + this.X2.ToString() + " " + this.Y2.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
