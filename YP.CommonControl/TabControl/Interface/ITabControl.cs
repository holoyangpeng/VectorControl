using System;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.TabControl.Interface
{
	/// <summary>
	/// 定义TabControl的一般行为
	/// </summary>
	public interface ITabControl
	{
		#region ..属性
		/// <summary>
		/// 获取或设置选择项索引
		/// </summary>
		int SelectedIndex{set;get;}

		/// <summary>
		/// 获取或设置当前选择的选项卡
		/// </summary>
		ITabPage SelectedTab{set;get;}

		/// <summary>
		/// 获取或设置选项卡是否显示在顶部
		/// </summary>
		bool PositionTop{set;get;}

		/// <summary>
		/// 获取当前是否是通过MDI子窗体形式显示
		/// </summary>
		bool MDIFormMode{get;}

		/// <summary>
		/// 决定是否显示选项卡导航按钮
		/// </summary>
		bool ShowTabButton{set;get;}

		/// <summary>
		/// 获取或设置选项卡是否启用热键跟踪
		/// </summary>
		bool HotTrack{set;get;}

		/// <summary>
		/// 获取或设置选项卡的长度计算方式
		/// </summary>
		TabSizeMode SizeMode{set;get;}

		/// <summary>
		/// 获取或设置选项卡的选项条背景颜色
		/// </summary>
		Color TabColor{set;get;}

		/// <summary>
		/// 当选项卡超出维度范围时，是否显示导航按钮
		/// </summary>
		bool ShowNavigateButton{set;get;}

		/// <summary>
		/// 选项卡是否显示控制按钮
		/// </summary>
		bool ShowControlBox{set;get;}

		/// <summary>
		/// 获取TabControl的选项卡集合
		/// </summary>
		ITabPageCollection TabPages{get;}

		/// <summary>
		/// 决定TabControl是否显示IDE类型的边
		/// </summary>
		bool IDEBorder{set;get;}
		#endregion

		#region ..事件
		/// <summary>
		/// 当选项卡索引发生改变时发生
		/// </summary>
		event EventHandler SelectedIndexChanged;
		#endregion
	}
}
