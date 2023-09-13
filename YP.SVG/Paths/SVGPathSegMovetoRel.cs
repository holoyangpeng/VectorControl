using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// m
	/// </summary>
	public class SVGPathSegMovetoRel:SVGPathSegMove,Interface.Paths.ISVGPathSegMovetoRel
	{
		#region ..���켰����
		public SVGPathSegMovetoRel(float x,float y ):base(x,y)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			this.pathSegType = (short)PathSegmentType.PATHSEG_MOVETO_REL;
			this.pathSegTypeAsLetter = "m";
			this.Relative = true;
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
				return "m" + this.X.ToString() + " " + this.Y.ToString();
			}
		}
		#endregion
	}
}
