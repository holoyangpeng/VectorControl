using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using YP.SVG.Interface;
using YP.SVG.Interface.DataType;
using YP.SVG.DataType;

namespace YP.SVG.GradientsAndPatterns
{
	/// <summary>
	/// 实现图案
	/// </summary>
	public class SVGPatternElement:YP.SVG.GradientsAndPatterns.SVGPaintTransformElement,Interface.GradientsAndPatterns.ISVGPatternElement,Interface.ISVGContainer
	{
		#region ..构造及消除
		public SVGPatternElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.patternContentUnits = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
			this.patternUnits = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
			this.paintTransform = new DataType.SVGTransformList(string.Empty);//("",this);
			this.href = new DataType.SVGString(string.Empty,string.Empty);//,this);
			this.x = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.width = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.height = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.y = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.preserveAspectRatio = new DataType.SVGPreserveAspectRatio(string.Empty);//("",this);
			this.viewBox = new DataType.SVGRect(string.Empty);;//("",this);

			tempFile = System.IO.Path.GetTempFileName()+".wmf";
//			this.fillStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFillStyle(this);
//			this.strokeStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedStrokeStyle(this);
//			this.visualMediaStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedVisualMediaStyle(this);
//			this.clipStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedClipStyle(this);
//			this.fontStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedFontStyle(this);
//			this.colorAndPaintStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedColorAndPaintStyle(this);
//			this.textStyle = new YP.SVGDom.StyleContainer.AnimatedStyle.AnimatedTextStyle(this);
		}
		#endregion

		#region ..私有变量
		System.Enum patternUnits;
		System.Enum patternContentUnits;
		DataType.SVGLength x,y,width,height;
		DataType.SVGRect viewBox;
		DataType.SVGPreserveAspectRatio preserveAspectRatio;
		DataType.SVGString href;
		bool setview = false;
		RectangleF rect = RectangleF.Empty;
		YP.SVG.SVGElementCollection renders = new YP.SVG.SVGElementCollection();
		string tempFile = string.Empty;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取有效子级列表
		/// </summary>
		public YP.SVG.SVGElementCollection ChildElements
		{
			get
			{
				return this.renders;
			}
		}

		/// <summary>
		/// 获取"patternUnits"属性
		/// </summary>
		public System.Enum PatternUnits
		{
			get
			{
				return this.patternUnits;
			}
		}

		public System.Enum PatternContentUnits
		{
			get
			{
				return this.patternContentUnits;
			}
		}

		public Interface.CTS.ISVGTransformList PatternTransform
		{
			get
			{
				return this.paintTransform;
			}
		}

		public Interface.DataType.ISVGLength X
		{
			get
			{
				return this.x;
			}
		}

		public Interface.DataType.ISVGLength Y
		{
			get
			{
				return this.y;
			}
		}

		public Interface.DataType.ISVGLength Width
		{
			get
			{
				return this.width;
			}
		}

		public Interface.DataType.ISVGLength Height
		{
			get
			{
				return this.height;
			}
		}

		public Interface.DataType.ISVGString Href
		{
			get
			{
				return this.href;
			}
		}

		public Interface.DataType.ISVGRect ViewBox
		{
			get
			{
				string attr = GetAttribute("viewBox").Trim();
				if(string.Compare(attr,"")==0)
				{
					RectangleF rect = new RectangleF(
						X.Value, 
						Y.Value, 
						Width.Value, 
						Height.Value);
					return new SVGRect(rect);//,this);
				}
				else
				{
					return new SVGRect(attr);//,this);
				}
			}
		}

		public Interface.CTS.ISVGPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				return this.preserveAspectRatio;
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
                switch (attributeName)
                {
                    case "href":
                        this.href = new DataType.SVGString(attributeValue, string.Empty);//,this);
                        break;
                    case "patternTransform":
                        this.paintTransform = new DataType.SVGTransformList(attributeValue);//,this);
                        break;
                    case "patternUnits":
                        SVGUnitType unit = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;
                        if (string.Compare(attributeValue,"userSpaceOnUse")==0)
                            unit = SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
                        this.patternUnits = unit;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
                        break;
                    case "patternContentUnits":
                        SVGUnitType content = SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX;
                        if (string.Compare(attributeValue,"userSpaceOnUse")==0)
                            content = SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
                        this.patternContentUnits = content;//,SVGUnitType.SVG_UNIT_TYPE_OBJECTBOUNDINGBOX,this);
                        break;
                    case "x":
                        this.x = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori);
                        this.rect.X = this.x.Value;
                        if (!this.setview)
                            this.viewBox = new DataType.SVGRect(this.rect);//,this);
                        break;
                    case "width":
                        this.width = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori);
                        this.rect.Width = this.width.Value;
                        if (!this.setview)
                            this.viewBox = new DataType.SVGRect(this.rect);//,this);
                        break;
                    case "y":
                        this.y = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);
                        this.rect.Y = this.y.Value;
                        if (!this.setview)
                            this.viewBox = new DataType.SVGRect(this.rect);//,this);
                        break;
                    case "height":
                        this.height = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);
                        this.rect.Height = this.height.Value;
                        if (!this.setview)
                            this.viewBox = new DataType.SVGRect(this.rect);//,this);
                        break;
                    case "viewBox":
                        this.setview = true;
                        this.viewBox = new DataType.SVGRect(attributeValue);//,this);
                        break;
                    case "preserveAspectRatio":
                        this.preserveAspectRatio = new DataType.SVGPreserveAspectRatio(attributeValue);//,this);
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

		#region ..判断节点是否是有效的子级节点
		/// <summary>
		/// 判断节点是否是有效的子级节点
		/// </summary>
		/// <param name="child">子级节点</param>
		/// <returns></returns>
		public bool ValidChild(Interface.ISVGElement child)
		{
            return child is SVGTransformableElement;
		}
		#endregion

		#region ..当对象填充指定路径时，获取其控制路径
		/// <summary>
		/// 当对象填充指定路径时，获取其控制路径
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public override GraphicsPath GetControlPath(GraphicsPath fillPath)
		{
			this.paintPath.Reset();
			bool userSpaceOnUse = (YP.SVG.SVGUnitType)this.PatternUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
			float x = this.X.Value;
			float y = this.Y.Value;
			float width = this.Width.Value;
			float height = this.Height.Value;	

			RectangleF rect1 = fillPath.GetBounds();
			if(!userSpaceOnUse)
			{
				x = x * rect1.Width;
				y = y * rect1.Height;
				width = (int)Math.Min(width * rect1.Width,rect1.Width);
				height = (int)Math.Min(height * rect1.Height,rect1.Height);
			}
			this.paintPath.AddRectangle(new RectangleF(x,y,width,height));
			using(Matrix matrix1 = new Matrix(1,0,0,1,rect1.X + rect1.Width / 2f,rect1.Y + rect1.Height / 2f))
			{
				this.paintPath.Transform(matrix1);
			}
			return this.paintPath;
		}
		#endregion

		#region ..当对象填充指定路径时，获取其控制点集
		/// <summary>
		/// 当对象填充指定路径时，获取其控制点集
		/// </summary>
		/// <param name="fillPath">将要填充的路径</param>
		public override PointF[] GetControlPoints(GraphicsPath fillPath)
		{
			bool userSpaceOnUse = (YP.SVG.SVGUnitType)this.PatternUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
			float x = this.X.Value;
			float y = this.Y.Value;
			float width = this.Width.Value;
			float height = this.Height.Value;	

			RectangleF rect1 = fillPath.GetBounds();
			if(!userSpaceOnUse)
			{
				x = x * rect1.Width;
				y = y * rect1.Height;
				width = (int)Math.Min(width * rect1.Width,rect1.Width);
				height = (int)Math.Min(height * rect1.Height,rect1.Height);
			}

			this.controlPoints =  new PointF[]{new PointF(x + width / 2f,y + height / 2f),new PointF(x ,y+ height / 2),new PointF(x+ width,y),new PointF(x + width / 2f,y),new PointF(x + width,y + height /2 ),new PointF(x + width/ 2f,y + height),new PointF(x ,y + height),PointF.Empty };
			using(Matrix matrix1 = new Matrix(1,0,0,1,rect1.X + rect1.Width / 2f,rect1.Y + rect1.Height / 2f))
			{
				matrix1.TransformPoints(this.controlPoints);
			}
			this.controlPoints[this.controlPoints.Length - 1] = PointF.Empty;
			return this.controlPoints;
		}
		#endregion

        #region ..GetBrush
        /// <summary>
		/// 当对象填充指定路径时，获取绘制画笔
		/// </summary>
		/// <param name="bounds">填充路径边界</param>
		public override Brush GetBrush(SVG.SVGTransformableElement ownerElement,Rectangle bounds,float opacity)
		{
            RectangleF temprect = bounds;
            if (this.CurrentTime != this.OwnerDocument.CurrentTime || bounds != this.preBounds || opacity != this.preFloat)
            {
                bool userSpaceOnUse = (SVG.SVGUnitType)this.PatternUnits == SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
                float x = this.X.Value;
                float y = this.Y.Value;
                float width = this.Width.Value;
                float height = this.Height.Value;

                SVG.Interface.ISVGFitToViewBox fitToVBElm = (SVG.Interface.ISVGFitToViewBox)this;
                DataType.SVGPreserveAspectRatio spar = (DataType.SVGPreserveAspectRatio)fitToVBElm.PreserveAspectRatio;

                RectangleF elmRect = new RectangleF(x, y, width, height);
                float[] translateAndScale = spar.FitToViewBox(
                    ((SVG.DataType.SVGRect)fitToVBElm.ViewBox).GDIRect,
                    elmRect
                    );
                using (System.Drawing.Drawing2D.Matrix viewMatrix = new System.Drawing.Drawing2D.Matrix())
                {
                    viewMatrix.Translate(translateAndScale[0], translateAndScale[1]);
                    viewMatrix.Scale(translateAndScale[2], translateAndScale[3]);

                    RectangleF rect = bounds;
                    if (!userSpaceOnUse)
                    {
                        x = x * rect.Width;
                        y = y * rect.Height;
                        width = (int)Math.Min(width * rect.Width, rect.Width);
                        height = (int)Math.Min(height * rect.Height, rect.Height);
                    }

                    using (Bitmap bmp = new Bitmap((int)width, (int)height))
                    {
                        using (Graphics mdc = Graphics.FromImage(bmp))
                        {
                            using (Matrix matrix = viewMatrix.Clone())
                            {
                                mdc.Transform = matrix;
                                mdc.SmoothingMode = SmoothingMode.HighQuality;
                                SVG.StyleContainer.StyleOperator sp = this.OwnerDocument.CreateStyleOperator();
                                sp.BeginStyleContainer(this);
                                for (int i = 0; i < this.ChildElements.Count; i++)
                                {
                                    SVGTransformableElement render = this.ChildElements[i] as SVGTransformableElement;
                                    render.SVGRenderer.Draw(mdc, sp);
                                }
                                if (this.brush != null)
                                    this.brush.Dispose();
                                this.brush = null;
                                brush = new TextureBrush(bmp, new RectangleF(x, y, width, height));
                                ((TextureBrush)brush).WrapMode = WrapMode.Tile;
                                ((TextureBrush)brush).Transform = this.PatternTransform.FinalMatrix.GetGDIMatrix();
                            }
                        }
                    }
                }
                translateAndScale = null;
            }
            this.preBounds = temprect;
            this.CurrentTime = this.OwnerDocument.CurrentTime;
            this.preFloat = opacity;

            return (Brush)this.brush.Clone();
		}
		#endregion

		#region ..当对象填充指定路径时，获取其变换矩阵
		/// <summary>
		/// 当对象填充指定路径时，获取其变换矩阵
		/// </summary>
		public override Matrix GetTransform(GraphicsPath fillPath)
		{
			YP.SVG.Interface.ISVGFitToViewBox fitToVBElm = (YP.SVG.Interface.ISVGFitToViewBox )this;
			DataType.SVGPreserveAspectRatio spar = (DataType.SVGPreserveAspectRatio)fitToVBElm.PreserveAspectRatio;

			bool userSpaceOnUse = (YP.SVG.SVGUnitType)this.PatternUnits == YP.SVG.SVGUnitType.SVG_UNIT_TYPE_USERSPACEONUSE;
			float x = this.X.Value;
			float y = this.Y.Value;
			float width = this.Width.Value;
			float height = this.Height.Value;	

			RectangleF rect1 = fillPath.GetBounds();
			if(!userSpaceOnUse)
			{
				x = x * rect1.Width;
				y = y * rect1.Height;
				width = (int)Math.Min(width * rect1.Width,rect1.Width);
				height = (int)Math.Min(height * rect1.Height,rect1.Height);
			}

			RectangleF elmRect = new RectangleF(x, y, width, height);
			float[] translateAndScale = spar.FitToViewBox(
				((YP.SVG.DataType.SVGRect)fitToVBElm.ViewBox).GDIRect,
				elmRect
				);
			System.Drawing.Drawing2D.Matrix viewMatrix = new System.Drawing.Drawing2D.Matrix();
			viewMatrix.Translate(translateAndScale[0], translateAndScale[1]);
			viewMatrix.Scale(translateAndScale[2], translateAndScale[3]);
			translateAndScale = null;
			spar = null;
			return viewMatrix;
		}
		#endregion
	}
}
