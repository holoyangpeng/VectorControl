using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	///��һ����t
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticSmoothRel:ISVGPathSeg
	{/// <summary>
		/// �յ���Ժ�����
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// �յ����������
		/// </summary>
		float Y{set;get;}
	}
}
