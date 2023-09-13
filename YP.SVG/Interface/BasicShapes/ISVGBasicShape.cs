using System;

namespace YP.SVG.Interface.BasicShapes
{
	/// <summary>
	/// 定义基本形状的一般行为
	/// </summary>
	public interface ISVGBasicShape
	{
		/// <summary>
		/// 转换为路径对象
		/// </summary>
		YP.SVG.Interface.Paths.ISVGPathElement ConvertToPath();
	}
}
