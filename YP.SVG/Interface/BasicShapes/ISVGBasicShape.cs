using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// ���������״��һ����Ϊ
	/// </summary>
	public interface ISVGBasicShape
	{
		/// <summary>
		/// ת��Ϊ·������
		/// </summary>
		YP.SVG.Interface.Paths.ISVGPathElement ConvertToPath();
	}
}
