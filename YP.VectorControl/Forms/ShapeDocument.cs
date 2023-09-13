using System;
using System.Runtime.Serialization;
using YP.SVG.Paths;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ShapeDocument 的摘要说明。
	/// </summary>
	[Serializable]
	internal class ShapeDocument:DisposeBase,System.Runtime.Serialization.ISerializable,System.IDisposable
	{
		#region ..构造及消除
		public ShapeDocument(string filepath,bool symbol)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            SVG.Document.SVGDocument doc = SVG.Document.SvgDocumentFactory.CreateSimpleDocumentFromFile(filepath);
			System.Xml.XmlNodeList list = doc.GetElementsByTagName("group",doc.NamespaceURI);
			System.Collections.ArrayList list1 = new System.Collections.ArrayList();
			if(list != null && list.Count >0)
			{
				for(int i = 0;i<list.Count;i++)
				{
					System.Xml.XmlElement element = list[i] as System.Xml.XmlElement;
					if(element == null)
						continue;
					string id = element.GetAttribute("id");
					if(id.Trim().Length >0)
					{
						ShapeGroup group = new ShapeGroup();
						group.ID = id.Trim();

						System.Xml.XmlNodeList childs = element.GetElementsByTagName("path",element.NamespaceURI);
						if(childs != null )
						{
							for(int j = 0;j<childs.Count;j++)
							{
								SVGPathElement child = childs[j] as SVGPathElement;
								string id1 = child.GetAttribute("id",child.NamespaceURI);
								if(id1.Trim().Length > 0)
								{
										group.Add(child as SVGPathElement);
								}
								id1 = null;
							}
							if(group.Count > 0)
								list1.Add(group);
						}
					}
				}
			}
			if(list1.Count > 0)
			{
				this.groups = new ShapeGroup[list1.Count];
				list1.CopyTo(this.groups);
			}
			list1 = null;
			list = null;
			doc = null;
		}

		protected ShapeDocument(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			groups = info.GetValue("groups",typeof(ShapeGroup[])) as ShapeGroup[];
		}
		#endregion

		#region ..私有变量
		ShapeGroup[] groups = null;
		#endregion

		#region ISerializable 成员

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// TODO:  添加 ShapeDocument.GetObjectData 实现
			info.SetType(typeof(ShapeDocument));
			if(this.groups != null && this.groups.Length > 0)
				info.AddValue("groups",groups);
		}

		#endregion

		#region ..ShapeGroup
		/// <summary>
		/// 获取文档中的组的集合
		/// </summary>
		internal ShapeGroup[] Shapes
		{
			get
			{
				return this.groups;
			}
		}
		#endregion

		#region IDisposable 成员

		public override void Dispose()
		{
			// TODO:  添加 ShapeDocument.Dispose 实现
			this.groups = null;
			base.Dispose();
		}

		#endregion
	}
}
