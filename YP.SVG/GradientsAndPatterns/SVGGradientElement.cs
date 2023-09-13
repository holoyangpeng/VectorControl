using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// ʵ�ֽ������
	/// </summary>
	public abstract class SVGGradientElement:YP.SVG.GradientsAndPatterns.SVGPaintTransformElement,Interface.ISVGContainer,Interface.GradientsAndPatterns.ISVGGradientElement
	{
		#region ..���켰����
		public SVGGradientElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.gradientUnits = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
			this.paintTransform = new DataType.SVGTransformList(string.Empty);;//"",this);
			this.href = new DataType.SVGString("",string.Empty);//,this);
			this.spreadMethod = SVG.SpreadMethod.SVG_SPREADMETHOD_PAD;//,SpreadMethod.SVG_SPREADMETHOD_PAD,this);
		}
		#endregion

		#region ..˽�б���
		YP.SVG.SVGElementCollection stops = new YP.SVG.SVGElementCollection();
		System.Enum gradientUnits;
		DataType.SVGString href ;
		System.Enum spreadMethod ;
		ColorBlend b = new ColorBlend();
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ��Ч�Ӽ��б�
		/// </summary>
		public SVGElementCollection ChildElements
		{
			get
			{
				return this.stops;
			}
		}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public System.Enum  GradientUnits
		{
			get
			{
				return this.gradientUnits;
			}
		}

		/// <summary>
		/// ��ȡ����Ķ�ά�任
		/// </summary>
		public Interface.CTS.ISVGTransformList GradientTransform
		{
			get
			{
				return this.paintTransform;
			}
		}

		/// <summary>
		/// ��ȡ�����ƽ��ģʽ
		/// </summary>
		public System.Enum SpreadMethod
		{
			get
			{
				return this.spreadMethod;
			}
		}

		/// <summary>
		/// ��ȡhref����
		/// </summary>
		public Interface.DataType.ISVGString Href
		{
			get
			{
				return this.href;
			}
		}
		#endregion

		#region ..��ȡ��ɫ��λ������
		/// <summary>
		/// ��ȡ��ɫ��λ������
		/// </summary>
		/// <param name="opacity">����͸����</param>
		/// <returns></returns>
		public System.Drawing.Drawing2D.ColorBlend GetColors(float opacity)
		{
			if(this.OwnerDocument.CurrentTime != this.CurrentTime)
			{
				YP.SVG.SVGElementCollection list = this.stops;
				Color[] colors = new Color[list.Count];
				float[] poses = new float[list.Count];

				for(int i = 0; i < list.Count; i++)
				{
					YP.SVG.GradientsAndPatterns.SVGStopElement stop = (YP.SVG.GradientsAndPatterns.SVGStopElement)list[i];
					int alpha = 255;
					float opacity1 = stop.Opacity.Value;
					if(opacity1 >= 0 && opacity1 <= 255)
					{
						if(opacity1 <= 1)
							alpha = (int)(opacity1 * 255);
						else
							alpha = (int)opacity1;
					}
					alpha = (int)Math.Min(opacity * 255,alpha);
					DataType.RGBColor rgbcolor = (DataType.RGBColor)stop.Color;
					float pos = (float)Math.Min(1,Math.Max(0,stop.Offset.Value / 100f));
					colors[i] = Color.FromArgb(alpha,rgbcolor.GDIColor);
					poses[i] = pos;
				}
				this.b = new ColorBlend(list.Count);
				b.Colors = colors;
				b.Positions = poses;
			}
			return b;
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
                switch (attributeName)
                {
                    case "href":
                    case "xlink:href":
                        this.href = new DataType.SVGString(attributeValue, string.Empty); ;//,this);
                        break;
                    case "gradientTransform":
                        this.paintTransform = new DataType.SVGTransformList(attributeValue); ;//,this);
                        break;
                    case "gradientUnits":
                        SVGUnitType unit = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;
                        if (string.Compare(attributeValue,"userSpaceOnUse")==0)
                            unit = SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
                        this.gradientUnits = unit;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
                        break;
                    case "spreadMethod":
                        SpreadMethod spread = SVG.SpreadMethod.SVG_SPREADMETHOD_PAD;
                        if (string.Compare(attributeValue,"reflect")==0)
                            spread = SVG.SpreadMethod.SVG_SPREADMETHOD_REFLECT;
                        else if (string.Compare(attributeValue,"repeat")==0)
                            spread = SVG.SpreadMethod.SVG_SPREADMETHOD_REPEAT;
                        this.spreadMethod = spread;//,SpreadMethod.SVG_SPREADMETHOD_PAD,this);
                        break;
                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (Exception e)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e.Message }, ExceptionLevel.Normal));
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
//				case "gradientTransform":
//					return this.paintTransform;
//				case "spreadMethod":
//					return this.spreadMethod;
//				case "gradientUnits":
//					return this.gradientUnits;
//				case "href":
//				case "xlink:href":
//					return this.href;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..�жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
		/// <summary>
		/// �жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
		/// </summary>
		/// <param name="child">�Ӽ��ڵ�</param>
		/// <returns></returns>
		public bool ValidChild(Interface.ISVGElement child)
		{
			return child is Interface.GradientsAndPatterns.ISVGStopElement;
		}
		#endregion

		#region ..���������ָ��·��ʱ����ȡ��任����
		/// <summary>
		/// ���������ָ��·��ʱ����ȡ��任����
		/// </summary>
		public override Matrix GetTransform(GraphicsPath fillPath)
		{
			bool userSpaceOnUse = (SVGUnitType)this.gradientUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;

			Matrix coord = new Matrix();
			RectangleF rect = fillPath.GetBounds();
			Interface.ISVGElement element = this.ViewPortElement;
			if(userSpaceOnUse && element != null)
			{
				rect = ((YP.SVG.DataType.SVGRect)((YP.SVG.Interface.ISVGFitToViewBox)element).ViewBox).GDIRect;	
			}
			if(!rect.IsEmpty)
			{
					coord.Translate(rect.X,rect.Y);
				coord.Scale(rect.Width,(this is YP.SVG.GradientsAndPatterns.SVGLinearGradientElement?rect.Width:rect.Height));
				if(this is YP.SVG.GradientsAndPatterns.SVGLinearGradientElement)
					coord.Scale(1,rect.Height / rect.Width);
			}
			return coord;
		}
		#endregion
	}
}
