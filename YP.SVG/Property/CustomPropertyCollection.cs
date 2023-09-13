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
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		

		/// <summary>
		/// ����һ������,����ָ���������ʼ��
		/// </summary>
		/// <param name="value">��ʼ������</param>
		public CustomPropertyCollection(IProperty[] value) 
		{
			this.AddRange(value);
		}
		#endregion
		
		#region ..Index
		/// <summary>
		/// ָ��������ָ���������Ķ���
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
		/// ��ָ���Ķ�����ӵ�����
		/// </summary>
		/// <param name="value">Ҫ��ӵĶ���</param>
		/// <returns>��Ӻ�Ķ������б��е�����</returns>
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
		/// ��������������ӵ�����
		/// </summary>
		/// <param name="value">Ҫ��ӵļ���</param>
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
		/// ��������������ӵ�����
		/// </summary>
		/// <param name="value">Ҫ��ӵļ���</param>
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
		/// �жϼ����Ƿ����ָ���Ķ���
		/// </summary>
		/// <param name="value">Ҫ�жϵĶ���</param>
		/// <returns>һ��boolֵ��ָ���Ƿ����</returns>
		public bool Contains(IProperty value) 
		{
			return List.Contains(value);
		}
		#endregion		

		#region ..CopyTo
		/// <summary>
		/// �������еĶ������������ʼ���Ƶ�������
		/// </summary>
		/// <param name="array">Ŀ������</param>
		/// <param name="index">Ŀ�������е�����</param>
		public void CopyTo(IProperty[] array, int index) 
		{
			if(array != null)
				List.CopyTo(array, index);
		}		
		#endregion

		#region ..IndexOf
		/// <summary>
		/// �ж϶����ڼ����е�����
		/// </summary>
		/// <param name="value">Ҫ�жϵĶ���</param>
		/// <returns>��������</returns>
		public int IndexOf(IProperty value) 
		{
			return List.IndexOf(value);
		}
		#endregion
		
		#region ..Insert
		/// <summary>
		/// ��������뵽�����е�ָ��������
		/// </summary>
		/// <param name="index">Ҫ�����λ��</param>
		/// <param name="value">�������</param>
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
		/// ��������ĸ���
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
		/// �Ӽ�����ɾ��ָ������
		/// </summary>
		/// <param name="value">Ҫɾ���Ķ���</param>
		public void Remove(IProperty  value)
		{
			List.Remove(value);
		}
		#endregion

		#region ..GetEnumerator
		/// <summary>
		/// ��ȡ�����ö�ٶ���
		/// </summary>
		/// <returns></returns>
		public new CustomPropertyEnumerator GetEnumerator() 
		{
			return new CustomPropertyEnumerator(this);
		}
		#endregion
			
		#region ..CustomPropertyEnumerator
		/// <summary>
		/// ����CustomPropertyCollection��ö�ٶ���
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
