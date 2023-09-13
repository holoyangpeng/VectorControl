using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// M
	/// </summary>
	public class SVGPathSegMovetoAbs:SVGPathSegMove,Interface.Paths.ISVGPathSegMovetoAbs
	{
		#region ..���켰����
		public SVGPathSegMovetoAbs(float x,float y):base(x,y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_MOVETO_ABS;
			this.pathSegTypeAsLetter = "M";
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
				return "M" + this.X.ToString() + " " + this.Y.ToString();
			}
		}
		#endregion
	}
}
