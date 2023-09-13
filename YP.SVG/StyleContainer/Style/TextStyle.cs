using System;

using YP.SVG.StyleContainer;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 记录文本类型
	/// </summary>
    public struct TextStyle
	{
		#region ..构造及消除
//		public TextStyle()
//		{
//			//
//			// TODO: 在此处添加构造函数逻辑
//			//
//			this.direction = new DataType.SVGString("ltr");
//			this.letter_spacing = new DataType.SVGString("normal");
//			this.text_decoration = new DataType.SVGString("none");
//			this.unicode_bidi = new DataType.SVGString("normal");
//			this.word_spacing = new DataType.SVGString("normal");
//			this.alignment_baseline = new DataType.SVGString("auto");
//			this.baseline_shift = new DataType.SVGString("baseline");
//			this.dominant_baseline = new DataType.SVGString("auto");
//			this.glyph_orientation_horizontal = new DataType.SVGString("0deg");
//			this.glyph_orientation_vertical = new DataType.SVGString("auto");
//			this.kerning = new DataType.SVGString("auto");
//			this.text_anchor = new DataType.SVGString("start");
//			this.writing_mode = new DataType.SVGString("lr-tb");
//		}

		public TextStyle(TextStyle style)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.direction = style.direction;
            this.text_color = style.text_color;
			this.letter_spacing = style.letter_spacing;
			this.text_decoration = style.text_decoration;
			this.unicode_bidi = style.unicode_bidi;
			this.word_spacing = style.word_spacing;
			this.alignment_baseline = style.alignment_baseline;
			this.baseline_shift = style.baseline_shift;
			this.dominant_baseline = style.dominant_baseline;
			this.glyph_orientation_horizontal = style.glyph_orientation_horizontal;
			this.glyph_orientation_vertical = style.glyph_orientation_vertical;
			this.kerning = style.kerning;
			this.text_anchor = style.text_anchor;
			this.writing_mode = style.writing_mode;
            this.vertical_align = style.vertical_align;
            this.Changed = style.Changed;
		}

        TextStyle(
            DataType.SVGString direction,
            DataType.SVGString letter_spacing,
            DataType.SVGString text_decoration,
            DataType.SVGString unicode_bidi,
            DataType.SVGString word_spacing,
            DataType.SVGString alignment_baseline,
            DataType.SVGString baseline_shift,
            DataType.SVGString dominant_baseline,
            DataType.SVGString glyph_orientation_horizontal,
            DataType.SVGString glyph_orientation_vertical,
            DataType.SVGString kerning,
            DataType.SVGString text_anchor,
            DataType.SVGString writing_mode):this(direction, letter_spacing, 
            text_decoration,unicode_bidi,letter_spacing, alignment_baseline, baseline_shift, dominant_baseline, glyph_orientation_horizontal,
            glyph_orientation_vertical,kerning,text_anchor, writing_mode,Paint.SVGPaint.Empty,DataType.SVGString.Empty)
        {
        }

		TextStyle(
			DataType.SVGString direction,
			DataType.SVGString letter_spacing,
			DataType.SVGString text_decoration,
			DataType.SVGString unicode_bidi,
			DataType.SVGString word_spacing,
			DataType.SVGString alignment_baseline,
			DataType.SVGString baseline_shift,
			DataType.SVGString dominant_baseline,
			DataType.SVGString glyph_orientation_horizontal,
			DataType.SVGString glyph_orientation_vertical,
			DataType.SVGString kerning,
			DataType.SVGString text_anchor,
			DataType.SVGString writing_mode,
            Paint.SVGPaint foreground,
            DataType.SVGString vertical_align)
		{
			this.alignment_baseline = alignment_baseline;
			this.baseline_shift = baseline_shift;
			this.direction = direction;
            this.text_color = foreground;
			this.dominant_baseline = dominant_baseline;
			this.glyph_orientation_horizontal = glyph_orientation_horizontal;
			this.glyph_orientation_vertical = glyph_orientation_vertical;
			this.kerning = kerning;
			this.letter_spacing = letter_spacing;
			this.text_anchor = text_anchor;
			this.text_decoration  = text_decoration;
			this.unicode_bidi = unicode_bidi;
			this.word_spacing = word_spacing;
			this.writing_mode = writing_mode;
            this.vertical_align = vertical_align;
            this.Changed = false;
		}
		#endregion

		#region ..静态变量
		static TextStyle style = new TextStyle(
			new DataType.SVGString("ltr"),
			new DataType.SVGString("normal"),
			new DataType.SVGString("none"),
			new DataType.SVGString("normal"),
			new DataType.SVGString("normal"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("baseline"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("0deg"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("start"),
			new DataType.SVGString("lr-tb"),
            new Paint.SVGPaint("black", null, "black"),
            new DataType.SVGString("center"));


		static TextStyle empty = new TextStyle(
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
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
		public static TextStyle Default
		{
			get
			{
				return style;
			}
		}

		/// <summary>
		/// 获取空对象
		/// </summary>
		public static TextStyle Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..保护属性
		public DataType.SVGString direction;
        public Paint.SVGPaint text_color;
		public DataType.SVGString letter_spacing ;
		public DataType.SVGString text_decoration ;
		public DataType.SVGString unicode_bidi ;
		public DataType.SVGString word_spacing ;
		public DataType.SVGString alignment_baseline ;
		public DataType.SVGString baseline_shift ;
		public DataType.SVGString dominant_baseline ;
		public DataType.SVGString glyph_orientation_horizontal; 
		public DataType.SVGString glyph_orientation_vertical ;
		public DataType.SVGString kerning ;
        public DataType.SVGString vertical_align;
		public DataType.SVGString text_anchor ;
		public DataType.SVGString writing_mode;
        public bool Changed;
		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public TextStyle MutiplyStyle(TextStyle newStyle)
		{
			
			if(this.direction.IsEmpty)
				this.direction = (DataType.SVGString)newStyle.direction;
			else
				this.direction = TypeMultiply.Multiply(this.direction,newStyle.direction);
			if(this.letter_spacing .IsEmpty)
				this.letter_spacing = (DataType.SVGString)newStyle.letter_spacing;
			else
				this.letter_spacing = TypeMultiply.Multiply(this.letter_spacing,newStyle.letter_spacing);
			if(this.text_decoration .IsEmpty)
				this.text_decoration = (DataType.SVGString)newStyle.text_decoration;
			else
				this.text_decoration = TypeMultiply.Multiply(this.text_decoration,newStyle.text_decoration);
			if(this.unicode_bidi .IsEmpty)
				this.unicode_bidi = (DataType.SVGString)newStyle.unicode_bidi;
			else
				this.unicode_bidi = TypeMultiply.Multiply(this.unicode_bidi,newStyle.unicode_bidi);
			if(this.word_spacing .IsEmpty)
				this.word_spacing = (DataType.SVGString)newStyle.word_spacing;
			else
				this.word_spacing = TypeMultiply.Multiply(this.word_spacing,newStyle.word_spacing);
			if(this.alignment_baseline .IsEmpty)
				this.alignment_baseline = (DataType.SVGString)newStyle.alignment_baseline;
			else
				this.alignment_baseline = TypeMultiply.Multiply(this.alignment_baseline,newStyle.alignment_baseline);
			if(this.baseline_shift .IsEmpty)
				this.baseline_shift = (DataType.SVGString)newStyle.baseline_shift;
			else
				this.baseline_shift = TypeMultiply.Multiply(this.baseline_shift,newStyle.baseline_shift);
			if(this.dominant_baseline .IsEmpty)
				this.dominant_baseline = (DataType.SVGString)newStyle.dominant_baseline;
			else
				this.dominant_baseline = TypeMultiply.Multiply(this.dominant_baseline,newStyle.dominant_baseline);

			if(this.glyph_orientation_horizontal .IsEmpty)
				this.glyph_orientation_horizontal = (DataType.SVGString)newStyle.glyph_orientation_horizontal;
			else
				this.glyph_orientation_horizontal = TypeMultiply.Multiply(this.glyph_orientation_horizontal,newStyle.glyph_orientation_horizontal);
			if(this.glyph_orientation_vertical .IsEmpty)
				this.glyph_orientation_vertical =(DataType.SVGString)newStyle.glyph_orientation_vertical;
			else
				this.glyph_orientation_vertical = TypeMultiply.Multiply(this.glyph_orientation_vertical,newStyle.glyph_orientation_vertical);
			if(this.kerning .IsEmpty)
				this.kerning = (DataType.SVGString)newStyle.kerning;
			else
				this.kerning = TypeMultiply.Multiply(this.kerning,newStyle.kerning);
			if(this.text_anchor .IsEmpty)
				this.text_anchor = (DataType.SVGString)newStyle.text_anchor;
			else
				this.text_anchor = TypeMultiply.Multiply(this.text_anchor,newStyle.text_anchor);
			if(this.writing_mode .IsEmpty)
				this.writing_mode = (DataType.SVGString)newStyle.writing_mode;
			else
				this.writing_mode = TypeMultiply.Multiply(this.writing_mode,newStyle.writing_mode);

            if (this.vertical_align.IsEmpty)
                this.vertical_align = (DataType.SVGString)newStyle.vertical_align;
            else
                this.vertical_align = TypeMultiply.Multiply(this.vertical_align, newStyle.vertical_align);

            if (this.text_color.IsEmpty)
                this.text_color = newStyle.text_color;
            else
                this.text_color = TypeMultiply.Multiply(this.text_color, newStyle.text_color);
			return this;
		}
		#endregion

		#region ..克隆
//		public TextStyle Clone()
//		{
//			TextStyle fillstyle = new TextStyle();
//			if(this.direction != null)
//				fillstyle.direction = new DataType.SVGString(this.direction.Value);
//			if(this.letter_spacing != null)
//				fillstyle.letter_spacing = new DataType.SVGString(this.letter_spacing.Value);
//			if(this.text_decoration != null)
//				fillstyle.text_decoration = new DataType.SVGString(this.text_decoration.Value);
//			if(this.unicode_bidi != null)
//				fillstyle.unicode_bidi = new DataType.SVGString(this.unicode_bidi.Value);
//			if(this.word_spacing != null)
//				fillstyle.word_spacing = new DataType.SVGString(this.word_spacing.Value);
//			if(this.alignment_baseline != null)
//				fillstyle.alignment_baseline = new DataType.SVGString(this.alignment_baseline.Value);
//			if(this.baseline_shift != null)
//				fillstyle.baseline_shift = new DataType.SVGString(this.baseline_shift.Value);
//			if(this.dominant_baseline != null)
//				fillstyle.dominant_baseline = new DataType.SVGString(this.dominant_baseline.Value);
//
//			if(this.glyph_orientation_horizontal != null)
//				fillstyle.glyph_orientation_horizontal = new DataType.SVGString(this.glyph_orientation_horizontal.Value);
//			if(this.alignment_baseline != null)
//				fillstyle.glyph_orientation_vertical = new DataType.SVGString(this.glyph_orientation_vertical.Value);
//			if(this.kerning != null)
//				fillstyle.kerning = new DataType.SVGString(this.kerning.Value);
//			if(this.text_anchor != null)
//				fillstyle.text_anchor = new DataType.SVGString(this.text_anchor.Value);
//			if(this.writing_mode != null)
//				fillstyle.writing_mode = new DataType.SVGString(this.writing_mode.Value);
//			return fillstyle;
//		}
		#endregion
	}
}
