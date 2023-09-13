using System;
using System.Collections;
using System.Collections.Specialized;

using YP.CommonControl.TabControl.Interface;

namespace YP.CommonControl.TabControl
{
	/// <summary>
	/// TabPageCollection 的摘要说明。
	/// </summary>
	public class TabPageCollection:Common.CollectionWithEvents,Interface.ITabPageCollection
	{
		#region ..Constructor
		public TabPageCollection(TabControl tabControl)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.tabControl = tabControl;
		}
		#endregion

		#region ..private fields
		TabControl tabControl = null;
		Hashtable tabs = new Hashtable();
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取或设置指定索引处的TabPage
		/// </summary>
		public ITabPage this[int index]
		{
			set
			{
				this.List[index] = value;
			}
			get
			{
				return (ITabPage)this.List[index];
			}
		}
		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加TabPage
		/// </summary>
		/// <param name="newTab"></param>
		public void Add(ITabPage newTab)
		{
			base.List.Add(newTab as object);
		}

		/// <summary>
		/// 在指定位置插入选项卡
		/// </summary>
		/// <param name="index">欲插入的索引</param>
		/// <param name="newTab"></param>
		public void Insert(int index,ITabPage newTab)
		{
			base.List.Insert(index,newTab as object);
		}

		/// <summary>
		/// 移除TabPage
		/// </summary>
		/// <param name="tab"></param>
		public void Remove(ITabPage tab)
		{
			base.List.Remove(tab as object);
		}

		/// <summary>
		/// 移除指定索引处的选项卡
		/// </summary>
		/// <param name="index"></param>
		public new void RemoveAt(int index)
		{
			base.List.RemoveAt(index);
		}

		/// <summary>
		/// 判断集合是否包含指定的TabPage
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public bool Contains(ITabPage tab)
		{
			return this.List.Contains(tab);
		}

		/// <summary>
		/// 获取指定TabPage在集合中的索引
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public int IndexOf(ITabPage tab)
		{
			return this.List.IndexOf(tab);
		}

		/// <summary>
		/// 获取可循环访问集合的枚举数
		/// </summary>
		/// <returns></returns>
		public new TabPageEnumerator GetEnumerator()
		{
			return new TabPageEnumerator(this);
		}
		#endregion

		#region ..访问枚举
		public class TabPageEnumerator :object, IEnumerator 
		{
			private IEnumerator baseEnumerator;
            
			private IEnumerable temp;
            
			public TabPageEnumerator(TabPageCollection mappings) 
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
            
			public TabPage Current 
			{
				get 
				{
					return ((TabPage)(baseEnumerator.Current));
				}
			}
            
			object IEnumerator.Current 
			{
				get 
				{
					return baseEnumerator.Current;
				}
			}
            
			public bool MoveNext() 
			{
				return baseEnumerator.MoveNext();
			}
            
			bool IEnumerator.MoveNext() 
			{
				return baseEnumerator.MoveNext();
			}
            
			public void Reset() 
			{
				baseEnumerator.Reset();
			}
            
			void IEnumerator.Reset() 
			{
				baseEnumerator.Reset();
			}
		}
		#endregion
	}
}
