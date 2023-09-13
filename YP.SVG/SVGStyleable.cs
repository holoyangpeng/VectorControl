using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace YP.SVG
{
	/// <summary>
	/// 实现一类节点，这一类节点都具有Style属性
	/// </summary>
	public abstract class SVGStyleable:YP.SVG.SVGElement,Interface.ISVGStylable,Base.Interface.IStyleElement
	{
		#region ..构造及消除
		public SVGStyleable(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			//			this.keyPointList = new DataType.KeyPoint.KeyPointInfoList(this);
			//			this.activeTime.OwnerElement = this;

			this.fillStyle = YP.SVG.StyleContainer.FillStyle.Empty;//(this);
			this.strokeStyle = YP.SVG.StyleContainer.StrokeStyle.Empty;//new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
			this.visualMediaStyle =YP.SVG.StyleContainer.VisualMediaStyle.Empty;// new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
			this.clipStyle = YP.SVG.StyleContainer.ClipStyle.Empty;//new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedClipStyle(this);
			this.fontStyle = YP.SVG.StyleContainer.FontStyle.Empty;//new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
			this.colorAndPaintStyle = YP.SVG.StyleContainer.ColorAndPaintStyle.Empty;// new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
			this.textStyle = YP.SVG.StyleContainer.TextStyle.Empty;//new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
			this.shadowStyle = YP.SVG.StyleContainer.ShadowStyle.Default;
			//			this.keyPointList.Removed += new YP.Base.EventCollectionBase.CollectionEventHandler(keyPointList_Removed);
			//			this.keyPointList.Inserted +=new YP.Base.EventCollectionBase.CollectionEventHandler(keyPointList_Inserted);
		}
		#endregion

		#region ..私有变量
		Hashtable styleProperties = new Hashtable();
		StyleContainer.FillStyle fillStyle;//new StyleContainer.AnimatedStyle.AnimatedFillStyle();
		StyleContainer.StrokeStyle strokeStyle;// StyleContainer.AnimatedStyle.AnimatedStrokeStyle();
		StyleContainer.VisualMediaStyle visualMediaStyle;// StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle();
		StyleContainer.ClipStyle clipStyle;// StyleContainer.AnimatedStyle.AnimatedClipStyle();
		StyleContainer.ColorAndPaintStyle colorAndPaintStyle ;//new StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle();
		StyleContainer.FontStyle fontStyle;//new StyleContainer.AnimatedStyle.AnimatedFontStyle();
		StyleContainer.TextStyle textStyle;//new StyleContainer.AnimatedStyle.AnimatedTextStyle();
		StyleContainer.ShadowStyle shadowStyle;
		public bool NeedUpdateCSSStyle = false;

		YP.Base.CSS.CSSRuleSetContent cssContent = new Base.CSS.CSSRuleSetContent();
        //-2:从未被绘制
        //-1:被绘制，需要被刷新
        //0:缓存状态
		int currentTime = -2;
		//		bool applyStyle = true;
		public Hashtable styleInfos = new Hashtable();
		YP.SVG.StyleContainer.StyleContainer _styleContainer = null;//new YP.SVGDom.StyleContainer.StyleContainer();
		/// <summary>
		/// 记录影响对象要素的动画对象列表
		/// </summary>
		YP.SVG.SVGElementCollection animateList = new SVGElementCollection();
		/// <summary>
		/// 当对象作为图元或图元元素被引用时，记录其对应的引用对象
		/// </summary>
		public YP.SVG.SVGStyleable refedElement = null;
		bool applyStyle = false;
		ArrayList changeAttributes = new ArrayList();
		ClickAction action = new ClickAction(ActionType.None,string.Empty);
		System.Collections.Hashtable styleNames = new Hashtable();
		ViewStyle viewStyle = ViewStyle.None;
		#endregion

		#region ..保护属性
        /// <summary>
        /// 对象是否处于激活状态
        /// 所谓激活状态就是已经被加入绘制序列
        /// </summary>
        public override bool IsActive
        {
            get
            {
                return this.CurrentTime >= -1;
            }
        }

		public virtual StyleContainer.StyleContainer StyleContainer
		{
			set
			{
				this._styleContainer = value;
			}
			get
			{
				return this._styleContainer;
			}
		}

		/// <summary>
		/// 获取填充信息
		/// </summary>
		public YP.SVG.StyleContainer.FillStyle FillStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.fillStyle;
			}
		}

		/// <summary>
		/// gets or sets the view style of the element
		/// </summary>
		public ViewStyle ViewStyle
		{
			get
			{
				return this.viewStyle;
			}
			set
			{
				this.viewStyle = value;
			}
		}

		/// <summary>
		/// 获取画笔填充信息
		/// </summary>
		public YP.SVG.StyleContainer.StrokeStyle StrokeStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.strokeStyle;
			}
		}

		public virtual bool HasShadow
		{
			get
			{
				return false;
			}
		}

		public ClickAction Action
		{
			get
			{
				return this.action ;
			}
		}

		/// <summary>
		/// 获取或设置当前对象在时间轴上所处的位置
		/// </summary>
		public int CurrentTime
		{
			get
			{
				return this.currentTime;
			}
			set
			{
                this.currentTime = value;
			}
		}

		/// <summary>
		/// 获取普通信息
		/// </summary>
		public YP.SVG.StyleContainer.VisualMediaStyle VisualMediaStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.visualMediaStyle;
			}
		}

		/// <summary>
		/// 获取颜色和绘制信息信息
		/// </summary>
		public YP.SVG.StyleContainer.ColorAndPaintStyle ColorAndPaintStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.colorAndPaintStyle;
			}
		}

		/// <summary>
		/// 获取文本信息
		/// </summary>
		public YP.SVG.StyleContainer.TextStyle TextStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.textStyle;
			}
		}

		/// <summary>
		/// 获取Shadow信息
		/// </summary>
		public YP.SVG.StyleContainer.ShadowStyle ShadowStyle
		{
			get
			{
				return this.shadowStyle;
			}
		}

		/// <summary>
		/// 获取字体信息
		/// </summary>
		public YP.SVG.StyleContainer.FontStyle FontStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.fontStyle;
			}
		}

		/// <summary>
		/// 获取剪切信息
		/// </summary>
		public YP.SVG.StyleContainer.ClipStyle ClipStyle
		{
			get
			{
				this.ApplyCSSStyle();
				return this.clipStyle;
			}
		}
		#endregion

        #region ..GetFinalAttributValue
        /// <summary>
		/// 获取指定属性在这一层用户空间的值(不可累加)
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <returns></returns>
		public string GetFinalAttributValue(string attributeName)
		{
			string attr = this.GetAttribute(attributeName);
			
			if(attr.Length == 0|| string.Compare(attr, "inherit")==0)
			{
				if(ParentNode is SVGStyleable)
				{
					attr = ((SVGStyleable)ParentNode).GetFinalAttributValue(attributeName);
				}
			}	
			return attr;
		}
		#endregion

        #region ..SetSVGAttribute
        /// <summary>
		/// 当属性发生修改活着添加时，更新对象属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <param name="attributeValue">属性值</param>
		public override void SetSVGAttribute(string attributeName,string attributeValue)
		{
            try
            {
                switch (attributeName)
                {
                    #region ..StrokeStyle
                    case "stroke":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.svgStroke = new Paint.SVGPaint(attributeValue, this, "none");
                        //					this.strokeStyle.svgStroke.IsEmpty = false;
                        break;
                    case "stroke-opacity":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.strokeOpacity = new DataType.SVGNumber(attributeValue, "1");//,this);
                        //					this.strokeStyle.strokeOpacity.IsEmpty = false;
                        break;
                    case "stroke-width":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.strokewidth = new DataType.SVGNumber(attributeValue, "1");//,this);
                        //					this.strokeStyle.strokewidth.IsEmpty = false;
                        break;
                    case "stroke-dashoffset":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.stroke_dashoffset = new DataType.SVGLength(attributeValue, this, LengthDirection.Viewport);//,"0");
                        //					this.strokeStyle.stroke_dashoffset.IsEmpty = false;
                        break;
                    case "stroke-miterlimit":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.stroke_miterlimit = new DataType.SVGNumber(attributeValue, "4");//,this);
                        //					this.strokeStyle.stroke_miterlimit.IsEmpty = false;
                        break;
                    case "stroke-linejoin":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.stroke_linejoin = new DataType.SVGString(attributeValue, "miter");//,this);
                        //					this.strokeStyle.stroke_linejoin.IsEmpty = false;
                        break;
                    case "stroke-linecap":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.stroke_linecap = new DataType.SVGString(attributeValue, "butt"); ;//,this);
                        //					this.strokeStyle.stroke_linecap.IsEmpty = false;
                        break;
                    case "stroke-dasharray":
                        //					if(this.strokeStyle == null)
                        //						this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
                        this.strokeStyle.stroke_dasharray = new DataType.SVGString(attributeValue, "none");//"none",this);
                        //					this.strokeStyle.stroke_dasharray.IsEmpty = false;
                        break;
                    case "stroke_hatchcolor":
                        if (attributeValue == null || attributeValue.Trim().Length == 0)
                            this.strokeStyle.stroke_hatchColor = new SVG.DataType.RGBColor(Color.Black);
                        else
                            this.strokeStyle.stroke_hatchColor = new SVG.DataType.RGBColor(attributeValue);
                        break;
                    case "stroke-hatchstyle":
                        this.strokeStyle.stroke_hatchStyle = new SVG.DataType.SVGString(attributeValue, "none");
                        break;
                    case "stroke-gradientmode":
                        this.strokeStyle.stroke_gradientMode = new SVG.DataType.SVGString(attributeValue, "auto");
                        break;
                    #endregion

                    #region ..FillStyle
                    case "fill-opacity":
                        if (attributeValue.Length == 0)
                            this.fillStyle.fillOpacity = new DataType.SVGNumber(1);
                        else
                            this.fillStyle.fillOpacity = new DataType.SVGNumber(attributeValue, "1");//,this);
                        break;
                    case "fill":
                        this.fillStyle.svgPaint = new Paint.SVGPaint(attributeValue, this, "black");
                        break;
                    case "fill-rule":
                        //					if(this.fillStyle == null)
                        //						this.fillStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFillStyle(this);
                        this.fillStyle.fillrule = new DataType.SVGString(attributeValue, "nonzero");//,this);
                        //					this.fillStyle.fillrule.IsEmpty = false;
                        break;
                    case "hatchcolor":
                        if (attributeValue == null || attributeValue.Trim().Length == 0)
                            this.fillStyle.HatchColor = new YP.SVG.DataType.RGBColor(Color.Black);
                        else
                            this.fillStyle.HatchColor = new YP.SVG.DataType.RGBColor(attributeValue);
                        break;
                    case "hatchstyle":
                        this.fillStyle.HatchStyle = new YP.SVG.DataType.SVGString(attributeValue, "none");
                        break;
                    #endregion

                    #region ..VisualMedia
                    case "visibility":
                        //					if(this.visualMediaStyle == null)
                        //						this.visualMediaStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
                        this.visualMediaStyle.visiblility = new DataType.SVGString(attributeValue);//,"visible",this);
                        //					this.visualMediaStyle.visiblility.IsEmpty = false;
                        break;
                    case "color":
                        //					if(this.visualMediaStyle == null)
                        //						this.visualMediaStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
                        break;
                    case "display":
                        //					if(this.visualMediaStyle == null)
                        //						this.visualMediaStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
                        this.visualMediaStyle.display = new DataType.SVGString(attributeValue);//,"block",this);
                        break;
                    case "overflow":
                        //					if(this.visualMediaStyle == null)
                        //						this.visualMediaStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
                        this.visualMediaStyle.overflow = new DataType.SVGString(attributeValue);//,"visible",this);
                        //					this.visualMediaStyle.overflow.IsEmpty = false;
                        break;
                    case "clip":
                        //					if(this.visualMediaStyle == null)
                        //						this.visualMediaStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
                        this.visualMediaStyle.color = new DataType.SVGString(attributeValue);//,"auto",this);
                        //					this.visualMediaStyle.color.IsEmpty = false;
                        break;
                    case "wrap":
                        this.visualMediaStyle.wrap = new DataType.SVGString(attributeValue);
                        break;
                    #endregion

                    #region ..FontStyle
                    case "font":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.Font = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.fontStyle.Font.IsEmpty = false;
                        break;
                    case "font-family":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.FontFamily = new DataType.SVGString(attributeValue, "Arial");//,this);
                        //					this.fontStyle.FontFamily.IsEmpty = false;
                        break;
                    case "font-size":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.FontSize = new DataType.SVGLength(attributeValue, this, LengthDirection.Viewport, "12");//);
                        //					this.fontStyle.FontSize.IsEmpty = false;
                        break;
                    case "font-size-adjust":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.FontSizeAdjust = new DataType.SVGNumber(attributeValue, "0");//,this);
                        //					this.fontStyle.FontSizeAdjust.IsEmpty = false;
                        break;
                    case "font-stretch":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.FontStretch = new DataType.SVGString(attributeValue, "normal");//,this);
                        //					this.fontStyle.FontStretch.IsEmpty = false;
                        break;
                    case "font-style":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.Font_Style = new DataType.SVGString(attributeValue, "normal");//,this);
                        //					this.fontStyle.Font_Style.IsEmpty = false;
                        break;
                    case "font-variant":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.FontVariant = new DataType.SVGString(attributeValue, "normal");//,this);
                        //					this.fontStyle.FontVariant.IsEmpty = false;
                        break;
                    case "font-weight":
                        //					if(this.fontStyle == null)
                        //						this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
                        this.fontStyle.FontWeigth = new DataType.SVGString(attributeValue, "normal");//,this);
                        //					this.fontStyle.FontWeigth.IsEmpty = false;
                        break;
                    #endregion

                    #region ..TextStyle
                    case "direction":
                        this.textStyle.direction = new DataType.SVGString(attributeValue, "ltr");//,this);
                        this.textStyle.Changed = true;
                        break;
                    case "letter-spacing":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.letter_spacing = new DataType.SVGString(attributeValue, "normal");//,this);
                        //					this.textStyle.letter_spacing.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "text-decoration":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.text_decoration = new DataType.SVGString(attributeValue, "none");//,this);
                        //					this.textStyle.text_decoration.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "unicode-bidi":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.unicode_bidi = new DataType.SVGString(attributeValue, "normal");//,this);
                        this.textStyle.Changed = true;
                        break;
                    case "word-spacing":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.word_spacing = new DataType.SVGString(attributeValue, "normal");//,this);
                        //					this.textStyle.word_spacing.IsEmpty = false;
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.Changed = true;
                        break;
                    case "alignment-baseline":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.alignment_baseline = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.textStyle.alignment_baseline.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "baseline-shift":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.baseline_shift = new DataType.SVGString(attributeValue, "baseline");//,this);
                        //					this.textStyle.baseline_shift.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "dominant-baseline":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.dominant_baseline = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.textStyle.dominant_baseline.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "glyph-orientation-horizontal":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.glyph_orientation_vertical = new DataType.SVGString(attributeValue, "0deg");//,this);
                        //					this.textStyle.glyph_orientation_vertical.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "glyph-orientation-vertical":
                        //					if(this.textStyle == null)
                        //						this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
                        this.textStyle.glyph_orientation_vertical = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.textStyle.glyph_orientation_vertical.IsEmpty = false;
                        this.textStyle.Changed = true;
                        break;
                    case "kerning":
                        this.textStyle.kerning = new DataType.SVGString(attributeValue, "auto");//,this);
                        this.textStyle.Changed = true;
                        break;
                    case "text-anchor":
                        this.textStyle.text_anchor = new DataType.SVGString(attributeValue, "start");//,this);
                        this.textStyle.Changed = true;
                        if (this.IsActive && this.StyleContainer != null)
                        {
                            var style = this.StyleContainer.TextStyle;
                            style.text_anchor = this.textStyle.text_anchor;
                            this.StyleContainer.TextStyle = style;
                        }
                        break;
                    case "writing-mode":
                        this.textStyle.writing_mode = new DataType.SVGString(attributeValue, "lr-tb");//,this);
                        this.textStyle.Changed = true;
                        break;
                    case "vertical-align":
                        this.textStyle.vertical_align = new DataType.SVGString(attributeValue, "middle");
                        this.textStyle.Changed = true;
                        if (this.IsActive && this.StyleContainer != null)
                        {
                            var style = this.StyleContainer.TextStyle;
                            style.vertical_align = this.textStyle.vertical_align;
                            this.StyleContainer.TextStyle = style;
                        }
                        break;
                    case "text-color":
                        this.textStyle.text_color = new Paint.SVGPaint(attributeValue, this, "black");
                        this.textStyle.Changed = true;
                        break;
                    #endregion

                    #region ..ClipStyle
                    case "opacity":
                        //					if(this.clipStyle == null)
                        //						this.clipStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedClipStyle(this);
                        this.clipStyle.opacity = new DataType.SVGNumber(attributeValue, "1");//,this);
                        //					this.clipStyle.opacity.IsEmpty = false;
                        break;
                    case "clip-path":
                        //					if(this.clipStyle == null)
                        //						this.clipStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedClipStyle(this);
                        this.clipStyle.clip_path = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.clipStyle.clip_path.IsEmpty = false;
                        break;

                    case "clip-rule":
                        //					if(this.clipStyle == null)
                        //						this.clipStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedClipStyle(this);
                        this.clipStyle.clip_rule = new DataType.SVGString(attributeValue, "nonzero");//,this);
                        //					this.clipStyle.clip_rule.IsEmpty = false;
                        break;
                    case "mask":
                        //					if(this.clipStyle == null)
                        //						this.clipStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedClipStyle(this);
                        this.clipStyle.mask = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.clipStyle.mask.IsEmpty = false;
                        break;
                    #endregion

                    #region ..Color And Paint
                    case "color-interpolation":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.color_interpolation = new DataType.SVGString(attributeValue, "sRGBlinearRGB");//,this);
                        //					this.colorAndPaintStyle.color_interpolation.IsEmpty = false;
                        break;
                    case "color-interpolation-filters":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.color_interpolation_filters = new DataType.SVGString(attributeValue, "linearRGB");//,this);
                        //					this.colorAndPaintStyle.color_interpolation_filters.IsEmpty = false;
                        break;
                    case "color-profile":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.color_profile = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.colorAndPaintStyle.color_profile.IsEmpty = false;
                        break;
                    case "color-rendering":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.color_rendering = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.colorAndPaintStyle.color_rendering.IsEmpty = false;
                        break;
                    case "image-rendering":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.image_rendering = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.colorAndPaintStyle.image_rendering.IsEmpty = false;
                        break;
                    case "marker":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.marker = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.colorAndPaintStyle.marker.IsEmpty = false;
                        break;
                    case "marker-end":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.marker_end = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.colorAndPaintStyle.marker_end.IsEmpty = false;
                        break;
                    case "marker-mid":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.marker_mid = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.colorAndPaintStyle.marker_mid.IsEmpty = false;
                        break;
                    case "marker-start":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.marker_start = new DataType.SVGString(attributeValue);//,string.Empty,this);
                        //					this.colorAndPaintStyle.marker_start.IsEmpty = false;
                        break;
                    case "text-rendering":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.text_rendering = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.colorAndPaintStyle.text_rendering.IsEmpty = false;
                        break;
                    case "shape-rendering":
                        //					if(this.colorAndPaintStyle == null)
                        //						this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
                        this.colorAndPaintStyle.shape_rendering = new DataType.SVGString(attributeValue, "auto");//,this);
                        //					this.colorAndPaintStyle.shape_rendering.IsEmpty = false;
                        break;

                    #endregion

                    #region ..CSSStyle
                    case "style":
                        this.cssContent = new Base.CSS.CSSRuleSetContent(attributeValue);
                        break;
                    #endregion

                    case "class":
                        if (attributeValue != null && attributeValue.Trim().Length > 0)
                            this.styleNames.Clear();

                        this.applyStyle = false;
                        break;

                    #region ..ShadowStyle
                    case "shadow":
                        this.shadowStyle.DrawShadow = attributeValue.ToLower().ToLower() == "true";
                        break;
                    case "shadowColor":
                        this.shadowStyle.ShadowColor = new DataType.SVGColor(attributeValue, this, "gray").RgbColor.GDIColor;
                        break;
                    case "xOffset":
                        this.shadowStyle.XOffset = (int)(new DataType.SVGNumber(attributeValue, "5").Value);
                        break;
                    case "yOffset":
                        this.shadowStyle.YOffset = (int)(new DataType.SVGNumber(attributeValue, "5").Value);
                        break;
                    case "shadowOpacity":
                        this.shadowStyle.Opacity = (float)(new DataType.SVGNumber(attributeValue, "0.5").Value);
                        break;
                    #endregion

                    #region ..动作设置
                    case "action":
                        this.action = ClickAction.Parse(attributeValue);
                        break;
                    #endregion

                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new string[] { e1.Message }, YP.SVG.ExceptionLevel.Normal));
            }
		}
		#endregion

        #region ..ApplyCSSStyle
        /// <summary>
		/// 应用CSS属性
		/// </summary>
		public void ApplyCSSStyle()
		{
			if(!this.applyStyle || this.OwnerDocument.CSSStyleChanged)
			{
				Base.CSS.CSSRuleSetContent content = new Base.CSS.CSSRuleSetContent();
				this.OwnerDocument.MatchStyleable(this,content);
			
				foreach(string name in this.styleNames.Keys)
				{
					this.SetSVGAttribute(name,this.styleNames[name] as string);
				}

				string[] names = content.PropertyNames;
				foreach(string name in names)
				{
					string oriValue = null;
					if(this.HasAttribute(name))
						oriValue = this.GetAttribute(name);
					this.styleNames[name] = oriValue;
					this.SetSVGAttribute(name,content.GetProperty(name));
				}

				names = this.cssContent.PropertyNames ;
				foreach(string name in names)
				{
					this.SetSVGAttribute(name,this.cssContent.GetProperty(name));
				}
				names = null;
				content = null;
				this.NeedUpdateCSSStyle = true;
			}
			this.applyStyle = true;
		}
		#endregion

        #region ..MatchXPath
        /// <summary>
		/// 判断节点是否和指定xpath匹配
		/// </summary>
		/// <param name="xpath"></param>
		public bool MatchXPath(string xpath)
		{
			this.OwnerDocument.BeginProcess();
			System.Xml.XPath.XPathNavigator nav = this.CreateNavigator();
			
			System.Xml.XPath.XPathExpression spath = nav.Compile(xpath);
			this.OwnerDocument.EndProcess();
			return nav.Matches(spath);
		}
		#endregion

		#region ..剪切画布
		/// <summary>
		/// 剪切画布
		/// </summary>
		/// <param name="g">剪切画布</param>
		/// <param name="sp"></param>
		public void Clip(Graphics g,YP.SVG.StyleContainer.StyleOperator sp)
		{
			if(this.GetAttribute("clip-path").Trim().Length > 0)
			{
                string clipstr = this.GetAttribute("clip-path").Trim().Substring(4);//.ToString(0,4);
				int index = clipstr.IndexOf(")");
				if(index>=0)
				{
					clipstr = clipstr.Substring(0,index);
					System.Xml.XmlNode node = this.OwnerDocument.GetReferencedNode(clipstr,new string[]{"clipPath"});
					if(node is YP.SVG.ClipAndMask.SVGClipPathElement )
					{
						YP.SVG.ClipAndMask.SVGClipPathElement clipelement = (YP.SVG.ClipAndMask.SVGClipPathElement)node;
						this.Clip(clipelement,g,sp);
					}
				}
			}
		}

		/// <summary>
		/// 将指定剪切对象应用到画布
		/// </summary>
		/// <param name="clipelement">剪切对象</param>
		/// <param name="g">画布</param>
		/// <param name="sp">类型容器</param>
        public void Clip(YP.SVG.ClipAndMask.SVGClipPathElement clipelement, Graphics g, YP.SVG.StyleContainer.StyleOperator sp)
		{
			if(clipelement == null)
				return;
			GraphicsPath gp = (clipelement as SVG.Interface.ISVGPathable).GPath;
			string clip_rule = clipelement.GetAttribute("clip-rule");
			if(clip_rule.Length == 0 || string.Compare(clip_rule,"inherit")==0)
				clip_rule = sp.ClipStyle.clip_rule.Value;
			if(string.Compare(clip_rule,"evenodd")==0)
				gp.FillMode = FillMode.Alternate;
			else
				gp.FillMode = FillMode.Winding;
			clip_rule = null;
			YP.SVG.SVGUnitType pathUnits = (YP.SVG.SVGUnitType)clipelement.ClipPathUnits ;
            if (pathUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX)
            {
                YP.SVG.Interface.DataType.ISVGRect bound = ((YP.SVG.Interface.ISVGLocatable)this).GetOriBBox();
                float fLeft = bound.X;
                float fTop = bound.Y;
                float boundsWidth = bound.Width;
                float boundsHeight = bound.Height;

                // scale clipping path
                using (Matrix matrix = new Matrix())
                {
                    matrix.Scale(boundsWidth, boundsHeight);
                    gp.Transform(matrix);
                    g.SetClip(gp);
                }

                // offset clip
                g.TranslateClip(fLeft, fTop);
            }
            else
            {
                using (GraphicsPath gp1 = gp.Clone() as GraphicsPath)
                {
                    if (this is SVGTransformableElement)
                        gp1.Transform((this as SVGTransformableElement).TotalTransform);
                    //当绘制use时，需要进行变换
                    if (sp.UseCoordTransform && sp.coordTransform != null)
                        gp1.Transform(sp.coordTransform);
                    g.SetClip(gp1);
                }
            }
			this.Clip(clipelement.ClipElement,g,sp);
		}
		#endregion

		#region ..UpdateElement
		/// <summary>
		/// 节点发生改变时，更新节点
		/// </summary>
		public virtual void UpdateElement()
		{
            if (this.CurrentTime == -2)
                return;
			this.CurrentTime = -1;
		}

		/// <summary>
		/// 节点发生改变时，更形节点
		/// </summary>
		/// <param name="updateParent"></param>
        public void UpdateElement(bool updateParent)
		{
			this.UpdateElement();
			YP.SVG.SVGStyleable parent = this.ParentElement as YP.SVG.SVGStyleable;
			while(parent != null && updateParent)
			{
				parent.UpdateElement();
				parent = parent.ParentElement as YP.SVG.SVGStyleable;
			}
		}

        /// <summary>
        /// 当attribute发生改变时，更新节点
        /// </summary>
        /// <param name="attributeName"></param>
        public virtual void UpdateElementWithAttribute(string attributeName)
        {
            this.UpdateElement();
        }
		#endregion

        #region ..UpdateAttribute
        /// <summary>
		/// 在当前文档时间里更改属性或生成相应的动画
		/// </summary>
		/// <param name="attributename">指定的属性名称</param>
		/// <param name="attributeValue">属性值</param>
		public void UpdateAttribute(string attributename,string attributevalue)
		{
			bool adaptattri = true;
			YP.SVG.Document.SVGDocument doc = this.OwnerDocument;
			bool old = doc.AcceptNodeChanged;
			//更改属性
			if(adaptattri)
			{
				doc.AcceptNodeChanged = true;
				this.InternalSetAttribute(attributename,attributevalue);
			}

			doc.AcceptNodeChanged = old;
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            switch (attributeName)
            {
                case "stroke":
                case "stroke-opacity":
                case "stroke-width":
                case "stroke-dashoffset":
                case "stroke-miterlimit":
                case "stroke-linejoin":
                case "stroke-linecap":
                case "stroke-dasharray":
                case "fill-opacity":
                case "fill":
                case "fill-rule":
                case "hatchcolor":
                case "hatchstyle":
                case "visibility":
                case "color":
                case "display":
                case "overflow":
                case "clip":
                case "glyph-orientation-horizontal":
                case "glyph-orientation-vertical":
                case "kerning":
                case "opacity":
                case "clip-path":
                case "clip-rule":
                case "mask":
                case "color-interpolation":
                case "color-interpolation-filters":
                case "color-profile":
                case "color-rendering":
                case "image-rendering":
                case "marker":
                case "marker-end":
                case "marker-mid":
                case "marker-start":
                case "text-rendering":
                case "shape-rendering":
                case "style":
                case "class":
                case "shadow":
                case "shadowColor":
                case "xOffset":
                case "yOffset":
                case "shadowOpacity":
                    return AttributeChangedResult.VisualChanged;
                case "writing-mode":
                case "text-anchor":
                case "vertical-align":
                case "font":
                case "font-family":
                case "font-size":
                case "font-size-adjust":
                case "font-stretch":
                case "font-style":
                case "font-variant":
                case "font-weight":
                case "direction":
                case "letter-spacing":
                case "text-decoration":
                case "unicode-bidi":
                case "word-spacing":
                case "alignment-baseline":
                case "baseline-shift":
                case "dominant-baseline":
                    if (this is SVG.Interface.Text.ITextElement)
                        return AttributeChangedResult.GraphicsPathChanged;
                    return AttributeChangedResult.VisualChanged;
            }
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..AfterAttributeChanged
        public override void AfterAttributeChanged(string attributeName)
        {
            AttributeChangedResult result = this.AttributeChangeTest(attributeName);
            if ((result) != AttributeChangedResult.NoVisualChanged)
            {
                this.UpdateElement(true);
                this.UpdateElementWithAttribute(attributeName);
                this.OwnerDocument.RefreshElement(this);
            }
        }
        #endregion

        #region ..GetFinalAttribute
        public override string GetFinalAttribute(string attributeName)
        {
            if (this.cssContent != null && this.cssContent.ContainsProperty(attributeName))
                return this.cssContent.GetProperty(attributeName);
            return base.GetFinalAttribute(attributeName);
        }
        #endregion

        #region ..SetFinalAttribute
        public override void SetFinalAttribute(string attributeName, string attributeValue)
        {
            if (this.cssContent != null && this.cssContent.ContainsProperty(attributeName))
            {
                this.cssContent.SetProperty(attributeName, attributeValue, "", 2);
                this.InternalSetAttribute("style", this.cssContent.CSSText);
            }
            else
                base.SetFinalAttribute(attributeName, attributeValue);
        }
        #endregion

        public override System.Xml.XmlNode CloneNode(bool deep)
        {
            SVGStyleable style = base.CloneNode(deep) as SVGStyleable;
            if (this.StyleContainer != null)
                style.StyleContainer = new StyleContainer.StyleContainer(this.StyleContainer);
            return style;
        }

        #region ..getvalue for the custom property
        /// <summary>
        /// gets the value for the custom property
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        internal override object GetPropertyValue(string attributeName)
        {
            StyleContainer.FillStyle fillstyle = this.FillStyle;
            if (this.StyleContainer != null)
                fillstyle = this.StyleContainer.FillStyle;

            StyleContainer.StrokeStyle stroke = this.StrokeStyle;
            if (this.StyleContainer != null && this.StyleContainer.FillStyle.HatchColor != null)
                stroke = this.StyleContainer.StrokeStyle;

            StyleContainer.ShadowStyle shadow = this.ShadowStyle;
            if (this.StyleContainer != null)
                shadow = this.StyleContainer.ShadowStyle;

            StyleContainer.FontStyle font = this.FontStyle;
            if (this.StyleContainer != null && !this.StyleContainer.FontStyle.FontFamily.IsEmpty)
                font = this.StyleContainer.FontStyle;

            StyleContainer.TextStyle textStyle = this.TextStyle;
            if (this.StyleContainer != null && !this.StyleContainer.TextStyle.text_decoration.IsEmpty)
                textStyle = this.StyleContainer.TextStyle;
            switch (attributeName)
            {
                #region ..fill-style
                case "fill":
                    if (fillstyle.svgPaint.PaintType == (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR)
                        return ((DataType.RGBColor)fillstyle.svgPaint.RgbColor).GDIColor;
                    else if (fillstyle.svgPaint.PaintType == (ulong)PaintType.SVG_PAINTTYPE_NONE)
                        return Color.Empty;
                    return Color.White;
                case "fill-opacity":
                    if (!((DataType.SVGNumber)fillstyle.fillOpacity).IsEmpty)
                        return fillstyle.fillOpacity.Value;
                    return 1f;
                case "hatchcolor":
                    if (fillstyle.HatchColor != null)
                        return fillstyle.HatchColor.GDIColor;
                    return Color.Black;
                #endregion

                #region ..stroke-style
                case "stroke":
                    if (stroke.svgStroke.PaintType == (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR && stroke.svgStroke.RgbColor != null)
                        return ((DataType.RGBColor)stroke.svgStroke.RgbColor).GDIColor;
                    else if (stroke.svgStroke.PaintType == (ulong)PaintType.SVG_PAINTTYPE_NONE)
                        return Color.Empty;
                    return Color.Black;
                case "stroke-width":
                    if (!stroke.strokewidth.IsEmpty)
                        return stroke.strokewidth.Value;
                    return 1f;
                case "stroke-opacity":
                    if (!((DataType.SVGNumber)stroke.strokeOpacity).IsEmpty)
                        return stroke.strokeOpacity.Value;
                    return 1f;
                case "stroke-dasharray":
                    if (!stroke.stroke_dasharray.IsEmpty)
                        return stroke.stroke_dasharray.Value;
                    break;
                #endregion

                #region ..shadow style
                case "shadow":
                    return shadow.DrawShadow;
                case "shadowColor":
                    return shadow.ShadowColor;
                case "shadowOpacity":
                    return shadow.Opacity;
                case "xOffset":
                    return shadow.XOffset;
                case "yOffset":
                    return shadow.YOffset;
                #endregion

                #region ..FontStyle
                case "font-family":
                    if (!font.FontFamily.IsEmpty)
                        return font.FontFamily.Value;
                    return "宋体";
                case "font-size":
                    if (!font.FontSize.IsEmpty)
                        return font.FontSize.Value;
                    return 12;
                case "font-weight":
                    if (!font.FontWeigth.IsEmpty)
                    {
                        string a = font.FontWeigth.Value;
                        bool b = string.Compare(a, "bold") == 0 || string.Compare(a, "bolder") == 0;
                        return b;
                    }
                    return false;
                case "font-style":
                    if (!font.Font_Style.IsEmpty)
                    {
                        string a = font.Font_Style.Value;
                        bool b = string.Compare(a, "italic") == 0;
                        return b;
                    }
                    return false;
                case "text-decoration":
                    if (!textStyle.text_decoration.IsEmpty)
                    {
                        string a = textStyle.text_decoration.Value;
                        bool b = string.Compare(a, "underline") == 0;
                        return b;
                    }
                    return false;
                #endregion

                #region ..文本块
                case "text-color":
                    if (!textStyle.text_color.IsEmpty)
                    {
                        if (textStyle.text_color.PaintType == (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR)
                            return ((DataType.RGBColor)textStyle.text_color.RgbColor).GDIColor;
                        else if (textStyle.text_color.PaintType == (ulong)PaintType.SVG_PAINTTYPE_NONE)
                            return Color.Empty;
                    }
                    return Color.Black;
                case "text-anchor":
                    if (!textStyle.text_anchor.IsEmpty && textStyle.text_anchor.Value != null)
                    {
                        switch (textStyle.text_anchor.Value.ToLower())
                        {
                            case "start":
                                return Alignment.Left;
                            case "end":
                                return Alignment.Right;
                        }
                    }
                    return Alignment.Center;
                case "vertical-align":
                    if (!textStyle.vertical_align.IsEmpty && textStyle.vertical_align.Value != null)
                    {
                        switch (textStyle.vertical_align.Value.ToLower())
                        {
                            case "top":
                                return VerticalAlignment.Top;
                            case "bottom":
                                return VerticalAlignment.Bottom;
                        }
                    }
                    return VerticalAlignment.Middle;
                    #endregion
            }
            return base.GetPropertyValue(attributeName);
        }
        #endregion

        #region ..setvalue for the custom property
        /// <summary>
        /// set the value for the custom property
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        internal override void SetPropertyValue(string attributeName, object value)
        {
            ArrayList list = new ArrayList(new string[] { "rect", "use", "polygon", "circle", "connect", "text", "tspan", "ellipse", "path", "use", "textBlock" });
            string valueStr = string.Empty;
            float fValue = 1;
            switch (attributeName)
            {
                #region ..fill-style
                case "fill":
                    valueStr = ColorHelper.GetColorStringInHex((Color)value);
                    UpdateElementAttribute(this, "white", "fill", valueStr, list);
                    break;
                case "fill-opacity":
                    fValue = (float)value;
                    if (fValue < 0 || fValue > 1)
                    {
                        throw new Exception("请输入0和1之间的数字！");
                    }
                    UpdateElementAttribute(this, "1", "fill-opacity", value.ToString(), list);
                    break;
                case "hatchcolor":
                    valueStr = ColorHelper.GetColorStringInHex((Color)value);
                    UpdateElementAttribute(this, "black", "hatchcolor", valueStr, list);
                    break;
                #endregion

                #region ..stroke-style
                case "stroke":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    valueStr = ColorHelper.GetColorStringInHex((Color)value);
                    UpdateElementAttribute(this, "black", "stroke", valueStr, list);
                    break;
                case "stroke-width":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    fValue = (float)value;
                    if (fValue < 0)
                    {
                        throw new Exception("属性值必须是大于0 的数字！");
                    }
                    UpdateElementAttribute(this, "1", "stroke-width", value.ToString(), list);
                    break;
                case "stroke-opacity":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    fValue = (float)value;
                    if (fValue < 0 || fValue > 1)
                    {
                        throw new Exception("请输入0和1之间的数字！");
                    }
                    UpdateElementAttribute(this, "1", "stroke-opacity", fValue.ToString(), list);
                    break;

                case "stroke-dasharray":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    UpdateElementAttribute(this, "none", "stroke-dasharray", value.ToString(), list);
                    break;
                #endregion

                #region ..shadow style
                case "shadow":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    UpdateElementAttribute(this, "false", "shadow", value.ToString(), list);
                    break;
                case "shadowColor":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    valueStr = ColorHelper.GetColorStringInHex((Color)value);
                    UpdateElementAttribute(this, "black", "shadowColor", valueStr, list);
                    break;
                case "shadowOpacity":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    fValue = (float)value;
                    if (fValue < 0 || fValue > 1)
                        throw new Exception("请输入0和1之间的数字！");
                    UpdateElementAttribute(this, "1", "shadowOpacity", value.ToString(), list);
                    break;
                case "xOffset":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    UpdateElementAttribute(this, "0", "xOffset", value.ToString(), list);
                    break;
                case "yOffset":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    UpdateElementAttribute(this, "0", "yOffset", value.ToString(), list);
                    break;
                #endregion

                #region ..FontStyle
                case "font-family":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    this.UpdateCustomProperty("font-family", value.ToString(), "宋体");
                    break;
                case "font-size":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    fValue = (float)value;
                    if (fValue < 0 || fValue > System.Single.MaxValue)
                        throw new Exception("无效的尺寸值，尺寸应该介于0和" + System.Single.MaxValue.ToString() + "之间");
                    this.UpdateCustomProperty("font-size", value.ToString(), "12");
                    break;
                case "font-weight":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    valueStr = ((bool)value) ? "bold" : "normal";
                    this.UpdateCustomProperty("font-weight", valueStr, "normal");
                    break;
                case "font-style":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    valueStr = ((bool)value) ? "italic" : "normal";
                    this.UpdateCustomProperty("font-style", valueStr, "normal");
                    break;
                case "text-decoration":
                    list.AddRange(new string[] { "line", "connect", "polyline" });
                    valueStr = ((bool)value) ? "underline" : "normal";
                    this.UpdateCustomProperty("text-decoration", valueStr, "normal");
                    break;
                #endregion

                #region ..文本块
                case "text-color":
                    valueStr = ColorHelper.GetColorStringInHex((Color)value);
                    this.UpdateCustomProperty(attributeName, valueStr, "black");
                    break;
                case "text-anchor":
                    Alignment align = (Alignment)value;
                    valueStr = "center";
                    if (align == Alignment.Left)
                        valueStr = "start";
                    else if (align == Alignment.Right)
                        valueStr = "end";
                    this.UpdateCustomProperty("text-anchor", valueStr, "center");
                    break;
                case "vertical-align":
                    VerticalAlignment valign = (VerticalAlignment)value;
                    valueStr = "middle";
                    if (valign == VerticalAlignment.Top)
                        valueStr = "top";
                    else if (valign == VerticalAlignment.Bottom)
                        valueStr = "bottom";
                    this.UpdateCustomProperty("vertical-align", valueStr, "middle");
                    break;
                #endregion
                default:
                    base.SetPropertyValue(attributeName, value);
                    break;
            }

        }
        #endregion

        #region ..UpdateCustomProperty
        internal void UpdateCustomProperty(string attributename, string attributevalue, string defaultvalue)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList(new string[] { "text", "use", "line", "circle", "rect", "ellipse", "path", "image", "polyline", "polygon", "connect", "textBlock" });
            UpdateElementAttribute(this, defaultvalue, attributename, attributevalue, list);
               
        }
        #endregion
    }
}
