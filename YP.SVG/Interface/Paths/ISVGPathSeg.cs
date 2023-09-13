using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义一段路径构成单元的通用行为
	/// </summary>
	public interface ISVGPathSeg:Interface.DataType.ISVGType
	{
		/// <summary>
		/// 获取路径单元类别
		/// </summary>
		short PathSegType{get;}

		/// <summary>
		/// 获取路径命令
		/// </summary>
		string PathSegTypeAsLetter{get;}

		/// <summary>
		/// 获取路径数据的文本表达
		/// </summary>
		string PathString{get;}

		/// <summary>
		/// 获取路径终点
		/// </summary>
		System.Drawing.PointF GetLastPoint(ISVGPathSegList svgPathSegList);

		/// <summary>
		/// 将路径单元附加到指定路径末尾
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList);
	}
}
