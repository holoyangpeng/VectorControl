using System;

namespace YP.CommonControl.TabControl.Interface
{
	/// <summary>
	/// 定义选项卡集合
	/// </summary>
	public interface ITabPageCollection
	{
		/// <summary>
		/// 获取集合中所包含的元素数
		/// </summary>
		int Count{get;}

		/// <summary>
		/// 添加TabPage
		/// </summary>
		/// <param name="newTab"></param>
		void Add(ITabPage newTab);

		/// <summary>
		/// 在指定位置插入选项卡
		/// </summary>
		/// <param name="index">欲插入的索引</param>
		/// <param name="newTab"></param>
		void Insert(int index,ITabPage newTab);

		/// <summary>
		/// 移除TabPage
		/// </summary>
		/// <param name="tab"></param>
		void Remove(ITabPage tab);

		/// <summary>
		/// 移除指定索引处的选项卡
		/// </summary>
		/// <param name="index"></param>
		void RemoveAt(int index);

		/// <summary>
		/// 获取或设置指定索引处的TabPage
		/// </summary>
		ITabPage this[int index]{set;get;}

		/// <summary>
		/// 判断集合是否包含指定的TabPage
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		bool Contains(ITabPage tab);

		/// <summary>
		/// 获取指定TabPage在集合中的索引
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		int IndexOf(ITabPage tab);
	}
}
