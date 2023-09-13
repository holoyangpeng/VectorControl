using System;
using System.Collections;
using System.Collections.Specialized;

namespace YP.Base.CSS
{
	/// <summary>
	/// 实现CSSStyleSheet的集合
	/// </summary>
	public class CSSStyleSheetList:CollectionBase,Base.StyleSheets.IStyleSheetList 
	{
		#region ..构造和消除
		public CSSStyleSheetList()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..private fields
		Hashtable names = new Hashtable();
		Hashtable elements = new Hashtable();
		Hashtable uries = new Hashtable();
		#endregion

		#region ..获取指定索引处的行
		/// <summary>
		/// 获取或设置指定索引处的行
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

		#region ..添加新项
		/// <summary>
		/// 添加新项
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
		/// 添加新项
		/// </summary>
		/// <param name="element">一个WebElement对象，通过该对象简历StyleSheet</param>
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

		#region ..判断集合是否包含指定项
		/// <summary>
		/// 判断集合是否包含指定项
		/// </summary>
		/// <param name="sheet"></param>
		/// <returns></returns>
		public bool Contains(Base.StyleSheets.IStyleSheet sheet)
		{
			return List.Contains(sheet);
		}
		#endregion

		#region ..拷贝
		/// <summary>
		/// 拷贝对象到数组
		/// </summary>
		/// <param name="lines">目标数组</param>
		/// <param name="index">开始索引</param>
		public void CopyTo(Base.StyleSheets.IStyleSheet[] sheets,int index)
		{
			List.CopyTo(sheets,index);
		}
		#endregion

		#region ..获取对象索引
		/// <summary>
		/// 获取对象索引
		/// </summary>
		/// <param name="line">集合中的行</param>
		/// <returns></returns>
		public int IndexOf(Base.StyleSheets.IStyleSheet line)
		{
			return List.IndexOf(line);
		}
		#endregion

		#region ..删除
		/// <summary>
		/// 删除指定索引处的对象
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
		/// 删除指定对象创建的StyleSheet
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

		#region ..获取可循环访问集合的枚举数
		/// <summary>
		/// 获取可循环访问集合的枚举数
		/// </summary>
		/// <returns></returns>
		public new StyleSheetEnumerator GetEnumerator()
		{
			return new StyleSheetEnumerator(this);
		}
		#endregion

		#region ..行访问枚举
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
