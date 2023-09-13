using System;
using System.Collections;
using System.Collections.Generic;
using YP.SVG.Interface;


namespace YP.SVG
{
	/// <summary>
	/// 实现SVG对象集合操作
	/// </summary>
	[Serializable()]
	public class SVGElementCollection : CollectionBase
	{
		public event CollectionChangedEventHandler CollectionChanged;
		public bool NotifyEvent
        {
            set;
            get;
        }

		public bool collectionChanged = false;

        List<SVGElement> lockElements = new List<SVGElement>();

		public SVGElementCollection() 
		{

		}
		
		public SVGElementCollection(SVGElementCollection value) 
		{
			this.AddRange(value);
		}
		
		public SVGElementCollection(SVGElement[] value) 
		{
			this.AddRange(value);
		}

        /// <summary>
        /// 集合中是否包含锁定元素
        /// </summary>
        public bool ContainsLockedElement
        {
            get
            {
                return this.lockElements.Count > 0;
            }
        }


		
		public SVGElement this[int index] 
		{
			get 
			{
                return ((SVGElement)(List[index]));
			}
			set 
			{
				List[index] = value;
			}
		}

        public int Add(object value) 
		{
			if((value as SVGElement) == null || this.Contains(value))
				return -1;
            //if (value is SVGStyleable)
            //{
            //    SVGDom.ViewStyle viewStyle = (value as SVGDom.SVGStyleable).ViewStyle;
            //    if ((viewStyle & SVGDom.ViewStyle.Lock) == SVGDom.ViewStyle.Lock)
            //        return -1;
            //}
			int index = List.Add(value);
			if(this.CollectionChanged != null && this.NotifyEvent)
				this.CollectionChanged(this,new CollectionChangedEventArgs(value as SVGElement,CollectionChangeAction.Insert));
			return index;
		}

        public SVGElement[] ToArray()
        {
            SVGElement[] elements = new SVGElement[this.Count];
            this.CopyTo(elements, 0);
            return elements;
        }
		
		public void AddRange(SVGElement[] value) 
		{
			bool old = this.NotifyEvent;
			this.NotifyEvent = false;
			if(value != null)
			{
				for (int i = 0; (i < value.Length); i = (i + 1)) 
				{
					if(!this.Contains(value[i]))
						this.Add(value[i]);
				}
			}
			this.NotifyEvent = old;
			if(this.CollectionChanged != null && this.NotifyEvent)
			{
				this.CollectionChanged(this,new CollectionChangedEventArgs(value,CollectionChangeAction.Insert));
			}
		}
		
		public void AddRange(SVGElementCollection value) 
		{
			bool old = this.NotifyEvent;
			this.NotifyEvent = false;
			if(value != null)
			{
				for (int i = 0; (i < value.Count); i = (i + 1)) 
				{
					if(!this.Contains((SVGElement)value[i]))
						this.Add((SVGElement)value[i]);
				}
			}
			this.NotifyEvent = old;
			if(this.CollectionChanged != null && this.NotifyEvent)
			{
				SVGElement[] list = null;
				if(value != null)
				{
					list = new SVGElement[value.Count];
					value.CopyTo(list,0);
				}
				this.CollectionChanged(this,new CollectionChangedEventArgs(list,CollectionChangeAction.Insert));
			}
		}

        public void LockElement(SVGElement element)
        {
            if (!this.lockElements.Contains(element))
            {
                this.lockElements.Add(element);
                this.CollectionChanged(this, new CollectionChangedEventArgs(element, CollectionChangeAction.None));
            }
        }

        public void UnLockElement(SVGElement element)
        {
            if (this.lockElements.Contains(element))
            {
                this.lockElements.Remove(element);
                this.CollectionChanged(this, new CollectionChangedEventArgs(element, CollectionChangeAction.None));
            }
        }
		
		public bool Contains(object value) 
		{
			return List.Contains(value);
		}
		
		public void CopyTo(SVGElement[] array, int index) 
		{
			if(array != null)
				List.CopyTo(array, index);
		}

        public int IndexOf(object value) 
		{
			return List.IndexOf(value);
		}
		
		public void Insert(int index, SVGElement value) 
		{
			if(!this.Contains(value))
			{
				List.Insert(index, value);
				if(this.CollectionChanged != null && this.NotifyEvent)
					this.CollectionChanged(this,new CollectionChangedEventArgs(value,CollectionChangeAction.Insert));
			}
		}

        public void InsertRange(int index, SVGElementCollection elements)
        {
            bool old = this.NotifyEvent;
            this.NotifyEvent = false;
            if (elements != null)
            {
                for (int i = elements.Count - 1; i >= 0; i --)
                {
                    if (!this.Contains(elements[i]))
                        this.Insert(index, elements[i]);
                }
            }
            this.NotifyEvent = old;
            if (this.CollectionChanged != null && this.NotifyEvent)
            {
                this.CollectionChanged(this, new CollectionChangedEventArgs(elements.ToArray(), CollectionChangeAction.Insert));
            }
        }

		public SVGElementCollection Clone()
		{
			SVGElementCollection temp = new SVGElementCollection();
			temp.AddRange(this);
			return temp;
		}

        public void Remove(object value)
        {
            if (value != null)
            {
                List.Remove(value);
                if (this.CollectionChanged != null && value is SVGElement)
                    this.CollectionChanged(this, new CollectionChangedEventArgs(value as SVGElement, CollectionChangeAction.Remove));
            }
        }

		public new void RemoveAt(int index)
		{
			SVGElement changeelement = null;
			if(index >=0 && index < this.Count)
			{
				changeelement = (SVGElement)List[index];
			}
			List.RemoveAt (index);
			if(this.CollectionChanged != null)
				this.CollectionChanged(this,new CollectionChangedEventArgs(changeelement,CollectionChangeAction.Remove));
		}

		public new void Clear()
		{
			if(this.Count > 0)
			{
				SVGElement[] list = new SVGElement[this.Count];
				this.CopyTo(list,0);
				List.Clear();
				if(this.CollectionChanged != null && this.NotifyEvent)
				{
					this.CollectionChanged(this,new CollectionChangedEventArgs(list,CollectionChangeAction.Remove));
				}
			}
		}

		#region ..override on..
        /// <summary>
        /// 当清除 System.Collections.CollectionBase 实例的内容时执行其他自定义进程。
        /// </summary>
		protected override void OnClearComplete()
		{
			base.OnClear ();
            this.lockElements.Clear();
			collectionChanged = true;
		}

        /// <summary>
        /// 当在 System.Collections.CollectionBase 实例中设置值后执行其他自定义进程。
        /// </summary>
        /// <param name="index">从零开始的索引，可在该位置找到 oldValue</param>
        /// <param name="oldValue">要用 newValue 替换的值。</param>
        /// <param name="newValue">index 处的元素的新值</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete (index, oldValue, newValue);
            if (this.lockElements.Contains(oldValue as SVGElement))
                this.lockElements.Remove(oldValue as SVGElement);

            if (newValue is SVGStyleable && ((newValue as SVGStyleable).ViewStyle & ViewStyle.Lock) == ViewStyle.Lock)
                this.lockElements.Add(newValue as SVGStyleable);
            collectionChanged = true;
		}


        /// <summary>
        /// 在从 System.Collections.CollectionBase 实例中移除元素之后执行其他自定义进程。
        /// </summary>
        /// <param name="index">从零开始的索引，可在该位置找到 value。</param>
        /// <param name="value">要从 index 移除的元素的值</param>
        protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete (index, value);
            if (this.lockElements.Contains(value as SVGElement))
                this.lockElements.Remove(value as SVGElement);
			collectionChanged = true;
		}

        /// <summary>
        /// 在向 System.Collections.CollectionBase 实例中插入新元素之后执行其他自定义进程
        /// </summary>
        /// <param name="index">从零开始的索引，在该处插入 value</param>
        /// <param name="value">index 处的元素的新值</param>
        protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete (index, value);
            if (value is SVGStyleable && ((value as SVGStyleable).ViewStyle & ViewStyle.Lock) == ViewStyle.Lock)
                lockElements.Add(value as SVGStyleable);
			collectionChanged = true;
		}
		#endregion

        #region ..GetEnumerator
        public new ISVGElementEnumerator GetEnumerator() 
		{
			return new ISVGElementEnumerator(this);
		}
				
		public class ISVGElementEnumerator : object, IEnumerator 
		{
			
			private IEnumerator baseEnumerator;
			
			private IEnumerable temp;
			
			public ISVGElementEnumerator(CollectionBase mappings) 
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = ((CollectionBase)temp).GetEnumerator();
			}
			
			public SVGElement Current 
			{
				get 
				{
					return ((SVGElement)(baseEnumerator.Current));
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
