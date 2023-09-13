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
	/// <para>��¼��״��Ե�û�����Ϣ��</para>
	/// <para>ͨ��Stroke�ṹ��������������VectrolControl�л��ƶ�����ʹ�õ���ɫ��������״��͸���ȵ���Ϣ��</para>
	/// </summary>
	public struct Stroke
    {
        #region ..Constructor
        /// <summary>
		/// ��ָ������ɫ����һ��Stroke����
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
		/// ���û�����ɫ
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
		/// �û��Զ���Ļ��ʵĶ̻��ߺͿհ����������
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
		/// ������״��Ե�Ŀ��
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
		/// ����͸����
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
		/// ����==�����
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Stroke stroke1,Stroke stroke2)
		{
			return stroke1.Color == stroke2.Color && stroke1.Opacity == stroke2.Opacity && stroke1.Width == stroke2.Width && stroke1.DashPattern == stroke2.DashPattern;
		}

		/// <summary>
		/// ����!=�����
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
	/// <para>��¼��״�ڲ��������Ϣ��</para>
	/// <para>ͨ��Fill�ṹ��������������VectorControl�����������ƶ����ڲ��Ļ�ˢ����ɫ��͸���ȵ���Ϣ��</para>
	/// </summary>
	public struct Fill
	{
		Color color;
		float opacity;

		/// <summary>
		/// ��ָ������ɫ��͸���ȹ���һ����ˢ����
		/// </summary>
		/// <param name="color">��ˢ��ɫ</param>
		/// <param name="opacity">͸����</param>
		public Fill(Color color,float opacity)
		{
			this.color = color;
			this.opacity = opacity;
		}

		/// <summary>
		/// ��ˢ�������ɫ
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
		/// ��ˢ͸����
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
		/// ����==�����
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Fill fill1,Fill fill2)
		{
			return fill1.color == fill2.color && fill1.opacity == fill2.opacity;
		}

		/// <summary>
		/// ����!=�����
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
	/// <para>��¼����������Ϣ��</para>
	/// <para>ͨ��Grid�ṹ������������VectorControl�ؼ��༭�����е�������������������Ԫ����ߴ磬�����Ƿ�ɼ��������ߵĻ�����ɫ���Լ��Ƿ���Ҫ���뵽�������Ϣ��</para>
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
		/// ָ���Ƿ���ʾ����
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
		/// ��ʾ����ʱ���Ƿ���ƻ����߿�
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
        /// ������ֵ�����
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
		/// ָ����Ԫ����ߴ�
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
		/// ָ����������ɫ
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
        /// ����һ��������Ϣ
        /// </summary>
        /// <param name="size">����ߴ�</param>
        /// <param name="color">��ɫ</param>
        /// <param name="snap">�Ƿ���뵽����</param>
        /// <param name="visible">�Ƿ�ɼ�</param>
        /// <param name="drawBorder">�Ƿ���ƻ����߿�</param>
        public Grid(bool visible, int size, Color color)
            : this(visible, size, color, GridType.Line)
        {

        }

        /// <summary>
        /// ����һ��������Ϣ
        /// </summary>
        /// <param name="size">����ߴ�</param>
        /// <param name="color">��ɫ</param>
        /// <param name="snap">�Ƿ���뵽����</param>
        /// <param name="visible">�Ƿ�ɼ�</param>
        /// <param name="drawBorder">�Ƿ���ƻ����߿�</param>
        public Grid(bool visible)
            : this(visible, 10, Color.Gray, GridType.Line)
        {

        }

		/// <summary>
		/// ����һ��������Ϣ
		/// </summary>
		/// <param name="size">����ߴ�</param>
		/// <param name="color">��ɫ</param>
		/// <param name="snap">�Ƿ���뵽����</param>
		/// <param name="visible">�Ƿ�ɼ�</param>
		/// <param name="drawBorder">�Ƿ���ƻ����߿�</param>
        public Grid(bool visible, int size, Color color, GridType type)
            : this(visible, size, color, true, type)
		{
			
		}

		/// <summary>
		/// ����һ��������Ϣ
		/// </summary>
		/// <param name="size">����ߴ�</param>
		/// <param name="color">��ɫ</param>
		/// <param name="snap">�Ƿ���뵽����</param>
		/// <param name="visible">�Ƿ�ɼ�</param>
		/// <param name="drawBorder">�Ƿ���ƻ����߿�</param>
        public Grid(bool visible, int size, Color color, bool drawBorder)
            : this(visible, size, color, drawBorder, GridType.Line)
		{
			
		}

		/// <summary>
		/// ����һ��������Ϣ
		/// </summary>
		/// <param name="size">����ߴ�</param>
		/// <param name="color">��ɫ</param>
		/// <param name="snap">�Ƿ���뵽����</param>
		/// <param name="visible">�Ƿ�ɼ�</param>
		/// <param name="drawBorder">�Ƿ���ƻ����߿�</param>
        public Grid(bool visible, int size, Color color,  bool drawBorder, GridType gridType)
		{
			this.size = size;
			this.color = color;
			this.visible = visible;
			this.showBorder = drawBorder;
            this.gridType = gridType;
		}

		/// <summary>
		/// ����==�����
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Grid grid1,Grid grid2)
		{
			return grid1.Visible == grid2.Visible && grid1.Color == grid2.Color && grid1.Size == grid2.Size && grid1.DrawBorder == grid2.DrawBorder && grid1.GridType == grid2.GridType;
		}

		/// <summary>
		/// ����!=�����
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
    /// �������λ����������ʱ����¼���εĲ���
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
        /// ������������״ʱ����ȡ�������γ����εĶ���ζ�����
        /// ����һ������3��100֮�������
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
        /// ������������״ʱ����ȡ�������γ�������������
        /// ����һ������-10��1֮��ĸ�����
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
        /// ����==�����
        /// </summary>
        /// <param name="guide1"></param>
        /// <param name="guide2"></param>
        /// <returns></returns>
        public static bool operator ==(Star star1, Star star2)
        {
            return star1.indent == star2.indent && star1.numberOfVertexes == star2.numberOfVertexes;
        }

        /// <summary>
        /// ����!=�����
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
	/// <para>��¼�ο���������Ϣ��</para>
	/// <para>ͨ��Guide�ṹ������������VectorControl�ؼ��༭�����е���زο��߲����������ο����Ƿ�ɼ����ο��ߵĻ�����ɫ���Ƿ������ο��ߣ��Լ��Ƿ���Ҫ���뵽�������Ϣ��</para>
	/// <para>ͨ��<see cref="GuideSetupDialog">GuideSetupDialog</see>�Ի��򣬿��Կ��ӻ��ı༭Guide������</para>
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential),Editor("Design.GuideEditor",typeof(UITypeEditor)),TypeConverter(typeof(Converter.GuideConverter)),ComVisible(true)]
	public struct Guide
	{
		bool _visible;
		bool _isLock;
		Color _color;

		/// <summary>
		/// ָ���������Ƿ�ɼ�
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
		/// ָ���Ƿ�����������
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
		/// ָ����������ʾ��ɫ
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
		/// ���츨������Ϣ
		/// </summary>
		/// <param name="color">��ɫ</param>
		/// <param name="islock">�Ƿ�����</param>
		/// <param name="snap">�Ƿ����</param>
		/// <param name="visible">�Ƿ�ɼ�</param>
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
		/// ����==�����
		/// </summary>
		/// <param name="guide1"></param>
		/// <param name="guide2"></param>
		/// <returns></returns>
		public static bool operator == (Guide guide1,Guide guide2)
		{
			return guide1.Visible == guide2.Visible && guide1.Lock == guide2.Lock && guide1.Color == guide2.Color ;
		}

		/// <summary>
		/// ����!=�����
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
	/// <para>��¼���������Ϣ��</para>
	/// <para>ͨ��Rule�ṹ������������VectorControl�����еı��������Ϣ����������Ƿ�ɼ����Լ�����ϵĵ�Ԫ�̶ȵ�λ�ȡ�</para>
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(Converter.RuleConverter)),ComVisible(true)]
	internal struct Rule
	{
		UnitType unitType;
		bool visible;

		/// <summary>
		/// ָ����ߵĿ̶ȵ�λ��
		/// ����һ��<see cref="UnitType">UnitType</see>���͵�ֵ��
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
		/// ָ����߿ɼ���
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
		/// ����==�����
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Rule rule1,Rule rule2)
		{
			return rule1.Visible == rule2.Visible && rule1.UnitType == rule2.UnitType;
		}

		/// <summary>
		/// ����!=�����
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
	/// <para>��¼�ı�������Ϣ.</para>
    /// <para>ͨ��TextStyle�ṹ������������VectorControl�ڻ��Ƶ�ǰ�ı�(Text)���ı���(TextBlock)ʱ�������õ������������β�����</para>
	/// </summary>
	public struct TextStyle
	{
		string fontname;
		float size;
		bool bold;
		bool italic;
		bool underline;

		/// <summary>
		/// ��ȡ��������������
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
		/// ��ȡ�����óߴ�
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
		/// ָ���ı��Ƿ��Դ������
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
		/// ָ���ı��Ƿ���б�����
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
		/// ָ���ı��Ƿ�߱��»���
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
		/// ָ���ı��Ƿ�߱�ɾ����
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
		/// ����==�����
		/// </summary>
		/// <param name="text1"></param>
		/// <param name="text2"></param>
		/// <returns></returns>
		public static bool operator == (TextStyle text1,TextStyle text2)
		{
			return text1.italic == text2.italic && text1.fontname == text2.fontname && text1.bold == text2.bold && text1.underline == text2.underline;// && text1.strikeout == text2.strikeout;
		}

		/// <summary>
		/// ����!=�����
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
	/// <para>��¼ͼ�������Ϣ.</para>
	/// <para>ͨ��ͼ�����������ý��䡢���Ƶ����ͼ�ε��ڲ�</para>
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
        /// ͼ��������ɫ
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
        /// ͼ��ǰ����ɫ
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
        /// �ж��Ƿ���ͼ���������ǽ�������
        /// </summary>
		public bool IsPattern
		{
			get
			{
				return this.style != YP.SVG.HatchStyle.None && (int)this.style < 56;
			}
		}

		/// <summary>
		/// ����==�����
		/// </summary>
		/// <param name="text1"></param>
		/// <param name="text2"></param>
		/// <returns></returns>
		public static bool operator == (Hatch hatch,Hatch hatch1)
		{
			return hatch.backColor == hatch1.backColor && hatch.style == hatch1.style;
		}

		/// <summary>
		/// ����!=�����
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
	/// ��¼���ƶ���ļ�ͷ��Ϣ
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
	/// ��¼�ĵ��߾�
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential),TypeConverter(typeof(Converter.MarginConverter)),ComVisible(true)]
	public struct Margin
	{
		#region ..Constructor
		public Margin(int left,int top,int right,int bottom)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
		/// ��ȡ�������ĵ�����߾�
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
		/// ��ȡ�������ĵ����ϱ߾�
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
		/// ��ȡ�������ĵ����ұ߾�
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
		/// ��ȡ�������ĵ����±߾�
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
		/// ����==�����
		/// </summary>
		/// <param name="rule1"></param>
		/// <param name="rule2"></param>
		/// <returns></returns>
		public static bool operator == (Margin margin1,Margin margin2)
		{
			return margin1._left == margin2._left && margin1._right == margin2._right && margin1._top == margin2._top &&margin1._bottom == margin2._bottom;
		}

		/// <summary>
		/// ����!=�����
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
    /// ��������ı���ʱ���ı��������
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
        /// ��ȡ���������ı���ɫ
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
        /// ��ȡ��������ˮƽ���뷽ʽ
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
        /// ��ȡ����������ֱ���뷽ʽ
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
        /// ����==�����
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
        /// ����!=�����
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
