using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// 实现渐变对象
	/// </summary>
	public abstract class SVGGradientElement:YP.SVG.GradientsAndPatterns.SVGPaintTransformElement,Interface.ISVGContainer,Interface.GradientsAndPatterns.ISVGGradientElement
	{
		#region ..构造及消除
		public SVGGradientElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.gradientUnits = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
			this.paintTransform = new DataType.SVGTransformList(string.Empty);;//"",this);
			this.href = new DataType.SVGString("",string.Empty);//,this);
			this.spreadMethod = SVG.SpreadMethod.SVG_SPREADMETHOD_PAD;//,SpreadMethod.SVG_SPREADMETHOD_PAD,this);
		}
		#endregion

		#region ..私有变量
		YP.SVG.SVGElementCollection stops = new YP.SVG.SVGElementCollection();
		System.Enum gradientUnits;
		DataType.SVGString href ;
		System.Enum spreadMethod ;
		ColorBlend b = new ColorBlend();
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取有效子级列表
		/// </summary>
		public SVGElementCollection ChildElements
		{
			get
			{
				return this.stops;
			}
		}

		/// <summary>
		/// 获取渐变类型
		/// </summary>
		public System.Enum  GradientUnits
		{
			get
			{
				return this.gradientUnits;
			}
		}

		/// <summary>
		/// 获取渐变的二维变换
		/// </summary>
		public Interface.CTS.ISVGTransformList GradientTransform
		{
			get
			{
				return this.paintTransform;
			}
		}

		/// <summary>
		/// 获取渐变的平铺模式
		/// </summary>
		public System.Enum SpreadMethod
		{
			get
			{
				return this.spreadMethod;
			}
		}

		/// <summary>
		/// 获取href属性
		/// </summary>
		public Interface.DataType.ISVGString Href
		{
			get
			{
				return this.href;
			}
		}
		#endregion

		#region ..获取颜色和位置数组
		/// <summary>
		/// 获取颜色和位置数组
		/// </summary>
		/// <param name="opacity">整体透明度</param>
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

		#region ..属性操作
		/// <summary>
		/// 当属性发生修改时，更新对象属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <param name="attributeValue">属性值</param>
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

		#region ..获取可动画属性
		/// <summary>
		/// 获取可动画属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
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

		#region ..判断节点是否是有效的子级节点
		/// <summary>
		/// 判断节点是否是有效的子级节点
		/// </summary>
		/// <param name="child">子级节点</param>
		/// <returns></returns>
		public bool ValidChild(Interface.ISVGElement child)
		{
			return child is Interface.GradientsAndPatterns.ISVGStopElement;
		}
		#endregion

		#region ..当对象填充指定路径时，获取其变换矩阵
		/// <summary>
		/// 当对象填充指定路径时，获取其变换矩阵
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
