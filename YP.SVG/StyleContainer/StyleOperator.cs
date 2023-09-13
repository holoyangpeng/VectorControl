using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// �ṩCSS��������Ϣ�Ĳ���ƽ̨
	/// </summary>
    public class StyleOperator : System.IDisposable
	{
		#region ..���켰����
		public StyleOperator(bool blackwhite)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
            this.Init(blackwhite);
		}

		public StyleOperator():this(false)
		{
		}

		public StyleOperator(StyleContainer sc)
		{
			this.currentStyleContainer = sc;
            if (sc == null)
                this.Init(true);
		}

		/// <summary>
		/// ���������ͷ���Դ
		/// </summary>
		public void Dispose()
		{
			if(this.connectPen != null)
				this.connectPen.Dispose();
			this.connectPen = null;
			this.currentStyleContainer = null;
			this.styleContainers = null;
			GC.SuppressFinalize(this);
			GC.Collect(0);
		}
		#endregion

		#region ..˽�б���
		StyleContainer currentStyleContainer ;
		Hashtable styleContainers = new Hashtable();
		#endregion

		#region ..ȫ�ֱ���
        public event PaintConnectablePointEventHandler PaintConnectablePoint;
		public System.Drawing.Pen outlinePen = null;//new System.Drawing.Pen(System.Drawing.Color.Black);
		public System.Drawing.Pen connectPen = new System.Drawing.Pen(System.Drawing.Color.MediumBlue);
        public System.Drawing.Drawing2D.Matrix coordTransform = new System.Drawing.Drawing2D.Matrix();
        public bool UseCoordTransform = false;
		public YP.SVG.SVGElementCollection connectElements = null;
        public bool? AutoBridgeForConnect = null;
		public bool drawConnects = false;
		bool _drawShadow = true;
		public int connectSnap = 6;
		public YP.SVG.SVGElementCollection renderElements = null;
		public bool AddRender = true;
		public System.Drawing.Drawing2D.GraphicsPath ShadowPath = new System.Drawing.Drawing2D.GraphicsPath();
		public bool AddConnectableElements = true;
		public System.Collections.ArrayList States = new ArrayList();
		public System.Drawing.Drawing2D.GraphicsState InitialState = null;
		public System.Drawing.Drawing2D.GraphicsState BlankState = null;
        public Region ClipRegion = new Region();
        public GraphicsPath ConnectPath = new GraphicsPath();
        public bool RememberOperationData = false;
        public RectangleF contentBounds = RectangleF.Empty;
		#endregion

        #region ..Init
        void Init(bool blackwhite)
        {
            this.currentStyleContainer = new StyleContainer(true);
            if (blackwhite)
            {
                FillStyle fillstyle = this.currentStyleContainer.FillStyle;
                fillstyle.svgPaint = new YP.SVG.Paint.SVGPaint("white", null, "white");
                this.currentStyleContainer.FillStyle = fillstyle;
                StrokeStyle stroke = this.currentStyleContainer.StrokeStyle;
                stroke.svgStroke = new YP.SVG.Paint.SVGPaint("black", null, "black");
                this.currentStyleContainer.StrokeStyle = stroke;
            }

            //��ʼ
            ClipRegion.MakeEmpty();
        }
        #endregion

        #region ..��������
        /// <summary>
		/// ��ȡ�����Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.FillStyle FillStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.FillStyle;
				return FillStyle.Empty;
			}
		}

		public YP.SVG.StyleContainer.ShadowStyle ShadowStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.ShadowStyle;
				return ShadowStyle.Default;
			}
		}
		
		public bool DrawShadow
		{
			set
			{
				this._drawShadow = value;
			}
			get
			{
				return this._drawShadow;
			}
		}

        bool boundView = false;
		/// <summary>
		/// �жϵ�ǰ�Ƿ��������
		/// </summary>
		public bool BoundView
		{
			get
			{
                return boundView;
                //if(this.currentStyleContainer != null)
                //    return this.currentStyleContainer.BoundView;
                //return false;
			}
			set
			{
                boundView = value;
                //if(this.currentStyleContainer != null)
                //    this.currentStyleContainer.BoundView = value;
			}
		}

		/// <summary>
		/// �жϵ�ǰ�Ƿ��������
		/// </summary>
		public bool ViewVisible
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.ViewVisible;
				return true;
			}
		}

		/// <summary>
		/// ��ȡ���������Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.StrokeStyle StrokeStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.StrokeStyle;
				return StrokeStyle.Empty;
			}
			set
			{
				if(this.currentStyleContainer != null)
					this.currentStyleContainer.StrokeStyle = value;
			}
		}

		/// <summary>
		/// ��ȡ��ͨ��Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.VisualMediaStyle VisualMediaStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.VisualMediaStyle;
				return VisualMediaStyle.Empty;
			}
			
		}

		/// <summary>
		/// ��ȡ��ɫ�ͻ�����Ϣ��Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.ColorAndPaintStyle ColorAndPaintStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.ColorAndPaintStyle;
				return ColorAndPaintStyle.Empty;
			}
		}

		/// <summary>
		/// ��ȡ�ı���Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.TextStyle  TextStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.TextStyle;
				return TextStyle.Empty;
			}
		}

		/// <summary>
		/// ��ȡ������Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.FontStyle FontStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.FontStyle;
				return FontStyle.Empty;
			}
		}

		/// <summary>
		/// ��ȡ������Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.ClipStyle ClipStyle
		{
			get
			{
				if(this.currentStyleContainer != null)
					return this.currentStyleContainer.ClipStyle;
				return ClipStyle.Empty;;
			}
		}

		/// <summary>
		/// ���õ�ǰ���������еĻ���ͽڵ�
		/// </summary>
//		public YP.SVGDom.SVGStyleable CurrentSVGStyleable
//		{
//			set
//			{
//				if(this.currentStyleContainer != null)
//					this.MergeSVGStyleable(value);
//			}
//		}
		#endregion

		#region ..��������
		/// <summary>
		/// �ڵ�ǰ״̬�¿�ʼһ��StyleContainer
		/// </summary>
		/// <returns></returns>
        public void BeginStyleContainer(SVGStyleable style)
        {
            if (style == null)
                return;
            if (!this.styleContainers.ContainsKey(style))
                this.styleContainers.Add(style, new StyleContainer(this.currentStyleContainer));
            if (style.CurrentTime != style.OwnerDocument.CurrentTime || style.StyleContainer == null)
            {
                this.MergeSVGStyleable(style);
                style.StyleContainer = new StyleContainer(this.currentStyleContainer);
            }
            else
                this.currentStyleContainer = new StyleContainer(style.StyleContainer);
            
        }

		/// <summary>
		/// ���ָ�����������Խ��
		/// </summary>
        /// <param name="style"></param>
        public void EndContainer(SVGStyleable style)
        {
            if (style == null)
                return;
            if (this.styleContainers.ContainsKey(style))
            {
                this.currentStyleContainer = this.styleContainers[style] as StyleContainer;
                this.styleContainers.Remove(style);
            }
        }
		#endregion

		#region ..�ϲ�SVGStyleable
		/// <summary>
		/// �ϲ�SVGStyleable
		/// </summary>
		/// <param name="svgStyle"></param>
		void MergeSVGStyleable(SVGStyleable svgStyle)
		{
			if(this.currentStyleContainer != null)
			{
				this.currentStyleContainer.FillStyle = this.currentStyleContainer.FillStyle.MutiplyStyle(svgStyle.FillStyle);
				this.currentStyleContainer.StrokeStyle= this.currentStyleContainer.StrokeStyle.MutiplyStyle(svgStyle.StrokeStyle);
				this.currentStyleContainer.VisualMediaStyle = this.currentStyleContainer.VisualMediaStyle.MutiplyStyle(svgStyle.VisualMediaStyle);
				this.currentStyleContainer.ClipStyle = this.currentStyleContainer.ClipStyle.MutiplyStyle(svgStyle.ClipStyle);
				this.currentStyleContainer.ColorAndPaintStyle = this.currentStyleContainer.ColorAndPaintStyle.MutiplyStyle(svgStyle.ColorAndPaintStyle);
				this.currentStyleContainer.FontStyle=this.currentStyleContainer.FontStyle.MutiplyStyle(svgStyle.FontStyle);
				this.currentStyleContainer.TextStyle = this.currentStyleContainer.TextStyle.MutiplyStyle(svgStyle.TextStyle);
				this.currentStyleContainer.ShadowStyle = svgStyle.ShadowStyle;
			}
		}
		#endregion

        #region ..OnPaintConnectablePoint
        public bool OnPaintConnectablePoint(Graphics g, SVGTransformableElement element, PointF point, int index)
        {
            if (this.PaintConnectablePoint != null)
            {
                PaintConnectablePointEventArgs e = new PaintConnectablePointEventArgs(g, element , point, index);
                this.PaintConnectablePoint(this, e);
                return e.OwnerDraw;
            }
            return false;
        }
        #endregion
    }
}
