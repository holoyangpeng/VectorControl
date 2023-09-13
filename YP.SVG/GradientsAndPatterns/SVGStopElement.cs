using System;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// ʵ��Stop����
	/// </summary>
	public class SVGStopElement:YP.SVG.SVGStyleable,Interface.GradientsAndPatterns.ISVGStopElement
	{
		#region ..���켰����
		public SVGStopElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.offset = new DataType.SVGNumber(1);//"1",this);
			this.opacity = new DataType.SVGNumber(1);//"1","1",this);
			this.color = new DataType.RGBColor("none");//,this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGNumber offset;
		DataType.RGBColor color;
		DataType.SVGNumber opacity;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡOffset���ԣ���100Ϊ����
		/// </summary>
		public Interface.DataType.ISVGNumber Offset
		{
			get
			{
				return this.offset;
			}
		}

		/// <summary>
		/// ��ȡ͸����
		/// </summary>
		public Interface.DataType.ISVGNumber Opacity
		{
			get
			{
				this.ApplyCSSStyle();
				return this.opacity;
			}
		}

		/// <summary>
		/// ��ȡstop-color����
		/// </summary>
		public Interface.DataType.IRgbColor Color
		{
			get
			{
				this.ApplyCSSStyle();
				return this.color;
			}
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
					case "stop-color":
						this.color = new DataType.RGBColor(attributeValue);//,this);
						break;
					case "offset":
						string attr = attributeValue;
						if(attr.EndsWith("%"))
						{
							attr = attr.TrimEnd(new char[1]{'%'});
						}
						else
						{
							float tmp = DataType.SVGNumber.ParseNumberStr(attr) * 100;
							attr = tmp.ToString();
						}
						this.offset = new DataType.SVGNumber(attr,"1");//,this);
						attr = null;
						break;
					case "stop-opacity":
						this.opacity = new DataType.SVGNumber(attributeValue,"1");//,this);
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
//				case "stop-color":
//					return this.color;
//				case "offset":
//					return this.offset;
//				case "stop-opacity":
//					return this.opacity;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion
	}
}
