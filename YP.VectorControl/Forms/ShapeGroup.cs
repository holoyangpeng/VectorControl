using System;
using System.Collections;
using System.Runtime.Serialization;
using YP.SVG.Interface;
using YP.SVG.Paths;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 记录具备某一个主题的Shape集合
	/// </summary>
	[Serializable]
	internal class ShapeGroup:CollectionBase,IGroup
	{
		#region ..构造及消除
		public ShapeGroup()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        //protected ShapeGroup(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        //{
        //    this.name = info.GetValue("id",typeof(string)) as string;
        //    serializable = info.GetValue("childs", typeof(SVGPathElement[])) as SVGPathElement[];
        //}
		#endregion

		#region ..私有变量
		string name = string.Empty;
		bool expand = true;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置指定索引处的Shape对象
		/// </summary>
		public IOutlookBarPath this[int index]
		{
			set
			{
				this.List[index] = value;
			}
			get
			{
				return this.List[index] as IOutlookBarPath;
			}
		}

        public object Tag { set; get; }

		/// <summary>
		/// 获取或设置组的展开状态
		/// </summary>
		public bool Expand
		{
			set
			{
				this.expand = value;
			}
			get
			{
				return this.expand;
			}
		}

		/// <summary>
		/// 获取或设置对象的ID
		/// </summary>
		public string ID
		{
			set
			{
				this.name = value;
			}
			get
			{
				return this.name;
			}
		}

		public new int Count
		{
			get
			{
//				this.FillSerializable();
				return base.Count;
			}
		}
		#endregion

		#region ..集合操作
		/// <summary>
		/// 添加一个新的Shape对象
		/// </summary>
		/// <param name="shape"></param>
        public void Add(IOutlookBarPath shape)
		{
			if(!this.List.Contains(shape))
				this.List.Add(shape);
		}

		/// <summary>
		/// 删除指定的Shape对象
		/// </summary>
		/// <param name="shape"></param>
        public void Remove(IOutlookBarPath shape)
		{
			if(this.List.Contains(shape))
				this.List.Remove(shape);
		}
		#endregion

		#region ..IndexOf
		public int IndexOf(IOutlookBarPath path)
		{
			return this.List.IndexOf(path);
		}
		#endregion

		#region ISerializable 成员
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// TODO:  添加 ShapeGroup.GetObjectData 实现
			info.SetType(typeof(ShapeGroup));
			info.AddValue("id",this.name);
            SVGPathElement[] shape = new SVGPathElement[this.Count];
			this.List.CopyTo(shape,0);
			info.AddValue("childs",shape);
		}
		#endregion

		#region ..更新序列化
//		internal void FillSerializable()
//		{
//			if(this.serializable != null)
//			{
//				for(int i = 0;i<this.serializable.Length;i++)
//				{
//					if(this.serializable[i] != null)
//						this.List.Add(this.serializable[i]);
//				}
//			}
//			this.serializable = null;
//		}
		#endregion
	}
}
