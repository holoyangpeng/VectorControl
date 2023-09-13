using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 定义数据列表
	/// </summary>
	public abstract class SVGTypeList:DataType.SVGType,Interface.DataType.ISVGTypeList
	{
		#region ..构造及消除
		public SVGTypeList()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		public System.Collections.ArrayList list = new System.Collections.ArrayList();
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取列表数目
		/// </summary>
		public int NumberOfItems
		{
			get
			{
				return this.list.Count;
			}
		}
		#endregion

		#region ..公共方法
		/// <summary>
		/// 清空列表项
		/// </summary>
		public virtual void Clear()
		{
			this.list.Clear();
		}

		/// <summary>
		/// 清空当前列表项，并用指定的Interface.DataType.ISVGType初始化列表
		/// </summary>
		public virtual Interface.DataType.ISVGType Initialize(Interface.DataType.ISVGType newItem)
		{
			this.list.Clear();
			return this.AppendItem(newItem);
		}

		/// <summary>
		/// 获取索引处的值
		/// </summary>
		public virtual Interface.DataType.ISVGType GetItem(int index)
		{
			if(index >= 0 && (int)index < this.list.Count)
				return (Interface.DataType.ISVGType)this.list[(int)index];
			return null;
		}

		/// <summary>
		/// 在指定的索引处插入SvgNumber项
		/// </summary>
		public virtual Interface.DataType.ISVGType InsertItemBefore(Interface.DataType.ISVGType newItem, int index)
		{
			if(index >= 0 && this.ValidType(newItem))
			{
				if((int)index < this.list.Count)
					this.list.Insert((int)index,newItem);
				else
					this.list.Add(newItem);
				return newItem;
			}
			return null;
		}

		/// <summary>
		/// 用指定的Interface.DataType.ISVGType替换指定索引处的项
		/// </summary>
		public virtual Interface.DataType.ISVGType ReplaceItem(Interface.DataType.ISVGType newItem, int index)
		{
			if(index>= 0 && (int)index < this.list.Count && this.ValidType(newItem))
			{
				this.list.RemoveAt((int)index);
				this.list.Insert((int)index,newItem);
				return newItem;
			}
			return null;
		}

		/// <summary>
		/// 移除指定索引处的项
		/// </summary>
		public virtual Interface.DataType.ISVGType RemoveItem(int index)
		{
			if(index>= 0 && (int)index < this.list.Count)
			{
				Interface.DataType.ISVGType length = (Interface.DataType.ISVGType)this.list[(int)index];
				this.list.Remove(length);
				return length;
			}
			return null;
		}

		/// <summary>
		/// 在列表末尾添加Interface.DataType.ISVGType项
		/// </summary>
		public virtual Interface.DataType.ISVGType AppendItem(Interface.DataType.ISVGType newItem)
		{
			if(this.ValidType(newItem))
			{
				this.list.Add(newItem);
				return newItem;
			}
			return null;
		}

		/// <summary>
		/// 确定某个元素是否在列表中
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public virtual bool Contains(object child)
		{
			return this.list.Contains (child);
		}
		#endregion

		#region ..检测是否为有效的数据值
		/// <summary>
		/// 检测是否为有效的数据值
		/// </summary>
		/// <param name="svgType">检测的数组</param>
		/// <returns></returns>
		public abstract bool ValidType(Interface.DataType.ISVGType svgType);
		#endregion

		#region ..获取索引
		/// <summary>
		/// 返回指定项在列表中的索引
		/// </summary>
		/// <param name="svgType"></param>
		/// <returns></returns>
		public virtual int IndexOf(Interface.DataType.ISVGType svgType)
		{
			return this.list.IndexOf(svgType);
		}
		#endregion
	}
}
