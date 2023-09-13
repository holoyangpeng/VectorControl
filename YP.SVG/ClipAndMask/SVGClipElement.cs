using System;
using System.Xml;
using System.Drawing.Drawing2D;

namespace YP.SVG.ClipAndMask
{
	/// <summary>
	/// clipPath
	/// </summary>
	public class SVGClipPathElement:YP.SVG.SVGTransformableElement,Interface.ISVGPathable,Interface.ClipAndMask.ISVGClipPathElement
	{
		#region ..���켰����
		public SVGClipPathElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			clip_path = new DataType.SVGString("",string.Empty);//,this);
		}
		#endregion

		#region ..˽�б���
		GraphicsPath gp = new GraphicsPath();
		YP.SVG.DataType.SVGString clip_path = DataType.SVGString.Empty;//new DataType.SVGString("",string.Empty,this);
		System.Xml.XmlNode refnode = null;
		string preclip = string.Empty;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ���з�ʽ
		/// </summary>
		public System.Enum ClipPathUnits
		{
			get
			{
				SVGUnitType clipPath = SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
				if(GetAttribute("clipPathUnits") == "objectBoundingBox") 
					clipPath = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;
				return clipPath;
			}
		}

        //Interface.ISVGRenderer Interface.ISVGPathable.Render
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

		public YP.SVG.ClipAndMask.SVGClipPathElement ClipElement
		{
			get
			{
				string clipstr = this.clip_path.Value;
				if(clipstr != this.preclip)
				{
					this.preclip = clipstr;
					string text = this.preclip.Substring(4);
					text = text.Substring(0,text.Length -1);
					this.refnode = this.OwnerDocument.GetReferencedNode(text);
					text = null;
				}

				if(this.refnode is YP.SVG.ClipAndMask.SVGClipPathElement)
					return (YP.SVG.ClipAndMask.SVGClipPathElement)this.refnode;
				return null;
			}
		}

		/// <summary>
		/// ��ȡ�����GDI·��
		/// </summary>
		public System.Drawing.Drawing2D.GraphicsPath GPath
		{
			get
			{
//				if(gp == null)
//				{
					this.gp.Reset();
					foreach(XmlNode child in this.ChildNodes)
					{
						if(child is Interface.ISVGPathable && child is YP.SVG.SVGStyleable)
						{
							GraphicsPath childPath = ((Interface.ISVGPathable)child).GPath.Clone() as GraphicsPath;
							childPath.Transform(((YP.SVG.SVGTransformableElement)child).Transform.FinalMatrix.GetGDIMatrix());
							
							this.gp.AddPath(childPath, true);
						}
					}
//				}
				return this.gp;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ���������Է����޸�ʱ�����¶���·��
		/// </summary>
		public void UpdateGPath()
		{
			this.gp = null;
		}
		#endregion

		#region ..���Բ���
		/// <summary>
		/// �����Է����޸�ʱ�����¶�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <param name="attributeValue">����ֵ</param>
		public override void SetSVGAttribute(string attributeName,string attributeValue)
		{
			try
			{
				switch(attributeName)
				{
					case "clip-path":
						this.clip_path = new DataType.SVGString(attributeValue,string.Empty);//,this);
						break;
					default:
						base.SetSVGAttribute(attributeName,attributeValue);
						break;
				}
			}
			catch(Exception e)
			{
				this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[]{e.Message},ExceptionLevel.Normal)); 
			}
		}
		#endregion

		#region ..��ȡ�ɶ�������
		/// <summary>
		/// ��ȡ�ɶ�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <returns></returns>
//		public override Interface.DataType.ISVGType GetAnimatedAttribute(string attributeName)
//		{
//			switch(attributeName)
//			{
//				case "clip-path":
//					return this.clip_path;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion
	}
}
