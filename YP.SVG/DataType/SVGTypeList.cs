using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ���������б�
	/// </summary>
	public abstract class SVGTypeList:DataType.SVGType,Interface.DataType.ISVGTypeList
	{
		#region ..���켰����
		public SVGTypeList()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		public System.Collections.ArrayList list = new System.Collections.ArrayList();
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�б���Ŀ
		/// </summary>
		public int NumberOfItems
		{
			get
			{
				return this.list.Count;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ����б���
		/// </summary>
		public virtual void Clear()
		{
			this.list.Clear();
		}

		/// <summary>
		/// ��յ�ǰ�б������ָ����Interface.DataType.ISVGType��ʼ���б�
		/// </summary>
		public virtual Interface.DataType.ISVGType Initialize(Interface.DataType.ISVGType newItem)
		{
			this.list.Clear();
			return this.AppendItem(newItem);
		}

		/// <summary>
		/// ��ȡ��������ֵ
		/// </summary>
		public virtual Interface.DataType.ISVGType GetItem(int index)
		{
			if(index >= 0 && (int)index < this.list.Count)
				return (Interface.DataType.ISVGType)this.list[(int)index];
			return null;
		}

		/// <summary>
		/// ��ָ��������������SvgNumber��
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
		/// ��ָ����Interface.DataType.ISVGType�滻ָ������������
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
		/// �Ƴ�ָ������������
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
		/// ���б�ĩβ���Interface.DataType.ISVGType��
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
		/// ȷ��ĳ��Ԫ���Ƿ����б���
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public virtual bool Contains(object child)
		{
			return this.list.Contains (child);
		}
		#endregion

		#region ..����Ƿ�Ϊ��Ч������ֵ
		/// <summary>
		/// ����Ƿ�Ϊ��Ч������ֵ
		/// </summary>
		/// <param name="svgType">��������</param>
		/// <returns></returns>
		public abstract bool ValidType(Interface.DataType.ISVGType svgType);
		#endregion

		#region ..��ȡ����
		/// <summary>
		/// ����ָ�������б��е�����
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
