using System;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl.Interface
{
	/// <summary>
	/// 定义TabPage的一般行为
	/// </summary>
	public interface ITabPage
	{
		/// <summary>
		/// 控制或设置TabPage所显示的控件
		/// </summary>
		Control Control{set;get;}

		/// <summary>
		/// 获取或设置TabPage可见性
		/// </summary>
		bool Visible{set;get;}

		/// <summary>
		/// 获取或设置TabPage的图像索引
		/// </summary>
		System.Drawing.Image Image{set;get;}

		/// <summary>
		/// 获取或设置TabPage的标题
		/// </summary>
		string Text{set;get;}
	}
}
