using System;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 类型属性存储容器，包括绘制、透明度、可见性等。
	/// </summary>
	public class StyleContainer
    {
		#region ..构造及消除
		public StyleContainer():this(false)
		{
			
		}

		/// <summary>
		/// 构造容器
		/// </summary>
		/// <param name="createDefault">是否创建默认值</param>
		public StyleContainer(bool createDefault)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			if(createDefault)
			{
				fillStyle = FillStyle.Default;
				strokeStyle = StrokeStyle.Default;
				visualMediaStyle = VisualMediaStyle.Default;
				fontStyle = FontStyle.Default;
				textStyle = TextStyle.Default;
				clipStyle = ClipStyle.Default;
				colorAndPaintStyle = ColorAndPaintStyle.Default;
				this.shadowStyle = ShadowStyle.Default;
			}
		}

		public StyleContainer(StyleContainer container)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            if (container != null)
            {
                fillStyle = new FillStyle(container.fillStyle);
                strokeStyle = new StrokeStyle(container.strokeStyle);
                visualMediaStyle = new VisualMediaStyle(container.visualMediaStyle);
                fontStyle = new FontStyle(container.fontStyle);
                textStyle = new TextStyle(container.textStyle);
                clipStyle = new ClipStyle(container.clipStyle);
                this.shadowStyle = new ShadowStyle(container.shadowStyle);
                colorAndPaintStyle = new ColorAndPaintStyle(container.colorAndPaintStyle);
                this.boundView = container.boundView;
                this.viewVisible = container.viewVisible;
            }
		}
		#endregion

		#region ..私有变量
		FillStyle fillStyle;
		StrokeStyle strokeStyle;
		VisualMediaStyle visualMediaStyle ;
		FontStyle fontStyle;
		TextStyle textStyle ;
		ClipStyle clipStyle;
		ShadowStyle shadowStyle;
		ColorAndPaintStyle colorAndPaintStyle ;
		bool boundView = false,viewVisible = true;
		#endregion
	
		#region ..保护属性
		/// <summary>
		/// 获取填充信息
		/// </summary>
		public YP.SVG.StyleContainer.FillStyle FillStyle
		{
			get
			{
				return this.fillStyle;
			}
			set
			{
				this.fillStyle = value;
			}
		}

		/// <summary>
		/// 判断当前绘制状态是否为轮廓绘制
		/// </summary>
		public bool BoundView
		{
			get
			{
				return this.boundView;
			}
			set
			{
				this.boundView = value;
			}
		}

		/// <summary>
		/// 当前是否绘制内容
		/// </summary>
		public bool ViewVisible
		{
			get
			{
				return this.viewVisible;
			}
			set
			{
				this.viewVisible = value;
			}
		}

		/// <summary>
		/// 获取画笔填充信息
		/// </summary>
		public YP.SVG.StyleContainer.StrokeStyle StrokeStyle
		{
			get
			{
				return this.strokeStyle;
			}
			set
			{
				this.strokeStyle = value;
			}
		}

		/// <summary>
		/// 获取普通信息
		/// </summary>
		public YP.SVG.StyleContainer.VisualMediaStyle VisualMediaStyle
		{
			get
			{
				return this.visualMediaStyle;
			}
			set
			{
				this.visualMediaStyle = value;
			}
		}

		/// <summary>
		/// 获取颜色和绘制信息信息
		/// </summary>
		public YP.SVG.StyleContainer.ColorAndPaintStyle ColorAndPaintStyle
		{
			get
			{
				return this.colorAndPaintStyle;
			}
			set
			{
				this.colorAndPaintStyle = value;
			}
		}

		/// <summary>
		/// 获取文本信息
		/// </summary>
		public YP.SVG.StyleContainer.TextStyle  TextStyle
		{
			get
			{
				return this.textStyle;
			}
			set
			{
				this.textStyle = value;
			}
		}

		/// <summary>
		/// 获取字体信息
		/// </summary>
		public YP.SVG.StyleContainer.FontStyle FontStyle
		{
			get
			{
				return this.fontStyle;
			}
			set
			{
				this.fontStyle = value;
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
			set
			{
				this.shadowStyle = value;
			}
		}

		/// <summary>
		/// 获取剪切信息
		/// </summary>
		public YP.SVG.StyleContainer.ClipStyle ClipStyle
		{
			get
			{
				return this.clipStyle;
			}
			set
			{
				this.clipStyle = value;
			}
		}
		#endregion

		#region ..克隆
		public StyleContainer Clone()
		{
			StyleContainer s = new StyleContainer();
			s.fillStyle = this.fillStyle;
			s.strokeStyle = this.strokeStyle;
			s.visualMediaStyle = this.visualMediaStyle;
			s.clipStyle = this.clipStyle;
			s.fontStyle = this.fontStyle;
			s.textStyle = this.textStyle;
			s.colorAndPaintStyle = this.colorAndPaintStyle;
			return s;
		}
		#endregion
	}
}
