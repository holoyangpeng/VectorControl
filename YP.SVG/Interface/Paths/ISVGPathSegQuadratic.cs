using System;
using System.Drawing;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ������α���������
	/// </summary>
	public interface ISVGPathSegQuadratic:Paths.ISVGPathSegCubic
	{
		/// <summary>
		/// ��ȡ�������߿��Ƶ�
		/// </summary>
		PointF GetQuadraticControl(ISVGPathSegList svgPathList);
	}
}
