using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// 视图操作
	/// </summary>
	internal class ViewOperation:Operation
	{
		#region ..构造及消除
		public ViewOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.mouseArea = mousearea;
		}
		#endregion

		#region ..私有变量
		PointF startPoint = PointF.Empty;
		PointF oriAutoScrollPos = PointF.Empty;
		#endregion

		#region ..鼠标事件

		#region ..OnMouseDown
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if(e.Button == MouseButtons.Left)
				{
					this.mousedown = true;
				}
				if(this.mouseArea.CurrentOperator == Operator.Roam)
				{
					if(this.mousedown)
					{
						this.startPoint =new PointF(e.X,e.Y);
						this.oriAutoScrollPos = this.mouseArea.AutoScrollPosition;
					}
				}
				else
					this.startPoint = new PointF(e.X,e.Y);
			}
			catch{}
		}
		#endregion

		#region ..OnMouseMove
		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if(e.Button == MouseButtons.Left && this.mousedown)
				{
					if(this.mouseArea.CurrentOperator == Operator.Roam)
					{
						float x1 = e.X - this.startPoint.X;
						float y = e.Y - this.startPoint.Y;
						if(this.mouseArea.CoordTransform.Elements[1] != 0)
							x1 = x1 / this.mouseArea.CoordTransform.Elements[1];
						if(this.mouseArea.CoordTransform.Elements[2] != 0)
							y = y / this.mouseArea.CoordTransform.Elements[2];

						this.mouseArea.SetScrollPos(Point.Ceiling(new PointF(-this.oriAutoScrollPos.X - x1,-this.oriAutoScrollPos.Y - y)));
						this.mouseArea.scrolled = true;
					}
					else
					{
						this.XORDrawPath(this.reversePath);
						this.reversePath.Reset();
						float x = (float)Math.Min(e.X,this.startPoint.X);
						float right = (float)Math.Max(e.X,this.startPoint.X);
						float top = (float)Math.Min(e.Y ,this.startPoint.Y);
						float bottom = (float)Math.Max(e.Y ,this.startPoint.Y);
						this.reversePath.AddRectangle(new RectangleF(x,top,right - x,bottom - top));
						this.XORDrawPath(this.reversePath);
					}
				}
			}
			catch{}
		}
		#endregion

		#region ..OnMouseUp
		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(object sender,MouseEventArgs e)
		{
			try
			{
				base.OnMouseUp(sender,e);
				if(!this.IsValidDocument)
					return;
				if(e.Button == MouseButtons.Left && this.mousedown && this.mouseArea.CurrentOperator != Operator.Roam)
				{
                    float x = (float)Math.Min(e.X, this.startPoint.X);
                    float right = (float)Math.Max(e.X, this.startPoint.X);
                    float top = (float)Math.Min(e.Y, this.startPoint.Y);
                    float bottom = (float)Math.Max(e.Y, this.startPoint.Y);
                    PointF p = new PointF((right + x) / 2f, (bottom + top) / 2f);

                    float targetScale = this.mouseArea.ScaleRatio;
                    if (right - x > 2 && bottom - top >= 0)
                    {
                        float scale = (float)(this.mouseArea.Width - 20) / (float)(right - x);
                        scale = (float)Math.Min(scale, (float)(this.mouseArea.Height - 20) / (float)(bottom - top));
                        targetScale *= scale;
                    }
                    else if (e.X == this.startPoint.X && e.Y == this.startPoint.Y)
                    {
                        float scale = this.mouseArea.ScaleRatio;
                        if (this.mouseArea.CurrentOperator == Operator.ZoomIn)
                            scale *= 2f;
                        else if (this.mouseArea.CurrentOperator == Operator.ZoomOut)
                            scale *= 0.5f;
                        targetScale = scale;
                    }

                    this.mouseArea.ScaleAtCenter(targetScale, Point.Round(p));
				}
				this.reversePath.Reset();
				this.mousedown = false;
				this.mouseArea.validContent = true;
			}
			catch{}
		}
		#endregion

		#endregion

		#region ..绘制事件
		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnPaint(object sender,PaintEventArgs e)
		{
			this.DrawXorPath(e);
		}
		#endregion

		#region ..改变属性事件
		protected override void OnAdaptAttribute(object sender,AdaptAttributeEventArgs e)
		{
		}
		#endregion

		#region ..ShiftSnap
		internal override bool ShiftSnap
		{
			get
			{
				return false;
			}
		}
		#endregion
	}
}
