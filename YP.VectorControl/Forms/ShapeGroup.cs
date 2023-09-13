using System;
using System.Collections;
using System.Runtime.Serialization;
using YP.SVG.Interface;
using YP.SVG.Paths;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ��¼�߱�ĳһ�������Shape����
	/// </summary>
	[Serializable]
	internal class ShapeGroup:CollectionBase,IGroup
	{
		#region ..���켰����
		public ShapeGroup()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

        //protected ShapeGroup(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        //{
        //    this.name = info.GetValue("id",typeof(string)) as string;
        //    serializable = info.GetValue("childs", typeof(SVGPathElement[])) as SVGPathElement[];
        //}
		#endregion

		#region ..˽�б���
		string name = string.Empty;
		bool expand = true;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ������ָ����������Shape����
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
		/// ��ȡ���������չ��״̬
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
		/// ��ȡ�����ö����ID
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

		#region ..���ϲ���
		/// <summary>
		/// ���һ���µ�Shape����
		/// </summary>
		/// <param name="shape"></param>
        public void Add(IOutlookBarPath shape)
		{
			if(!this.List.Contains(shape))
				this.List.Add(shape);
		}

		/// <summary>
		/// ɾ��ָ����Shape����
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

		#region ISerializable ��Ա
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// TODO:  ��� ShapeGroup.GetObjectData ʵ��
			info.SetType(typeof(ShapeGroup));
			info.AddValue("id",this.name);
            SVGPathElement[] shape = new SVGPathElement[this.Count];
			this.List.CopyTo(shape,0);
			info.AddValue("childs",shape);
		}
		#endregion

		#region ..�������л�
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
