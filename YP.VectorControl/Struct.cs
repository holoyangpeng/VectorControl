using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using YP.VectorControl.Forms;

namespace YP.VectorControl
{
    #region ..Stroke
    /// <summary>
	/// <para>记录形状边缘得绘制信息。</para>
	/// <para>通过Stroke结构，您可以设置在VectrolControl中绘制对象所使用的颜色，画笔形状，透明度等信息。</para>
	/// </summary>
	public struct Stroke
    {
        #region ..Constructor
        /// <summary>
		/// 用指定的颜色构造一个Stroke对象
		/// </summary>
		/// <param name="color"></param>
		public Stroke(Color color)
		{
			this.color = color;
			this.dashPattern = null;
			this.opacity = 1f;
			this.width = 1;
		}
        #endregion

        #region ..private fields
        Color color;
        float[] dashPattern;
        float opacity;
        float width;
        #endregion

        #region ..properties
        /// <summary>
		/// 设置画笔颜色
		/// </summary>
        public Color Color
        {
            set
            {
                this.color = value;
            }
            get
            {
                return this.color;
            }
        }

		/// <summary>
		/// 用户自定义的画笔的短划线和空白区域的数组
		/// </summary>
        public float[] DashPattern
        {
            set
            {
                this.dashPattern = value;
            }
            get
            {
                return this.dashPattern;
            }
        }

		/// <summary>
		/// 绘制形状边缘的宽度
		/// </summary>
        public float Width
        {
            set
            {
                this.width = value;
            }
            get
            {
                return this.width;
            }
        }

		/// <summary>
		/// 画笔透明度
		/// </summary>
        public float Opacity
        {
            set
            {
                this.opacity = value;
            }
            get
            {
                return this.opacity;
            }
        }
        #endregion

        /// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Stroke stroke1,Stroke stroke2)
		{
			return stroke1.Color == stroke2.Color && stroke1.Opacity == stroke2.Opacity && stroke1.Width == stroke2.Width && stroke1.DashPattern == stroke2.DashPattern;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (Stroke stroke1,Stroke stroke2)
		{
			return stroke1.Color != stroke2.Color || stroke1.Opacity != stroke2.Opacity|| stroke1.Width != stroke2.Width || stroke1.DashPattern != stroke2.DashPattern;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Stroke)
				return this == (Stroke)obj;
			return false;
		}
	}
	#endregion

    #region ..Fill
    /// <summary>
	/// <para>记录形状内部的填充信息。</para>
	/// <para>通过Fill结构，您可以设置在VectorControl中勇于填充绘制对象内部的画刷的颜色，透明度等信息。</para>
	/// </summary>
	public struct Fill
	{
		Color color;
		float opacity;

		/// <summary>
		/// 用指定的颜色和透明度构造一个画刷对象
		/// </summary>
		/// <param name="color">画刷颜色</param>
		/// <param name="opacity">透明度</param>
		public Fill(Color color,float opacity)
		{
			this.color = color;
			this.opacity = opacity;
		}

		/// <summary>
		/// 画刷的填充颜色
		/// </summary>
		public Color Color
		{
			set
			{
				this.color = value;
			}
			get
			{
				return this.color;
			}
		}

		/// <summary>
		/// 画刷透明度
		/// </summary>
		public float Opacity
		{
			set
			{
				this.opacity = value;
			}
			get
			{
				return this.opacity;
			}
		}

		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Fill fill1,Fill fill2)
		{
			return fill1.color == fill2.color && fill1.opacity == fill2.opacity;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (Fill fill1,Fill fill2)
		{
			return fill1.color != fill2.color || fill1.opacity != fill2.opacity;// || rule1.UnitType != rule2.UnitType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Fill)
				return this == (Fill)obj;
			return false;
		}
	}
	#endregion

    #region ..Grid
    /// <summary>
	/// <para>记录网格设置信息。</para>
	/// <para>通过Grid结构，您可以设置VectorControl控件编辑环境中的相关网格参数，包括单元网格尺寸，网格是否可见，网格线的绘制颜色，以及是否需要对齐到网格等信息。</para>
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential),Editor("Design.GridEditor",typeof(UITypeEditor)), TypeConverter(typeof(Converter.GridConverter)),ComVisible(true)]
	public struct Grid
	{
		bool visible;
		int size;
		Color color;
        GridType gridType;
		bool showBorder ;

		/// <summary>
		/// 指定是否显示网格
		/// </summary>
		public bool Visible
		{
			set
			{
				this.visible = value;
			}
			get
			{
				return this.visible;
			}
		}

		/// <summary>
		/// 显示网格时，是否绘制画布边框
		/// </summary>
		public bool DrawBorder
		{
			set
			{
				this.showBorder = value;
			}
			get
			{
				return this.showBorder;
			}
		}

        /// <summary>
        /// 网格呈现的类型
        /// </summary>
        public GridType GridType
        {
            set
            {
                this.gridType = value;
            }
            get
            {
                return this.gridType;
            }
        }

		/// <summary>
		/// 指定单元网格尺寸
		/// </summary>
		public int Size
		{
			set
			{
				this.size = value;
			}
			get
			{
				return this.size;
			}
		}

		/// <summary>
		/// 指定网格线颜色
		/// </summary>
		public Color Color
		{
			set
			{
				this.color = value;
			}
			get
			{
				return this.color;
			}
		}


        /// <summary>
        /// 构造一个网格信息
        /// </summary>
        /// <param name="size">网格尺寸</param>
        /// <param name="color">颜色</param>
        /// <param name="snap">是否对齐到网格</param>
        /// <param name="visible">是否可见</param>
        /// <param name="drawBorder">是否绘制画布边框</param>
        public Grid(bool visible, int size, Color color)
            : this(visible, size, color, GridType.Line)
        {

        }

        /// <summary>
        /// 构造一个网格信息
        /// </summary>
        /// <param name="size">网格尺寸</param>
        /// <param name="color">颜色</param>
        /// <param name="snap">是否对齐到网格</param>
        /// <param name="visible">是否可见</param>
        /// <param name="drawBorder">是否绘制画布边框</param>
        public Grid(bool visible)
            : this(visible, 10, Color.Gray, GridType.Line)
        {

        }

		/// <summary>
		/// 构造一个网格信息
		/// </summary>
		/// <param name="size">网格尺寸</param>
		/// <param name="color">颜色</param>
		/// <param name="snap">是否对齐到网格</param>
		/// <param name="visible">是否可见</param>
		/// <param name="drawBorder">是否绘制画布边框</param>
        public Grid(bool visible, int size, Color color, GridType type)
            : this(visible, size, color, true, type)
		{
			
		}

		/// <summary>
		/// 构造一个网格信息
		/// </summary>
		/// <param name="size">网格尺寸</param>
		/// <param name="color">颜色</param>
		/// <param name="snap">是否对齐到网格</param>
		/// <param name="visible">是否可见</param>
		/// <param name="drawBorder">是否绘制画布边框</param>
        public Grid(bool visible, int size, Color color, bool drawBorder)
            : this(visible, size, color, drawBorder, GridType.Line)
		{
			
		}

		/// <summary>
		/// 构造一个网格信息
		/// </summary>
		/// <param name="size">网格尺寸</param>
		/// <param name="color">颜色</param>
		/// <param name="snap">是否对齐到网格</param>
		/// <param name="visible">是否可见</param>
		/// <param name="drawBorder">是否绘制画布边框</param>
        public Grid(bool visible, int size, Color color,  bool drawBorder, GridType gridType)
		{
			this.size = size;
			this.color = color;
			this.visible = visible;
			this.showBorder = drawBorder;
            this.gridType = gridType;
		}

		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Grid grid1,Grid grid2)
		{
			return grid1.Visible == grid2.Visible && grid1.Color == grid2.Color && grid1.Size == grid2.Size && grid1.DrawBorder == grid2.DrawBorder && grid1.GridType == grid2.GridType;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (Grid grid1,Grid grid2)
		{
			return grid1.Visible != grid2.Visible || grid1.Color != grid2.Color || grid1.Size != grid2.Size ||  grid1.DrawBorder != grid2.DrawBorder || grid1.GridType != grid2.GridType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Grid)
				return this == (Grid)obj;
			return false;
		}
	}
	#endregion

    #region ..Star
    /// <summary>
    /// 绘制星形或者正多边形时，记录星形的参数
    /// </summary>
    public struct Star
    {
        #region ..Constructor
        public Star(int numerOfVertexes, float indent)
        {
            this.numberOfVertexes = numerOfVertexes;
            this.indent = indent;
        }
        #endregion

        #region ..private fields
        int numberOfVertexes;
        float indent;
        #endregion

        #region ..public properties
        /// <summary>
        /// 当构造星形形状时，获取或设置形成星形的多边形顶点数
        /// 这是一个介于3和100之间的整数
        /// </summary>
        public int NumberOfVertexes
        {
            set
            {
                value = (int)Math.Max(3, Math.Min(100, value));
                this.numberOfVertexes = value;
            }
            get
            {
                return this.numberOfVertexes;
            }
        }

        /// <summary>
        /// 当构造星形形状时，获取或设置形成星形缩进比率
        /// 这是一个介于-10和1之间的浮点数
        /// </summary>
        public float Indent
        {
            set
            {
                value = (float)Math.Min(5, Math.Max(-10, value));
                this.indent = value;
            }
            get
            {
                return this.indent;
            }
        }
        #endregion

        /// <summary>
        /// 重载==运算符
        /// </summary>
        /// <param name="guide1"></param>
        /// <param name="guide2"></param>
        /// <returns></returns>
        public static bool operator ==(Star star1, Star star2)
        {
            return star1.indent == star2.indent && star1.numberOfVertexes == star2.numberOfVertexes;
        }

        /// <summary>
        /// 重载!=运算符
        /// </summary>
        /// <param name="guide2"></param>
        /// <param name="guide2"></param>
        /// <returns></returns>
        public static bool operator !=(Star star1, Star star2)
        {
            return star1.numberOfVertexes != star2.numberOfVertexes || star1.indent != star2.indent;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Star)
                return this == (Star)obj;
            return false;
        }
    }
    #endregion

    #region ..Guide
    /// <summary>
	/// <para>记录参考线设置信息。</para>
	/// <para>通过Guide结构，您可以设置VectorControl控件编辑环境中的相关参考线参数，包括参考线是否可见，参考线的绘制颜色，是否锁定参考线，以及是否需要对齐到网格等信息。</para>
	/// <para>通过<see cref="GuideSetupDialog">GuideSetupDialog</see>对话框，可以可视化的编辑Guide参数。</para>
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential),Editor("Design.GuideEditor",typeof(UITypeEditor)),TypeConverter(typeof(Converter.GuideConverter)),ComVisible(true)]
	public struct Guide
	{
		bool _visible;
		bool _isLock;
		Color _color;

		/// <summary>
		/// 指定辅助线是否可见
		/// </summary>
		public bool Visible
		{
			set
			{
				this._visible = value;
			}
			get
			{
				return this._visible;
			}
		}

		/// <summary>
		/// 指定是否锁定辅助线
		/// </summary>
		public bool Lock
		{
			set
			{
				this._isLock = value;
			}
			get
			{
				return this._isLock;
			}
		}

		/// <summary>
		/// 指定辅助线显示颜色
		/// </summary>
		public Color Color
		{
			set
			{
				this._color = value;
			}
			get
			{
				return this._color;
			}
		}

		/// <summary>
		/// 构造辅助线信息
		/// </summary>
		/// <param name="color">颜色</param>
		/// <param name="islock">是否锁定</param>
		/// <param name="snap">是否对齐</param>
		/// <param name="visible">是否可见</param>
		public Guide(bool visible,bool islock,Color color)
		{
			this._color = color;
			this._isLock = islock;
			this._visible = visible;
		}

        public Guide(bool visible)
            : this(visible, false, Color.Blue)
        {
        }


		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="guide1"></param>
		/// <param name="guide2"></param>
		/// <returns></returns>
		public static bool operator == (Guide guide1,Guide guide2)
		{
			return guide1.Visible == guide2.Visible && guide1.Lock == guide2.Lock && guide1.Color == guide2.Color ;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="guide2"></param>
		/// <param name="guide2"></param>
		/// <returns></returns>
		public static bool operator != (Guide guide1,Guide guide2)
		{
			return guide1.Visible != guide2.Visible || guide1.Lock != guide2.Lock || guide1.Color != guide2.Color ;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Guide)
				return this == (Guide)obj;
			return false;
		}
	}
	#endregion

    #region ..Rule
    /// <summary>
	/// <para>记录标尺设置信息。</para>
	/// <para>通过Rule结构，您可以设置VectorControl环境中的标尺设置信息，包括标尺是否可见，以及标尺上的单元刻度单位等。</para>
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(Converter.RuleConverter)),ComVisible(true)]
	internal struct Rule
	{
		UnitType unitType;
		bool visible;

		/// <summary>
		/// 指定标尺的刻度单位。
		/// 这是一个<see cref="UnitType">UnitType</see>类型的值。
		/// </summary>
		public UnitType UnitType
		{
			set
			{
				this.unitType = value;
			}
			get
			{
				return this.unitType;
			}
		}

		/// <summary>
		/// 指定标尺可见性
		/// </summary>
		public bool Visible
		{
			set
			{
				this.visible = value;
			}
			get
			{
                return this.visible;
			}
		}

		public Rule(bool visible,UnitType type)
		{
			this.visible = visible;
			this.unitType = type;
		}

		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Rule rule1,Rule rule2)
		{
			return rule1.Visible == rule2.Visible && rule1.UnitType == rule2.UnitType;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (Rule rule1,Rule rule2)
		{
			return rule1.Visible != rule2.Visible || rule1.UnitType != rule2.UnitType;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Rule)
				return this == (Rule)obj;
			return false;
		}
	}
	#endregion

    #region ..TextStyle
    /// <summary>
	/// <para>记录文本绘制信息.</para>
    /// <para>通过TextStyle结构，您可以设置VectorControl在绘制当前文本(Text)和文本块(TextBlock)时，所采用的相关字体和字形参数。</para>
	/// </summary>
	public struct TextStyle
	{
		string fontname;
		float size;
		bool bold;
		bool italic;
		bool underline;

		/// <summary>
		/// 获取或设置字体名称
		/// </summary>
		public string FontName
		{
			set
			{
				this.fontname = value;
			}
			get
			{
				return this.fontname;
			}
		}

		/// <summary>
		/// 获取或设置尺寸
		/// </summary>
		public float Size
		{
			set
			{
				this.size = value;
			}
			get
			{
				return this.size;
			}
		}

		/// <summary>
		/// 指定文本是否以粗体绘制
		/// </summary>
		public bool Bold
		{
			set
			{
				this.bold = value;
			}
			get
			{
				return this.bold;
			}
		}

		/// <summary>
		/// 指定文本是否以斜体绘制
		/// </summary>
		public bool Italic
		{
			set
			{
				this.italic = value;
			}
			get
			{
				return this.italic;
			}
		}

		/// <summary>
		/// 指定文本是否具备下划线
		/// </summary>
		public bool Underline
		{
			set
			{
				this.underline = value;
			}
			get
			{
				return this.underline;
			}
		}

		/// <summary>
		/// 指定文本是否具备删除线
		/// </summary>
//			public bool StrikeOut
//			{
//				set
//				{
//					this.strikeout = value;
//				}
//				get
//				{
//					return this.strikeout;
//				}
//			}

		public TextStyle(string fontname,float size,bool bold,bool italic,bool underline)
		{
			this.fontname = fontname;
			this.size = size;
			this.bold = bold;
			this.italic = italic;
			this.underline = underline;
		}

		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="text1"></param>
		/// <param name="text2"></param>
		/// <returns></returns>
		public static bool operator == (TextStyle text1,TextStyle text2)
		{
			return text1.italic == text2.italic && text1.fontname == text2.fontname && text1.bold == text2.bold && text1.underline == text2.underline;// && text1.strikeout == text2.strikeout;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (TextStyle text1,TextStyle text2)
		{
			return text1.italic != text2.italic || text1.fontname != text2.fontname || text1.bold != text2.bold || text1.underline != text2.underline ;//|| text1.strikeout != text2.strikeout;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is TextStyle)
				return this == (TextStyle)obj;
			return false;
		}
	}
	#endregion

    #region ..HatchStyle
    /// <summary>
	/// <para>记录图案填充信息.</para>
	/// <para>通过图案，您可以用渐变、网纹等填充图形的内部</para>
	/// </summary>
	public struct Hatch
	{
		Color backColor;
		YP.SVG.HatchStyle style;
		Color forecolor;

		internal Hatch(Color backcolor,YP.SVG.HatchStyle style,Color forecolor)
		{
			this.backColor = backcolor;
			this.style = style;
			this.forecolor = forecolor;
		}

        /// <summary>
        /// 图案背景颜色
        /// </summary>
		public Color BackColor
		{
			set
			{
				this.backColor = value;
			}
			get
			{
				return this.backColor;
			}
		}

        /// <summary>
        /// 图案前景颜色
        /// </summary>
		public Color ForeColor
		{
			set
			{
				this.forecolor = value;
			}
			get
			{
				return this.forecolor;
			}
		}

		internal YP.SVG.HatchStyle Style
		{
			set
			{
				this.style = value;
			}
			get
			{
				return this.style;
			}
		}

        /// <summary>
        /// 判断是否是图案，或者是渐变类型
        /// </summary>
		public bool IsPattern
		{
			get
			{
				return this.style != YP.SVG.HatchStyle.None && (int)this.style < 56;
			}
		}

		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="text1"></param>
		/// <param name="text2"></param>
		/// <returns></returns>
		public static bool operator == (Hatch hatch,Hatch hatch1)
		{
			return hatch.backColor == hatch1.backColor && hatch.style == hatch1.style;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (Hatch hatch,Hatch hatch1)
		{
			return hatch.backColor != hatch1.backColor || hatch.style != hatch1.style;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Hatch)
				return this == (Hatch)obj;
			return false;
		}

		public override string ToString()
		{
			return this.style.ToString();
		}

	}
	#endregion

    #region ..ArrowProperty
    /// <summary>
	/// 记录绘制对象的箭头信息
	/// </summary>
	internal struct ArrowProperty
	{
		bool endarrow;
		Arrow arrow;
		YP.SVG.ClipAndMask.SVGMarkerElement marker;
		string id ;

		public ArrowProperty(Arrow arrow,YP.SVG.ClipAndMask.SVGMarkerElement marker,bool endarrow,string id)
		{
			this.endarrow = endarrow;
			this.marker = marker;
			this.arrow = arrow;
			this.id = id;
		}

		public string ID
		{
			set
			{
				this.id = value;
			}
			get
			{
				return this.id;
			}
		}

		public bool EndArrow
		{
			set
			{
				this.endarrow = value;
			}
			get
			{
				return this.endarrow;
			}
		}

		public Arrow Arrow
		{
			set
			{
				this.arrow = value;
			}
			get
			{
                return this.arrow;					
			}
		}

		public YP.SVG.ClipAndMask.SVGMarkerElement Marker
		{
			set
			{
				this.marker = value;
			}
			get
			{
				return this.marker;
			}
		}

		public override string ToString()
		{
			return this.id;
		}

	}
	#endregion

    #region ..Margin
    /// <summary>
	/// 记录文档边距
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential),TypeConverter(typeof(Converter.MarginConverter)),ComVisible(true)]
	public struct Margin
	{
		#region ..Constructor
		public Margin(int left,int top,int right,int bottom)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this._left = (int)Math.Max(0,left);
			this._right = (int)Math.Max(0,right);
			this._top = (int)Math.Max(0,top);
			this._bottom = (int)Math.Max(0,bottom);
		}
		#endregion

		#region ..private fields
		int _left;
		int _right ;
		int _top ;
		int _bottom ;
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取或设置文档的左边距
		/// </summary>
		public int Left
		{
			set
			{
				this._left = (int)Math.Max(0,value);
			}
			get
			{
				return this._left;
			}
		}

		/// <summary>
		/// 获取或设置文档的上边距
		/// </summary>
		public int Top
		{
			set
			{
				this._top =  (int)Math.Max(0,value);;
			}
			get
			{
				return this._top;
			}
		}

		/// <summary>
		/// 获取或设置文档的右边距
		/// </summary>
		public int Right
		{
			set
			{
				this._right =  (int)Math.Max(0,value);;
			}
			get
			{
				return this._right;
			}
		}

		/// <summary>
		/// 获取或设置文档的下边距
		/// </summary>
		public int Bottom
		{
			set
			{
				this._bottom =  (int)Math.Max(0,value);;
			}
			get
			{
				return this._bottom ;
			}
		}
		#endregion

		/// <summary>
		/// 重载==运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Margin margin1,Margin margin2)
		{
			return margin1._left == margin2._left && margin1._right == margin2._right && margin1._top == margin2._top &&margin1._bottom == margin2._bottom;
		}

		/// <summary>
		/// 重载!=运算符
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator != (Margin margin1,Margin margin2)
		{
			return margin1._left != margin2._left || margin1._right != margin2._right || margin1._top !=margin2._top || margin1._bottom !=margin2._bottom;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is Margin)
				return this == (Margin)obj;
			return false;
		}
	}
	#endregion

    #region ..TextBlockStyle
    /// <summary>
    /// 定义绘制文本块时，文本块的属性
    /// </summary>
    public struct TextBlockStyle
    {
        #region ..private fields
        Color textColor;
        SVG.Alignment alignment;
        SVG.VerticalAlignment verticalAlign;
        #endregion

        #region ..Constructor
        public TextBlockStyle(Color textColor, SVG.Alignment alignment, SVG.VerticalAlignment verticalAlignment)
        {
            this.textColor = textColor;
            this.alignment = alignment;
            this.verticalAlign = verticalAlignment;
        }
        #endregion

        #region ..public perperties
        /// <summary>
        /// 获取或者设置文本颜色
        /// </summary>
        public Color TextColor
        {
            set
            {
                this.textColor = value;
            }
            get
            {
                return this.textColor;
            }
        }

        /// <summary>
        /// 获取或者设置水平对齐方式
        /// </summary>
        public SVG.Alignment Alignment
        {
            set
            {
                this.alignment = value;
            }
            get
            {
                return this.alignment;
            }
        }

        /// <summary>
        /// 获取或者设置竖直对齐方式
        /// </summary>
        public SVG.VerticalAlignment VerticalAlignment
        {
            set
            {
                this.verticalAlign = value;
            }
            get
            {
                return this.verticalAlign;
            }
        }
        #endregion

        /// <summary>
        /// 重载==运算符
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns></returns>
        public static bool operator ==(TextBlockStyle text1, TextBlockStyle text2)
        {
            return text1.textColor == text2.textColor 
                && text1.alignment == text2.alignment 
                && text1.verticalAlign == text2.verticalAlign;// && text1.strikeout == text2.strikeout;
        }

        /// <summary>
        /// 重载!=运算符
        /// </summary>
        /// <param name="rule1"></param>
        /// <param name="rule2"></param>
        /// <returns></returns>
        public static bool operator !=(TextBlockStyle text1, TextBlockStyle text2)
        {
            return text1.textColor != text2.textColor 
                || text1.alignment != text2.alignment 
                || text1.verticalAlign != text2.verticalAlign;// || text1.underline != text2.underline;//|| text1.strikeout != text2.strikeout;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TextBlockStyle)
                return this == (TextBlockStyle)obj;
            return false;
        }
    }
    #endregion

    #region ..Win32 Struct
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCROLLINFO
    {
        public int cbSize;
        public uint fMask;
        public int nMin;
        public int nMax;
        public uint nPage;
        public int nPos;
        public int nTrackPos;
    }
    #endregion

}
