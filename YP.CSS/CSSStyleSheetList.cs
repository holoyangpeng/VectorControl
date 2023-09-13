using System;
using System.Collections;
using System.Collections.Specialized;

namespace YP.Base.CSS
{
	/// <summary>
	/// ʵ��CSSStyleSheet�ļ���
	/// </summary>
	public class CSSStyleSheetList:CollectionBase,Base.StyleSheets.IStyleSheetList 
	{
		#region ..���������
		public CSSStyleSheetList()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..private fields
		Hashtable names = new Hashtable();
		Hashtable elements = new Hashtable();
		Hashtable uries = new Hashtable();
		#endregion

		#region ..��ȡָ������������
		/// <summary>
		/// ��ȡ������ָ������������
		/// </summary>
		public Base.StyleSheets.IStyleSheet this[int index]
		{
			get
			{
				return (Base.StyleSheets.IStyleSheet)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
		#endregion

		#region ..�������
		/// <summary>
		/// �������
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="type"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		public Base.StyleSheets.IStyleSheet Add(Uri uri,string type,string title)
		{
			Base.StyleSheets.IStyleSheet sheet = new CSSStyleSheet(uri,type,title);
			List.Add (sheet);
			return sheet;
		}

		/// <summary>
		/// �������
		/// </summary>
		/// <param name="element">һ��WebElement����ͨ���ö������StyleSheet</param>
		/// <returns></returns>
		public Base.StyleSheets.IStyleSheet Add(Base.Interface.IWebElement element)
		{
			if(this.elements.ContainsKey(element))
				return (Base.StyleSheets.IStyleSheet)this.elements[element];
			CSS.CSSStyleSheet sheet = new CSS.CSSStyleSheet(element);
			List.Add (sheet);
			this.elements.Add(element,sheet);
			return sheet;
		}
		#endregion

		#region ..�жϼ����Ƿ����ָ����
		/// <summary>
		/// �жϼ����Ƿ����ָ����
		/// </summary>
		/// <param name="sheet"></param>
		/// <returns></returns>
		public bool Contains(Base.StyleSheets.IStyleSheet sheet)
		{
			return List.Contains(sheet);
		}
		#endregion

		#region ..����
		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="lines">Ŀ������</param>
		/// <param name="index">��ʼ����</param>
		public void CopyTo(Base.StyleSheets.IStyleSheet[] sheets,int index)
		{
			List.CopyTo(sheets,index);
		}
		#endregion

		#region ..��ȡ��������
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		/// <param name="line">�����е���</param>
		/// <returns></returns>
		public int IndexOf(Base.StyleSheets.IStyleSheet line)
		{
			return List.IndexOf(line);
		}
		#endregion

		#region ..ɾ��
		/// <summary>
		/// ɾ��ָ���������Ķ���
		/// </summary>
		/// <param name="index"></param>
		public new void RemoveAt(int index)
		{
			if(index >= 0 && index < List.Count)
			{
				List.RemoveAt(index);
			}
		}

		/// <summary>
		/// ɾ��ָ�����󴴽���StyleSheet
		/// </summary>
		/// <param name="element"></param>
		public void Remove(Base.Interface.IWebElement element)
		{
			if(this.elements.ContainsKey(element))
			{
				Base.StyleSheets.IStyleSheet sheet = (Base.StyleSheets.IStyleSheet)this.elements[element];
				List.Remove(sheet);
				this.elements.Remove(element);
			}
		}
		#endregion

		#region ..��ȡ��ѭ�����ʼ��ϵ�ö����
		/// <summary>
		/// ��ȡ��ѭ�����ʼ��ϵ�ö����
		/// </summary>
		/// <returns></returns>
		public new StyleSheetEnumerator GetEnumerator()
		{
			return new StyleSheetEnumerator(this);
		}
		#endregion

		#region ..�з���ö��
		public class StyleSheetEnumerator :object, IEnumerator 
		{
			private IEnumerator baseEnumerator;
            
			private IEnumerable temp;
            
			public StyleSheetEnumerator(CSSStyleSheetList mappings) 
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
            
			public Base.StyleSheets.IStyleSheet Current 
			{
				get 
				{
					return ((Base.StyleSheets.IStyleSheet)(baseEnumerator.Current));
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
