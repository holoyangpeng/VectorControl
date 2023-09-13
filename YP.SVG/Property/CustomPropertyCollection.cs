using System;
using System.Collections;

namespace YP.SVG.Property
{
	/// <summary>
	/// define the list to store the custom property
	/// </summary>
	internal class CustomPropertyCollection:CollectionBase
	{
		#region ..Constructor
		public CustomPropertyCollection()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		

		/// <summary>
		/// 创建一个集合,并用指定的数组初始化
		/// </summary>
		/// <param name="value">初始化数组</param>
		public CustomPropertyCollection(IProperty[] value) 
		{
			this.AddRange(value);
		}
		#endregion
		
		#region ..Index
		/// <summary>
		/// 指定或设置指定索引处的对象
		/// </summary>
		public IProperty this[int index] 
		{
			get 
			{
				return ((IProperty)(List[index]));
			}
			set 
			{
				List[index] = value;
			}
		}
		#endregion
		
		#region ..Add
		/// <summary>
		/// 将指定的对象添加到集合
		/// </summary>
		/// <param name="value">要添加的对象</param>
		/// <returns>添加后的对象在列表中的索引</returns>
		public int Add(IProperty value) 
		{
			if(value == null || this.Contains(value))
				return -1;
			int index = List.Add(value);
			return index;
		}
		#endregion
		
		#region ..AddRange
		/// <summary>
		/// 将给定的数组添加到集合
		/// </summary>
		/// <param name="value">要添加的集合</param>
		public void AddRange(IProperty[] value) 
		{
			if(value != null)
			{
				for (int i = 0; (i < value.Length); i = (i + 1)) 
				{
					if(!this.Contains(value[i]))
						this.Add(value[i]);
				}
			}
		}

		/// <summary>
		/// 将给定的数组添加到集合
		/// </summary>
		/// <param name="value">要添加的集合</param>
		public void AddRange(CustomPropertyCollection value) 
		{
			if(value != null)
			{
				for (int i = 0; (i < value.Count); i = (i + 1)) 
				{
					if(!this.Contains(value[i]))
						this.Add(value[i]);
				}
			}
		}
		#endregion

		#region ..Contains
		/// <summary>
		/// 判断集合是否包含指定的对象
		/// </summary>
		/// <param name="value">要判断的对象</param>
		/// <returns>一个bool值，指定是否包含</returns>
		public bool Contains(IProperty value) 
		{
			return List.Contains(value);
		}
		#endregion		

		#region ..CopyTo
		/// <summary>
		/// 将集合中的对象从索引处开始复制到数组中
		/// </summary>
		/// <param name="array">目标数组</param>
		/// <param name="index">目标数组中的索引</param>
		public void CopyTo(IProperty[] array, int index) 
		{
			if(array != null)
				List.CopyTo(array, index);
		}		
		#endregion

		#region ..IndexOf
		/// <summary>
		/// 判断对象在集合中的索引
		/// </summary>
		/// <param name="value">要判断的对象</param>
		/// <returns>返回索引</returns>
		public int IndexOf(IProperty value) 
		{
			return List.IndexOf(value);
		}
		#endregion
		
		#region ..Insert
		/// <summary>
		/// 将对象插入到集合中的指定索引处
		/// </summary>
		/// <param name="index">要插入的位置</param>
		/// <param name="value">插入对象</param>
		public void Insert(int index, IProperty value) 
		{
			if(!this.Contains(value))
			{
				List.Insert(index, value);
			}
		}
		#endregion

		#region ..Clone
		/// <summary>
		/// 创建对象的副本
		/// </summary>
		/// <returns></returns>
		public CustomPropertyCollection Clone()
		{
			CustomPropertyCollection temp = new CustomPropertyCollection();
			temp.AddRange(this);
			return temp;
		}
		#endregion

		#region ..Remove
		/// <summary>
		/// 从集合中删除指定对象
		/// </summary>
		/// <param name="value">要删除的对象</param>
		public void Remove(IProperty  value)
		{
			List.Remove(value);
		}
		#endregion

		#region ..GetEnumerator
		/// <summary>
		/// 获取对象的枚举对象
		/// </summary>
		/// <returns></returns>
		public new CustomPropertyEnumerator GetEnumerator() 
		{
			return new CustomPropertyEnumerator(this);
		}
		#endregion
			
		#region ..CustomPropertyEnumerator
		/// <summary>
		/// 定义CustomPropertyCollection的枚举对象
		/// </summary>
		public class CustomPropertyEnumerator : object, IEnumerator 
		{
			
			private IEnumerator baseEnumerator;
			
			private IEnumerable temp;
			
			public CustomPropertyEnumerator(CustomPropertyCollection mappings) 
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
			
			public IProperty Current 
			{
				get 
				{
					return ((IProperty)(baseEnumerator.Current));
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
