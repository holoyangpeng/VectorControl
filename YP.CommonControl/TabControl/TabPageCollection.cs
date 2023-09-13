using System;
using System.Collections;
using System.Collections.Specialized;

using YP.CommonControl.TabControl.Interface;

namespace YP.CommonControl.TabControl
{
	/// <summary>
	/// TabPageCollection ��ժҪ˵����
	/// </summary>
	public class TabPageCollection:Common.CollectionWithEvents,Interface.ITabPageCollection
	{
		#region ..Constructor
		public TabPageCollection(TabControl tabControl)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
		/// ��ȡ������ָ����������TabPage
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

		#region ..��������
		/// <summary>
		/// ���TabPage
		/// </summary>
		/// <param name="newTab"></param>
		public void Add(ITabPage newTab)
		{
			base.List.Add(newTab as object);
		}

		/// <summary>
		/// ��ָ��λ�ò���ѡ�
		/// </summary>
		/// <param name="index">�����������</param>
		/// <param name="newTab"></param>
		public void Insert(int index,ITabPage newTab)
		{
			base.List.Insert(index,newTab as object);
		}

		/// <summary>
		/// �Ƴ�TabPage
		/// </summary>
		/// <param name="tab"></param>
		public void Remove(ITabPage tab)
		{
			base.List.Remove(tab as object);
		}

		/// <summary>
		/// �Ƴ�ָ����������ѡ�
		/// </summary>
		/// <param name="index"></param>
		public new void RemoveAt(int index)
		{
			base.List.RemoveAt(index);
		}

		/// <summary>
		/// �жϼ����Ƿ����ָ����TabPage
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public bool Contains(ITabPage tab)
		{
			return this.List.Contains(tab);
		}

		/// <summary>
		/// ��ȡָ��TabPage�ڼ����е�����
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public int IndexOf(ITabPage tab)
		{
			return this.List.IndexOf(tab);
		}

		/// <summary>
		/// ��ȡ��ѭ�����ʼ��ϵ�ö����
		/// </summary>
		/// <returns></returns>
		public new TabPageEnumerator GetEnumerator()
		{
			return new TabPageEnumerator(this);
		}
		#endregion

		#region ..����ö��
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
