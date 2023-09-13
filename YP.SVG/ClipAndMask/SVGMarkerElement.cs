using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.ClipAndMask
{
	/// <summary>
	/// marker
	/// </summary>
	public class SVGMarkerElement:YP.SVG.SVGStyleable,Interface.ISVGMarkerElement,Interface.ISVGContainer
	{
		#region ..构造及消除
		public SVGMarkerElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
//			clip_path = new DataType.SVGString("");//,string.Empty,this);
			this.refY = this.refX = new DataType.SVGNumber(0);
			this.markerWidth = new SVG.DataType.SVGLength(3,this,LengthDirection.Hori);
			this.markerHeight = new SVG.DataType.SVGLength(3,this,LengthDirection.Vect);
		}
		#endregion

		#region ..私有变量
		DataType.SVGNumber refX,refY;
		DataType.SVGLength markerWidth,markerHeight;
		DataType.SVGAngle angle;
		DataType.SVGRect viewBox = new SVG.DataType.SVGRect(System.Drawing.Rectangle.Empty);
        DataType.SVGPreserveAspectRatio per = new DataType.SVGPreserveAspectRatio("xMidYMid meet");
		StyleContainer.StyleOperator styleoperator = new SVG.StyleContainer.StyleOperator();
		System.Drawing.Drawing2D.GraphicsPath gp = new GraphicsPath();
		Matrix markerTransform = new Matrix();
		#endregion

		#region ..属性操作
		public override void SetSVGAttribute(string attributeName, string attributeValue)
		{
			switch(attributeName)
			{
				case "refX":
					this.refX = new SVG.DataType.SVGNumber(attributeValue,"0");
					break;
				case "refY":
					this.refY = new SVG.DataType.SVGNumber(attributeValue,"0");
					break;
				case "markerWidth":
					this.markerWidth = new SVG.DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
					break;
				case "markerHeight":
					this.markerHeight = new SVG.DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
					break;
				case "orient":
						this.angle = new SVG.DataType.SVGAngle(attributeValue,"0");
					break;
				case "viewBox":
					this.viewBox = new SVG.DataType.SVGRect(attributeValue);
					break;
				case "preserveAspectRatio":
					this.per = new DataType.SVGPreserveAspectRatio(attributeValue);
					break;
			}
			base.SetSVGAttribute (attributeName, attributeValue);
		}

		#endregion

		#region ISVGFitToViewBox 成员
		public SVG.Interface.DataType.ISVGRect ViewBox
		{
			get
			{
				// TODO:  添加 SVGMarker.ViewBox getter 实现
				return this.viewBox;
			}
		}

		public SVG.Interface.CTS.ISVGPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				// TODO:  添加 SVGMarker.PreserveAspectRatio getter 实现
				return this.per;
			}
		}
		#endregion

		#region ..私有变量
		SVG.SVGElementCollection childRenders = new SVG.SVGElementCollection();
		#endregion

		#region ..公共属性
		/// <summary>
		/// Get the marker path
		/// </summary>
		public GraphicsPath MarkerPath
		{
			get
			{
				return this.gp;
			}
		}

		/// <summary>
		/// get a value indicates whether fill the shadow
		/// </summary>
		public bool FillShadow
		{
			get
			{
				return this.GetAttribute("fillShadow").Trim().ToLower() == "true";
			}
		}

		public Matrix MarkerTransform
		{
			get
			{
				return this.markerTransform;
			}
		}
		/// <summary>
		/// 获取子级绘制节点
		/// </summary>
		public SVG.SVGElementCollection ChildElements
		{
			get
			{
				return this.childRenders;
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

		#region ..RenderMarker
		/// <summary>
		/// 绘制线帽
		/// </summary>
		public void MarkerStart(System.Drawing.Graphics g,System.Drawing.Drawing2D.GraphicsPath path,SVG.StyleContainer.StyleOperator sp)
		{
			this.DrawMarker(g,path,sp,0);
		}

		/// <summary>
		/// 绘制线帽
		/// </summary>
        public void MarkerEnd(System.Drawing.Graphics g, System.Drawing.Drawing2D.GraphicsPath path, SVG.StyleContainer.StyleOperator sp)
		{
			if(path != null)
				this.DrawMarker(g,path,sp,path.PointCount - 1);
		}
		#endregion

		#region ..DrawMarker
		void DrawMarker(System.Drawing.Graphics g,System.Drawing.Drawing2D.GraphicsPath path,SVG.StyleContainer.StyleOperator sp,int pointIndex)
		{
            if (path != null && path.PointCount > 1)//&& path.PathTypes[path.PathTypes.Length - 1] <= (byte)PathPointType.CloseSubpath)
            {
                bool old = sp.drawConnects;
                sp.drawConnects = false;
                System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
                System.Drawing.Drawing2D.GraphicsContainer c = g.BeginContainer();
                g.SmoothingMode = mode;
                this.TransformGraphics(g, path, sp, pointIndex);
                bool drawShadow = sp.DrawShadow;
                this.gp.Reset();
                bool old1 = sp.DrawShadow;
                sp.DrawShadow = false;
                sp.BeginStyleContainer(this);
                //remember the clip rectangles
                Region rg = sp.ClipRegion.Clone();
                sp.ClipRegion.MakeEmpty();
                using (Matrix oriMatrix = sp.coordTransform.Clone())
                {
                    //Marker 不再变换
                    sp.coordTransform.Reset();
                    try
                    {
                        for (int i = 0; i < this.childRenders.Count; i++)
                        {
                            SVGTransformableElement render = this.childRenders[i] as SVGTransformableElement;
                            if (render == null || render.SVGRenderer == null)
                                continue;
                            render.SVGRenderer.Draw(g, sp);
                            if (render is SVG.Interface.ISVGPathable)
                            {
                                using (System.Drawing.Drawing2D.GraphicsPath path1 = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                {
                                    path1.Transform((render as SVG.SVGTransformableElement).TotalTransform);
                                    this.gp.StartFigure();
                                    this.gp.AddPath(path1, false);
                                }
                            }
                        }

                        sp.EndContainer(this);
                        markerTransform = g.Transform.Clone();
                        g.EndContainer(c);
                        sp.drawConnects = old;
                        sp.DrawShadow = old1;
                        using (GraphicsPath path1 = this.gp.Clone() as GraphicsPath)
                        {
                            path1.Transform(this.markerTransform);
                            this.DrawShadow(sp, g, path1);
                        }
                    }
                    catch (System.Exception e)
                    {
                        this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new string[] { e.Message }, ExceptionLevel.Normal));
                    }
                    finally
                    {
                        sp.ClipRegion.Union(rg);
                        rg.Dispose();
                    }

                    sp.coordTransform.Multiply(oriMatrix);
                }
            }
		}
		#endregion

		#region ..DrawShadow
		/// <summary>
		/// Draw shadow for the shape
		/// </summary>
		/// <param name="g"></param>
		/// <param name="shadowPath">the path of the shadow</param>
		public virtual void DrawShadow(StyleContainer.StyleOperator sp,Graphics g,System.Drawing.Drawing2D.GraphicsPath shadowPath)
		{
			if(sp.DrawShadow&& sp.ShadowStyle.DrawShadow  && shadowPath != null)
			{
				float opacity = sp.ShadowStyle.Opacity;
				using(Brush brush = new SolidBrush(Color.FromArgb((int)(255f * opacity),sp.ShadowStyle.ShadowColor)))
				{
					opacity = (float)Math.Min(sp.ClipStyle.opacity.Value,sp.StrokeStyle.strokeOpacity.Value);
					using(Pen pen = new Pen(brush))
					{
						System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
						System.Drawing.Drawing2D.GraphicsContainer c= g.BeginContainer();
						g.SmoothingMode = mode;
						g.SetClip(shadowPath,CombineMode.Exclude);
						g.TranslateTransform(sp.ShadowStyle.XOffset,sp.ShadowStyle.YOffset);
						if(this.FillShadow)
							g.FillPath(brush,shadowPath);
						g.DrawPath(pen,shadowPath);
						g.EndContainer(c);
					}
				}
			}
		}
		#endregion

		#region ..Tansform
		void TransformGraphics(System.Drawing.Graphics g,System.Drawing.Drawing2D.GraphicsPath path,SVG.StyleContainer.StyleOperator sp,int index)
		{
			if(path.PointCount > 1)
			{
				PointF startpoint = path.PathPoints[index];
				PointF endpoint = startpoint;
				PointF p = startpoint;
				if(((PathPointType)path.PathTypes[index] & PathPointType.CloseSubpath) == PathPointType.CloseSubpath)
				{
					for(int i = index - 1;i>=0;i--)
					{
						if(path.PathTypes[i] == (byte)PathPointType.Start)
						{
							p = endpoint = path.PathPoints[i];
							break;
						}
					}
				}
				else
				{				
					if(index > 0)
						startpoint = path.PathPoints[index-1];
					else
						endpoint = path.PathPoints[index + 1];
				}
				g.TranslateTransform(p.X,p.Y);
				
				float angle = 0;
				if(this.angle.IsEmpty)
				{
					angle = PathHelper.GetAngle(startpoint,endpoint);
				}
				else
					angle =(float)(this.angle.Value);
				g.RotateTransform(angle);
				if(this.GetAttribute("markerUnits") != "userSpaceOnUse")
				{
					float scale = sp.StrokeStyle.strokewidth.Value;
					g.ScaleTransform(scale,scale);
				}
				if(!this.viewBox.IsEmpty)
				{
					RectangleF rect = this.viewBox.GDIRect;
					float scale = this.markerHeight.Value / rect.Height;
					g.TranslateTransform(-this.refX.Value * scale ,-this.refY.Value * scale);
					g.ScaleTransform(scale,scale);
				}
			}
		}
		#endregion
	}
}
