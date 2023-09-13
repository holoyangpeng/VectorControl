using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ʵ������A
	/// </summary>
	public class SVGPathSegArcAbs:SVGPathSegArc,Interface.Paths.ISVGPathSegArcAbs
	{
		#region ..���켰����
		public SVGPathSegArcAbs(float x,float y,float r1,float r2,float angle,bool largeArcFlag,bool sweepFlag):base(x,y,r1,r2,angle,largeArcFlag,sweepFlag)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegTypeAsLetter = "A";
			this.pathSegType = (short)PathSegmentType.PATHSEG_ARC_ABS;
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
				return "A" + this.R1.ToString() + " " + this.R2.ToString() + " " + this.Angle + " " + (this.LargeArcFlag?"1":"0") + " " + (this.SweepFlag?"1":"0") + " " +this.X.ToString() + " " + this.Y.ToString();
			}
		}
		#endregion
	}
}
