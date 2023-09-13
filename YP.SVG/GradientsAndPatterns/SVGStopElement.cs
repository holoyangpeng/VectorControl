using System;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// 实现Stop对象
	/// </summary>
	public class SVGStopElement:YP.SVG.SVGStyleable,Interface.GradientsAndPatterns.ISVGStopElement
	{
		#region ..构造及消除
		public SVGStopElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.offset = new DataType.SVGNumber(1);//"1",this);
			this.opacity = new DataType.SVGNumber(1);//"1","1",this);
			this.color = new DataType.RGBColor("none");//,this);
		}
		#endregion

		#region ..私有变量
		DataType.SVGNumber offset;
		DataType.RGBColor color;
		DataType.SVGNumber opacity;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取Offset属性，以100为基数
		/// </summary>
		public Interface.DataType.ISVGNumber Offset
		{
			get
			{
				return this.offset;
			}
		}

		/// <summary>
		/// 获取透明度
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
		/// 获取stop-color属性
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
