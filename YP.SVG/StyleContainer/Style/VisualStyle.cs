using System;

using YP.SVG.StyleContainer;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 记录常规类型信息
	/// </summary>
    public struct VisualMediaStyle
	{
		#region ..构造及消除
//		public VisualMediaStyle()
//		{
//			//
//			// TODO: 在此处添加构造函数逻辑
//			//
//			this.display = new DataType.SVGString("block");
//			this.visiblility = new DataType.SVGString("visible");
//			this.clip = new DataType.SVGString("auto");
//			this.overflow = new DataType.SVGString("visible");
//			this.color  = new DataType.SVGString("auto");
//			this.cursor = new DataType.SVGString("auto");
//		}

		public VisualMediaStyle(VisualMediaStyle style)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.display = style.display;
			this.visiblility = style.visiblility;
			this.clip = style.clip;
			this.overflow = style.overflow;
			this.color  = style.color;
			this.cursor = style.cursor;
            this.wrap = style.wrap;
		}

		VisualMediaStyle(YP.SVG.DataType.SVGString vb,
			YP.SVG.DataType.SVGString dis,
			YP.SVG.DataType.SVGString clip,
			YP.SVG.DataType.SVGString overflow,
			YP.SVG.DataType.SVGString color,
			YP.SVG.DataType.SVGString cursor,
            DataType.SVGString wrap)
		{
			this.visiblility = vb;
			this.display = dis;
			this.clip = clip;
			this.overflow = overflow;
			this.color = color;
			this.cursor = cursor;
            this.wrap = wrap;
		}
		#endregion

		#region .静态
		static VisualMediaStyle style = new VisualMediaStyle(
			new DataType.SVGString("visible"),
			new DataType.SVGString("block"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("visible"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("auto"),
            new DataType.SVGString("auto"));
		static VisualMediaStyle empty = new VisualMediaStyle(
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
             DataType.SVGString.Empty);

		/// <summary>
		/// 获取默认对象
		/// </summary>
		public static VisualMediaStyle Default
		{
			get
			{
				return style;
			}
		}

		/// <summary>
		/// 获取空对象
		/// </summary>
		public static VisualMediaStyle Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..保护变量
		public YP.SVG.DataType.SVGString visiblility ;
		public YP.SVG.DataType.SVGString display;
		public YP.SVG.DataType.SVGString clip ;
		public YP.SVG.DataType.SVGString overflow ;
		public YP.SVG.DataType.SVGString color ;
		public YP.SVG.DataType.SVGString cursor;
        public DataType.SVGString wrap;
		#endregion

		#region ..私有变量

		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public VisualMediaStyle MutiplyStyle(VisualMediaStyle newstyle)
		{
//			if(newstyle == null )
//				return this;
			if(this.visiblility.IsEmpty)
				this.visiblility = (DataType.SVGString)newstyle.visiblility;
			else
				this.visiblility = TypeMultiply.Multiply(this.visiblility,newstyle.visiblility);
			if(this.clip .IsEmpty)
				this.clip = (DataType.SVGString)newstyle.clip;
			else
				this.clip = TypeMultiply.Multiply(this.clip,newstyle.clip);
			if(this.overflow .IsEmpty)
				this.overflow = (DataType.SVGString)newstyle.overflow;
			else
				this.overflow = TypeMultiply.Multiply(this.overflow,newstyle.overflow);
			if(this.display .IsEmpty)
				this.display = (DataType.SVGString)newstyle.display;
			else
				this.display = TypeMultiply.Multiply(this.display,newstyle.display);
			if(this.color .IsEmpty)
				this.color = (DataType.SVGString)newstyle.color;
			else
				this.color = TypeMultiply.Multiply(this.overflow,newstyle.overflow);
			if(this.cursor .IsEmpty)
				this.cursor = (DataType.SVGString)newstyle.cursor;
			else
				this.cursor = TypeMultiply.Multiply(this.cursor,newstyle.cursor);
            if (this.wrap.IsEmpty)
                this.wrap = newstyle.wrap;
            else
                this.wrap = TypeMultiply.Multiply(this.wrap, newstyle.wrap);
			return this;
		}
		#endregion

		#region ..克隆
//		public VisualMediaStyle Clone()
//		{
//			VisualMediaStyle fillstyle = new VisualMediaStyle();
//			if(this.visiblility != null)
//				fillstyle.visiblility = new DataType.SVGString(this.visiblility.Value);
//			if(this.display != null)
//				fillstyle.display = new DataType.SVGString(this.display.Value);
//			if(this.overflow != null)
//				fillstyle.overflow = new DataType.SVGString(this.overflow.Value);
//			if(this.clip != null)
//				fillstyle.clip = new DataType.SVGString(this.clip.Value);
//			if(this.color != null)
//				fillstyle.color = new DataType.SVGString(this.color.Value);
//			if(this.cursor != null)
//				fillstyle.cursor = new DataType.SVGString(this.cursor.Value);
//			
//			return fillstyle;
//		}
		#endregion
	}
}
