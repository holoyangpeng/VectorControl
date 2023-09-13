using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义SVG中可绘制的节点的一般行为
	/// </summary>
    public interface ISVGRenderer
	{
		/// <summary>
		/// 绘制元素
		/// </summary>
		/// <param name="g">画布</param>
		/// <param name="sp">类型容器</param>
		void Draw(System.Drawing.Graphics g,YP.SVG.StyleContainer.StyleOperator sp);
	}
}
