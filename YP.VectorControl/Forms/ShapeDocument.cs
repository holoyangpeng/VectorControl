using System;
using System.Runtime.Serialization;
using YP.SVG.Paths;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ShapeDocument ��ժҪ˵����
	/// </summary>
	[Serializable]
	internal class ShapeDocument:DisposeBase,System.Runtime.Serialization.ISerializable,System.IDisposable
	{
		#region ..���켰����
		public ShapeDocument(string filepath,bool symbol)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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

		#region ..˽�б���
		ShapeGroup[] groups = null;
		#endregion

		#region ISerializable ��Ա

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// TODO:  ��� ShapeDocument.GetObjectData ʵ��
			info.SetType(typeof(ShapeDocument));
			if(this.groups != null && this.groups.Length > 0)
				info.AddValue("groups",groups);
		}

		#endregion

		#region ..ShapeGroup
		/// <summary>
		/// ��ȡ�ĵ��е���ļ���
		/// </summary>
		internal ShapeGroup[] Shapes
		{
			get
			{
				return this.groups;
			}
		}
		#endregion

		#region IDisposable ��Ա

		public override void Dispose()
		{
			// TODO:  ��� ShapeDocument.Dispose ʵ��
			this.groups = null;
			base.Dispose();
		}

		#endregion
	}
}
