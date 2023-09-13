using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义ISVGRect 的一般行为
	/// </summary>
	public interface ISVGRect:ISVGType
	{
		/// <summary>
		/// 获取或设置指定元素的X属性值
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// 获取或设置指定元素的Y属性值
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// 获取或设置指定元素的Width属性值
		/// </summary>
		float Width{set;get;}

		/// <summary>
		/// 获取或设置指定元素的Height属性值
		/// </summary>
		float Height{set;get;}

        /// <summary>
        /// 获取用GDI矩形结构表达的边界
        /// </summary>
        System.Drawing.RectangleF GDIRect { get; }
	}
}
