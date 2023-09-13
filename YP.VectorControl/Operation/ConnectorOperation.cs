using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using YP.SVG;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// 实现连接线的操作
	/// </summary>
	internal class ConnectorOperation:Operation
	{
		#region ..ConnectOperator
		internal enum ConnectOperator
		{
			None,
			MoveAnchor,
			MoveConnector
		}
		#endregion

		#region ..ConnectorOperation
		internal ConnectorOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.mustMouseDown = false;
			this.mouseArea.MouseWheel += new MouseEventHandler(mouseArea_MouseWheel);
		}
		#endregion

		#region ..Dispose
		public override void Dispose()
		{
			base.Dispose ();
			this.mouseArea.MouseWheel -= new MouseEventHandler(mouseArea_MouseWheel);
		}
		#endregion

		#region ..private fields
        bool validMove = false;
		PointF startPoint1 = PointF.Empty;
		SVG.Interface.ISVGPathable cntRender = null,endRender = null;
        int connectIndex = -1, endConnectIndex = -1;
		PointF firstCnt = PointF.Empty;
		PointF[] anchors = null;
		SVG.BasicShapes.SVGConnectionElement selectedConnector = null;
		SVG.BasicShapes.SVGConnectionElement hoverSelector = null;
		int anchorIndex = 0;
		protected ConnectOperator cntOperator = ConnectOperator.None;
		PointF secondCnt = PointF.Empty;
        bool isControlPointOK = false;
        PointF currentPoint = PointF.Empty;
        bool connectionChanged = false;
        SVG.BasicShapes.SVGBranchElement parentBranch = null;
        List<SVG.BasicShapes.SVGBranchElement> branches = new List<SVG.BasicShapes.SVGBranchElement>();
        SVG.BasicShapes.SVGBranchElement currentBranch = null;
        #endregion

		#region ..properties
		/// <summary>
		/// judge whether the current operator execute as connectoroperation
		/// </summary>
		protected bool HasOneConnector
		{
			get
			{
                return this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement;
			}
		}

        SVG.BasicShapes.SVGConnectionElement SelectedConnection
        {
            set
            {
                if (this.selectedConnector != value)
                {
                    this.selectedConnector = value;
                    connectionChanged = true;
                }
            }
        }

		ConnectOperator CntOperator
		{
			set
			{
				if(this.cntOperator != value)
				{
					this.cntOperator = value;
					if(this.cntOperator == ConnectOperator.None)
						this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
                    else if (this.cntOperator == ConnectOperator.MoveAnchor && this.currentBranch != null)
                    {
                        if (this.anchorIndex >= 0 && this.anchorIndex < this.currentBranch.ConnectionPath.PointCount)
                            this.mouseArea.Cursor = System.Windows.Forms.Cursors.SizeAll;
                        else if (this.anchorIndex >= this.currentBranch.ConnectionPath.PointCount)
                        {
                            int index = this.anchorIndex - this.currentBranch.ConnectionPath.PointCount;
                            if (index >= 0 && index < this.currentBranch.ConnectionPath.PointCount - 1)
                            {
                                PointF p = this.currentBranch.ConnectionPath.PathPoints[index];
                                PointF p1 = this.currentBranch.ConnectionPath.PathPoints[index + 1];
                                if (Math.Abs(p.X - p1.X) < 0.01f)
                                    this.mouseArea.Cursor = Forms.Cursors.VScale;
                                else if (Math.Abs(p.Y - p1.Y) < 0.01f)
                                    this.mouseArea.Cursor = Forms.Cursors.HScale;
                                else
                                    this.mouseArea.Cursor = System.Windows.Forms.Cursors.SizeAll;
                            }
                        }
                    }
                    else
                        this.mouseArea.Cursor = System.Windows.Forms.Cursors.SizeAll;
				}
			}
		}
		#endregion

		#region ..OnMouseDown
		protected override void OnMouseDown(object sender, MouseEventArgs e)
		{
			if(!this.IsValidDocument)
				return;
            this.validMove = false;
			this.mousedown = e.Button == MouseButtons.Left;
            currentPoint = new PointF(e.X, e.Y);
			//check whether allows to edit the node
			ProtectType type = this.mouseArea.GetProtectTypeForElement(this.selectedConnector as SVG.SVGElement);
			if((type & ProtectType.NodeEdit) == ProtectType.NodeEdit)
				return;
			if(this.mousedown )
			{
                moved = false;
				PointF cntPoint = PointF.Empty;
                connectIndex = -1;
                SVG.Interface.ISVGPathable render = this.GetConnectTarget(new PointF(e.X, e.Y), ref cntPoint, ref connectIndex, ConnectionTargetType.StartElement | ConnectionTargetType.EndElement, this.selectedConnector);
				if(render != null)
				{
					firstCnt = cntPoint;
					this.startPoint1 = this.mouseArea.PointToVirtualView(cntPoint);
				}
				else if(this.mouseArea.anchorPoint.HasValue)
                    this.startPoint1 = this.mouseArea.anchorPoint.Value;//
                else
                    this.startPoint1 = this.mouseArea.PointToVirtualView(new PointF(e.X, e.Y));
				this.cntRender = render;

				//if move the connector
				if(this.cntOperator == ConnectOperator.MoveConnector)
				{
					if(this.hoverSelector != null && (!this.mouseArea.SVGDocument.SelectCollection.Contains(this.hoverSelector) || this.mouseArea.SVGDocument.SelectCollection.Count != 1))
					{
						this.mouseArea.SVGDocument.ChangeSelectElement(this.hoverSelector as SVG.SVGElement);
						this.selectedConnector = this.hoverSelector;
					}
				}

                isControlPointOK = true;
			}
		}
		#endregion

		#region ..OnMouseMove
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			if(!this.IsValidDocument)
				return;
			try
			{
                if (Math.Abs(e.X - currentPoint.X) < 1 && Math.Abs(e.Y - currentPoint.Y)<1)
                    return;
                Point nativePoint = e.Location;
                if (e is TLMouseEventArgs)
                    nativePoint = (e as TLMouseEventArgs).NativePoint;
                currentPoint = new PointF(e.X, e.Y);
				int r = Canvas.InfoGrap;
				if(this.reverseConnetorIndicatorPath.PointCount > 1)
					this.XORDrawPath(this.reverseConnetorIndicatorPath);
                this.XORDrawPath(this.reverseSnapIndicatorPath);
                this.reverseSnapIndicatorPath.Reset();
                this.reverseConnetorIndicatorPath.Reset();
				//获取当前位置的对象
				PointF cntPoint = new PointF(e.X,e.Y);
				Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
                Point nativeP = this.mouseArea.PointClientToView(nativePoint);
                int cntIndex = -1;
                this.reverseConnetorIndicatorPath.Reset();
                RectangleF bounds = RectangleF.Empty;
                SVG.Interface.ISVGPathable render = null;
                this.validMove = this.validMove || PathHelper.Distance(new PointF(e.X,e.Y), this.startPoint1) > 5;
                bool allowBranch = false;
				#region ..左键
				if(this.mousedown)
				{
					//check whether allows to edit the node
					ProtectType type = this.mouseArea.GetProtectTypeForElement(this.selectedConnector as SVG.SVGElement);
					if((type & ProtectType.NodeEdit) == ProtectType.NodeEdit)
						return;
					this.XORDrawPath(this.reversePath);
					this.reversePath.Reset();
					switch(this.cntOperator)
					{
							#region ..Move the anchor
							//MoveAnchor
						case ConnectOperator.MoveAnchor:
							if(this.anchorIndex >= 0 && this.currentBranch != null && this.selectedConnector != null)
							{
                                ConnectionTargetType connectType = ConnectionTargetType.None;
                                if (anchorIndex == 0)
                                    connectType = ConnectionTargetType.StartElement;
                                else if (anchorIndex == this.currentBranch.ConnectionPath.PointCount - 1)
                                    connectType = ConnectionTargetType.EndElement;
                                if ((this.anchorIndex == 0 && this.currentBranch == this.selectedConnector) || (this.anchorIndex == this.currentBranch.ConnectionPath.PointCount - 1 && this.currentBranch.ChildElements.Count == 0))
                                    render = this.GetConnectTarget(nativePoint, ref cntPoint, ref bounds, ref cntIndex, connectType,this.selectedConnector);
                                int index = this.anchorIndex;
                                PointF p1 = this.AlignToElement(this.currentBranch, new Point(e.X, e.Y), anchorIndex, this.reverseSnapIndicatorPath);
                                this.isControlPointOK = this.currentBranch.GetUpdatePath(this.reversePath, ref index, this.mouseArea.PointClientToView(p1), render as SVGTransformableElement, cntIndex);
                                
                                this.reversePath.Transform(this.mouseArea.GetTotalTransformForElement(this.selectedConnector));
								this.XORDrawPath(this.reversePath);
                                
                                //如果是移动端点，确定是否经过已有连接线，进行分叉
                                allowBranch = (render == null && anchorIndex == 0 && this.currentBranch == this.selectedConnector);
							}
							break;
							#endregion

							#region ..create the new connector
							//AddConnector
						case ConnectOperator.None:
							if(this.mouseArea.CurrentOperator == Operator.Connection)
							{
                                render = this.GetConnectTarget(nativePoint, ref cntPoint, ref bounds, ref cntIndex, ConnectionTargetType.EndElement,this.selectedConnector);
                                SVG.BasicShapes.SVGConnectionElement.GetInitializePath(this.reversePath, null, this.startPoint1, this.mouseArea.PointClientToView(cntPoint), this.mouseArea.ConnectionType, this.cntRender as SVG.SVGTransformableElement, render as SVG.SVGTransformableElement, connectIndex, cntIndex, null); 
								//this.reversePath.AddLine();
                                this.reversePath.Transform(this.mouseArea.CoordTransform);
								this.XORDrawPath(this.reversePath);

                                allowBranch = render == null;
                                if (render != null)
                                    Console.Write("fad");
							}
							break;
							#endregion

							#region ..move the connector
						case ConnectOperator.MoveConnector:						
							this.reversePath.AddPath((this.selectedConnector as SVG.Interface.ISVGPathable).GPath,false);
							this.reverseConnetorIndicatorPath.Reset();
							using(Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.selectedConnector) )
							{
								matrix.Translate(p.X - this.startPoint1.X,p.Y - this.startPoint1.Y, MatrixOrder.Append);
								//matrix.Multiply(this.selectedConnector.TotalTransform);
								this.reversePath.Transform(matrix);
							}
							//using the path to find the connect target
							if(this.reversePath.PointCount > 1)
							{
								PointF start = this.reversePath.PathPoints[0];
								PointF end = this.reversePath.PathPoints[this.reversePath.PointCount - 1];
								r = Canvas.InfoGrap;

								this.cntRender = this.GetConnectTarget(start,ref cntPoint,ref bounds, ref connectIndex,  ConnectionTargetType.StartElement,this.selectedConnector);
								if(this.cntRender != null)
								{
                                    if (connectIndex >= 0 || bounds.IsEmpty)
                                        this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                                    else
                                        this.reverseConnetorIndicatorPath.AddRectangle(bounds);
									this.firstCnt = cntPoint;
								}
                                endConnectIndex = -1;
                                this.endRender = this.GetConnectTarget(end, ref cntPoint, ref bounds,ref endConnectIndex, ConnectionTargetType.EndElement,this.selectedConnector);
								if(this.endRender != null)
								{
                                    if (endConnectIndex >= 0 || bounds.IsEmpty)
                                        this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                                    else
                                        this.reverseConnetorIndicatorPath.AddRectangle(bounds); 
                                    this.secondCnt = cntPoint;
								}
                                
                                //SVGDom.BasicShapes.SVGConnectElement.GetInitializePath(this.reversePath, null, start,end, this.selectedConnector.ConnectType, this.cntRender as SVGDom.SVGTransformableElement, endRender as SVGDom.SVGTransformableElement, connectIndex, endConnectIndex, this.mouseArea.CoordTransform); 
							}
							this.XORDrawPath(this.reversePath);
							this.XORDrawPath(this.reverseConnetorIndicatorPath);
							break;
							#endregion
					}
				}
					#endregion
					//无键 
					#region ..无键
				else
				{
                    this.mouseArea.anchorPoint = null;
					this.hoverSelector = null;
                    this.currentBranch = null;
                    this.anchorIndex = -1;
                    int r1 = this.mouseArea.grapSize;

                    #region ..根据Anchor预判断操作
                    //判断Anchor
                    //if(selectedConnector != null && this.anchors != null)
                    //{
                    //    this.anchorIndex = -1;
                    //    int r1 = this.mouseArea.grapSize;
                    //    int lastIndex = this.anchors.Length - 1;
                    //    int startIndex = 0;
                    //    for(int i = lastIndex;i>=startIndex;i--)
                    //    {
                        //    PointF p1 = this.anchors[i];
                        //    RectangleF rect = new RectangleF(p1.X - r1,p1.Y - r1,2 * r1,2 * r1);
                        //    if(rect.Contains(p))
                        //    {
                        //        this.anchorIndex = i;
                        //        this.CntOperator = ConnectOperator.MoveAnchor;
                        //        return;
                        //    }
                        //    if (i - 1 >= 0)
                        //    {
                        //        float x = (p1.X + this.anchors[i - 1].X) / 2;
                        //        float y = (p1.Y + this.anchors[i - 1].Y) / 2;
                        //        rect = new RectangleF(x - r1, y - r1, 2 * r1, 2 * r1);
                        //        if (rect.Contains(p))
                        //        {
                        //            this.anchorIndex = this.anchors.Length + i - 1;
                        //            this.CntOperator = ConnectOperator.MoveAnchor;
                        //            return;
                        //        }
                        //    }
                        //}
                    //}
                    #endregion

                    #region ..根据Branches预判断操作
                    for (int i = branches.Count - 1; i >= 0; i--)
                    {
                        SVG.BasicShapes.SVGBranchElement branch = branches[i];
                        if (branch == null || branch.ConnectionPath == null)
                            continue;
                        //Anchor优先
                        for (int j = branch.ConnectionPath.PointCount - 1; j >= 0; j--)
                        {
                            PointF p1 = branch.ConnectionPath.PathPoints[j];
                            RectangleF rect = new RectangleF(p1.X - r1,p1.Y - r1,2 * r1,2 * r1);
                            if(rect.Contains(p))
                            {
                                this.anchorIndex = j;
                                this.mouseArea.anchorPoint = p1;
                                this.currentBranch = branch;
                                this.CntOperator = ConnectOperator.MoveAnchor;
                                return;
                            }
                        }

                        //中点
                        for (int j = branch.ConnectionPath.PointCount - 1; j >= 0; j--)
                        {
                            PointF p1 = branch.ConnectionPath.PathPoints[j];
                            if (j - 1 >= 0)
                            {
                                float x = (p1.X + branch.ConnectionPath.PathPoints[j - 1].X) / 2;
                                float y = (p1.Y + branch.ConnectionPath.PathPoints[j - 1].Y) / 2;
                                RectangleF rect = new RectangleF(x - r1, y - r1, 2 * r1, 2 * r1);
                                if (rect.Contains(nativeP))
                                {
                                    this.anchorIndex = branch.ConnectionPath.PointCount + j - 1;
                                    this.mouseArea.anchorPoint = new PointF(x, y);
                                    this.currentBranch = branch;
                                    this.CntOperator = ConnectOperator.MoveAnchor;
                                    return;
                                }
                            }
                        }
                    }
                    #endregion

                    //Find the connect at the mouse point
					SVG.BasicShapes.SVGConnectionElement connect = this.GetConnectionAtClientPoint(nativePoint);
					if(connect != null)
					{
						//only when can broken connector
						if(this.mouseArea.CanBrokenConnector)
						{
							this.CntOperator = ConnectOperator.MoveConnector;
							this.hoverSelector = connect;
						}
						return;
					}

                    if(this.mouseArea.CurrentOperator == Operator.Connection)
                        render = this.GetConnectTarget(nativePoint, ref cntPoint, ref bounds, ref cntIndex, ConnectionTargetType.StartElement | ConnectionTargetType.EndElement,this.selectedConnector);
                    this.CntOperator = ConnectOperator.None;
                }
                #endregion

                //if not moving connector,find the connect
				if(this.cntOperator != ConnectOperator.MoveConnector)
				{
                    if (this.cntOperator == ConnectOperator.MoveAnchor && ((this.anchorIndex > 0 && this.anchorIndex < this.currentBranch.ConnectionPath.PointCount - 1) || this.anchorIndex >= this.currentBranch.ConnectionPath.PointCount))
                        return;
					if(this.mouseArea.CurrentOperator == Operator.Connection||(this.mousedown && this.HasOneConnector))
					{
						//this.reversePath2.Reset();
						if(render != null)
						{
                            this.reverseConnetorIndicatorPath.Reset();
                            if (cntIndex >= 0 || bounds.IsEmpty)
                                this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                            else
                                this.reverseConnetorIndicatorPath.AddRectangle(bounds);
						}

					}
				}

                parentBranch = null;
                //如果是移动端点或者新建连接线，确定是否经过已有连接线，进行分叉
                if (mousedown)
                    Console.WriteLine(allowBranch.ToString());
                if (allowBranch && this.mouseArea.BranchSupport && this.selectedConnector.Type == ConnectionType.Dynamic)
                {
                    SVG.BasicShapes.SVGConnectionElement connection = this.GetConnectionAtClientPoint(e.Location);
                    if (connection != null)
                    {
                        parentBranch = this.GetConnectionBranchAtViewPoint(connection, this.mouseArea.PointClientToView(e.Location), this.mouseArea.Document.SelectCollection .Count == 1? this.mouseArea.Document.SelectCollection[0] as SVG.BasicShapes.SVGBranchElement: null);
                        if (parentBranch != null && parentBranch != this.selectedConnector && parentBranch.type == ConnectionType.Dynamic )
                        {
                            this.reverseConnetorIndicatorPath.Reset();
                            this.reverseConnetorIndicatorPath.AddPath(parentBranch.ConnectionPath, false);
                            this.reverseConnetorIndicatorPath.Transform(this.mouseArea.CoordTransform);
                        }
                    }
                }

                if (this.reverseConnetorIndicatorPath.PointCount > 1 )
                    this.XORDrawPath(this.reverseConnetorIndicatorPath);
                this.XORDrawPath(this.reverseSnapIndicatorPath);
			}
			catch(System.Exception e1)
			{
                this.mouseArea.SVGDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, ExceptionLevel.Normal));
			}
		}
		#endregion

		#region ..OnMouseUp
		protected override void OnMouseUp(object sender, MouseEventArgs e)
		{
            try
            {
                if (!this.IsValidDocument)
                    return;

                if (this.mousedown)
                {
                    //check whether allows to edit the node
                    ProtectType type = this.mouseArea.GetProtectTypeForElement(this.selectedConnector as SVG.SVGElement);
                    if ((type & ProtectType.NodeEdit) == ProtectType.NodeEdit)
                        return;
                    this.XORDrawPath(this.reversePath);
                    this.XORDrawPath(this.reverseConnetorIndicatorPath);
                    this.reverseConnetorIndicatorPath.Reset();
                    this.reversePath.Reset();
                    Point nativePoint = e.Location;
                    if (e is TLMouseEventArgs)
                        nativePoint = (e as TLMouseEventArgs).NativePoint;
                    this.mouseArea.validContent = true;
                    if (!this.validMove)
                        return;
                    PointF p = new Point(e.X, e.Y);
                    switch (this.cntOperator)
                    {
                        #region ..Move the anchor
                        //Move the anchor
                        case ConnectOperator.MoveAnchor:
                            if (this.anchorIndex >= 0 && this.selectedConnector != null && this.currentBranch != null)
                            {
                                //创建分支
                                if (this.parentBranch != null && this.mouseArea.BranchSupport && this.parentBranch != this.selectedConnector)
                                {
                                    SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
                                    bool old = doc.AcceptNodeChanged;
                                    doc.AcceptNodeChanged = false;
                                    SVG.SVGElement element = this.CreateBranch(parentBranch, this.mouseArea.PointToVirtualView(p), null);
                                    if (element != null)
                                    {
                                        //拷贝原有信息
                                        if (this.selectedConnector.HasAttribute("end"))
                                            element.InternalSetAttribute("end", this.selectedConnector.GetAttribute("end"));

                                        if (this.selectedConnector.HasAttribute("x2"))
                                            element.InternalSetAttribute("x2", this.selectedConnector.GetAttribute("x2"));
                                        if (this.selectedConnector.HasAttribute("y2"))
                                            element.InternalSetAttribute("y2", this.selectedConnector.GetAttribute("y2"));
                                        if (this.selectedConnector.HasAttribute("d"))
                                            element.InternalSetAttribute("d", this.selectedConnector.GetAttribute("d"));

                                        this.mouseArea.InvalidateSelection();
                                        doc.AcceptNodeChanged = old;
                                        parentBranch.InternalAppendChild(element);
                                        if (this.selectedConnector.ParentElement != null)
                                            this.selectedConnector.ParentElement.InternalRemoveChild(this.selectedConnector);
                                        this.mouseArea.SVGDocument.ChangeSelectElement(parentBranch);
                                        doc.InvokeUndos();
                                    }
                                }
                                else
                                {
                                    int cntIndex = -1;
                                    ConnectionTargetType connectType = ConnectionTargetType.None;
                                    if (anchorIndex == 0)
                                        connectType = ConnectionTargetType.StartElement;
                                    else if (anchorIndex == this.currentBranch.ConnectionPath.PointCount - 1)
                                        connectType = ConnectionTargetType.EndElement;
                                    SVG.Interface.ISVGPathable render = this.GetConnectTarget(new PointF(e.X, e.Y), ref p, ref cntIndex, connectType,this.selectedConnector);
                                    this.mouseArea.validContent = true;
                                    //invalide the first snap
                                    this.mouseArea.InvalidateSelection();
                                    if (!this.isControlPointOK && this.currentBranch == this.selectedConnector)
                                        this.selectedConnector.InternalRemoveAttribute("d");
                                    p = this.AlignToElement(this.currentBranch, new PointF(e.X, e.Y), anchorIndex, null);
                                    this.currentBranch.UpdateConnect(this.anchorIndex, this.mouseArea.PointToVirtualView(p), render, cntIndex);
                                    this.mouseArea.InvalidateSelection();
                                    this.mouseArea.selectChanged = true;
                                    this.mouseArea.UpdateSelectInfo();
                                    this.mouseArea.InvalidateSelection();
                                    p = this.mouseArea.PointViewToClient(this.startPoint1);
                                    int r = this.mouseArea.clickSize;
                                    this.mouseArea.Invalidate(new Rectangle((int)p.X - 2 * r, (int)p.Y - 2 * r, 4 * r, 4 * r));
                                }
                            }
                            break;
                        #endregion

                        #region ..Move the connector
                        case ConnectOperator.MoveConnector:
                            PointF p1 = this.mouseArea.PointViewToClient(this.startPoint1);
                            if (this.selectedConnector != null && (!PathHelper.FloatEqual(p1.X,p.X) || !PathHelper.FloatEqual(p1.Y,p.Y)))
                            {
                                p = this.mouseArea.PointToVirtualView(p);
                                //get the first connect index
                                int startIndex = connectIndex;
                                //get the second connect index
                                int endIndex = endConnectIndex;

                                this.mouseArea.validContent = true;
                                //invalide the first snap
                                this.mouseArea.InvalidateSelection();
                                this.InvalidateAnchor(this.selectedConnector);
                                this.selectedConnector.UpdateConnect(p.X - this.startPoint1.X, p.Y - this.startPoint1.Y, this.cntRender, startIndex, this.endRender, endIndex);
                                this.mouseArea.InvalidateSelection();
                                this.mouseArea.selectChanged = true;
                                this.mouseArea.UpdateSelectInfo();
                                this.mouseArea.InvalidateSelection();
                                this.InvalidateAnchor(this.selectedConnector);
                            }
                            break;
                        #endregion

                        #region ..Create new connector
                        //create new connector
                        case ConnectOperator.None:
                            if (this.mouseArea.CurrentOperator == Operator.Connection)
                            {
                                PointF cntPoint = p;
                                p = this.mouseArea.PointToVirtualView(p);

                                //create the connector
                                if (this.startPoint1 != p)
                                {
                                    SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
                                    SVG.SVGElement element = null;
                                    bool old = doc.AcceptNodeChanged;
                                    doc.AcceptNodeChanged = false;
                                    PointF start = this.startPoint1;
                                    PointF end = p;
                                    //如果创建分支
                                    if (parentBranch != null)
                                    {
                                        element = this.CreateBranch(parentBranch, p, start);
                                    }
                                    //创建连接线
                                    else
                                    {
                                        int cntIndex = -1;
                                        SVG.Interface.ISVGPathable render = this.GetConnectTarget(nativePoint, ref p, ref cntIndex, ConnectionTargetType.EndElement,this.selectedConnector);

                                        element = doc.CreateElement("connect") as SVG.SVGElement;
                                        element.InternalSetAttribute("x1", start.X.ToString());
                                        element.InternalSetAttribute("y1", start.Y.ToString());
                                        //the end element
                                        if (render != null)
                                        {
                                            SVG.SVGTransformableElement element1 = render as SVG.SVGTransformableElement;
                                            string id = element1.GetAttribute("id");
                                            if (id == null || id.Trim().Length == 0)
                                            {
                                                id = this.mouseArea.SVGDocument.CreateID(element1, this.mouseArea.SVGDocument.RootElement as SVG.SVGElement);
                                                element1.InternalSetAttribute("id", id);
                                            }

                                            id = "#" + id;
                                            if (cntIndex >= 0)
                                                id += "." + cntIndex.ToString();
                                            element.InternalSetAttribute("end", id);
                                        }
                                        element.InternalSetAttribute("type", this.mouseArea.ConnectionType.ToString());
                                        element.InternalSetAttribute("x2", end.X.ToString());
                                        element.InternalSetAttribute("y2", end.Y.ToString());
                                    }

                                    if (this.cntRender != null)
                                    {
                                        SVG.SVGTransformableElement element1 = this.cntRender as SVG.SVGTransformableElement;
                                        string id = element1.GetAttribute("id");
                                        if (id == null || id.Trim().Length == 0)
                                        {
                                            id = this.mouseArea.SVGDocument.CreateID(element1, this.mouseArea.SVGDocument.RootElement as SVG.SVGElement);
                                            element1.InternalSetAttribute("id", id);
                                        }
                                        //								doc.AcceptNodeChanged = false;
                                        id = "#" + id;
                                        using (System.Drawing.Drawing2D.Matrix matrix = this.mouseArea.GetTotalTransformForElement(element1))//.TotalTransform.Clone())
                                        {
                                            PointF[] ps = element1.ConnectionPoints.Clone() as PointF[];
                                            if (ps.Length > 0)
                                            {
                                                matrix.TransformPoints(ps);
                                                int index = Array.IndexOf(ps, firstCnt);
                                                if (connectIndex >= 0)
                                                    id += "." + connectIndex.ToString();
                                                ps = null;
                                            }
                                        }
                                        if (parentBranch != null)
                                            element.InternalSetAttribute("end", id);
                                        else
                                            element.InternalSetAttribute("start", id);
                                    }
                                    doc.AcceptNodeChanged = true;
                                    if (parentBranch != null)
                                        parentBranch.InternalAppendChild(element);
                                    else
                                        element = this.mouseArea.AddElement(element, true, false);
                                    doc.AcceptNodeChanged = old;
                                    doc.InvokeUndos();
                                    if (parentBranch != null)
                                        this.mouseArea.SVGDocument.ChangeSelectElement(parentBranch);
                                    else
                                        this.mouseArea.SVGDocument.ChangeSelectElement(element);
                                    connectionChanged = true;
                                }
                            }
                            break;
                        #endregion
                    }
                }

                //this.anchorIndex = -1;
                //this.cntRender = null;
            }
            catch(System.Exception e1)
            {
                this.mouseArea.SVGDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, ExceptionLevel.Normal));
            }
            finally
            {
                this.mousedown = false;
                this.reversePath.Reset();
                this.reverseConnetorIndicatorPath.Reset();
                this.ResetColor();
                this.mouseArea.validContent = true;
            }
		}
		#endregion

        #region ..CreateBranch
        SVGElement CreateBranch(SVG.BasicShapes.SVGBranchElement parentBranch, PointF viewPoint, PointF? endPoint)
        {
            if (parentBranch == null)
                return null;
            PointF p = viewPoint;
            SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
            int anchorIndex = -1;
            PointF crossPoint = this.GetCrossPoint(parentBranch, p, ref anchorIndex);
            SVGElement element = doc.CreateElement("branch") as SVGElement;
            SVGElement childBranch = null;
            //根连接线
            //if (parentBranch is SVGDom.BasicShapes.SVGConnectElement)
            //{
            childBranch = doc.CreateElement("branch") as SVGElement;
            if (parentBranch.GetAttribute("end").Length > 0)
                childBranch.InternalSetAttribute("end", parentBranch.GetAttribute("end"));
            if (parentBranch.GetAttribute("x2").Length > 0)
                childBranch.InternalSetAttribute("x2", parentBranch.GetAttribute("x2"));
            if (parentBranch.GetAttribute("y2").Length > 0)
                childBranch.InternalSetAttribute("y2", parentBranch.GetAttribute("y2"));
            childBranch.InternalSetAttribute("x1", crossPoint.X.ToString());
            childBranch.InternalSetAttribute("y1", crossPoint.Y.ToString());
            //}
            if (endPoint.HasValue)
            {
                element.InternalSetAttribute("x2", endPoint.Value.X.ToString());
                element.InternalSetAttribute("y2", endPoint.Value.Y.ToString());
            }
            element.InternalSetAttribute("x1", crossPoint.X.ToString());
            element.InternalSetAttribute("y1", crossPoint.Y.ToString());
            doc.AcceptNodeChanged = true;
            string controlStr = string.Empty;

            string str = SVG.DataType.SVGPointList.GetPointsString(new PointF[] { crossPoint });
            string subbranchStr = string.Empty;
            if (anchorIndex > 0)
            {
                GraphicsPath temppath = parentBranch.ConnectionPath;
                var query = from t in temppath.PathPoints select t;
                PointF[] tempps = query.Skip(1).Take(anchorIndex).ToArray();
                controlStr = SVG.DataType.SVGPointList.GetPointsString(tempps);

                tempps = query.Skip(anchorIndex + 1).Take(temppath.PointCount - anchorIndex - 2).ToArray();
                subbranchStr = SVG.DataType.SVGPointList.GetPointsString(tempps);
            }

            controlStr += str;
            parentBranch.InternalSetAttribute("d", controlStr);
            parentBranch.InternalSetAttribute("x2", crossPoint.X.ToString());
            parentBranch.InternalSetAttribute("y2", crossPoint.Y.ToString());
            parentBranch.InternalRemoveAttribute("end");
            if (childBranch != null)
            {
                //将ParentBranch的分支加入child Branch
                SVG.SVGElement[] childs = parentBranch.ChildElements.ToArray();
                foreach (SVG.SVGElement elm in childs)
                {
                    if (elm is SVG.BasicShapes.SVGBranchElement)
                        childBranch.InternalAppendChild(elm);
                }
                childBranch.InternalSetAttribute("d", subbranchStr);
                parentBranch.InternalAppendChild(childBranch);
            }

            return element;
        }
        #endregion

        #region ..OnAdaptAttribute
        protected override void OnAdaptAttribute(object sender, AdaptAttributeEventArgs e)
		{
			
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(object sender, PaintEventArgs e)
		{
			this.DrawXorPath(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			if(this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement)
			{
				SVG.BasicShapes.SVGConnectionElement connect = this.mouseArea.SVGDocument.SelectCollection[0] as SVG.BasicShapes.SVGConnectionElement;
				//check whether allows to edit the node
				ProtectType type = this.mouseArea.GetProtectTypeForElement(connect);
				if((type & ProtectType.NodeEdit) == ProtectType.NodeEdit)
					return;
				PointF[] ps = (PointF[])connect.GetAnchors().Clone();
				if(ps.Length == 0)
					return;
                //int index = 0, lastIndex = ps.Length;
                ////if cannot broken connector
                this.anchors = (PointF[])ps.Clone();
                this.mouseArea.CoordTransform.TransformPoints(ps);
                this.branches.Clear();
                this.PaintBranch(e.Graphics, connect);
				this.SelectedConnection = connect;
			}
			else
                this.SelectedConnection = null;

            if (connectionChanged)
            {
                this.branches.Clear();
                this.NormalizeBranches(this.selectedConnector);
            }

            this.connectionChanged = false;
		}
		#endregion

        #region ..PaintBranch
        void PaintBranch(Graphics g, SVG.BasicShapes.SVGBranchElement cnn)
        {
            if (cnn == null || cnn.ConnectionPath == null || cnn.ConnectionPath.PointCount < 2)
                return;
            branches.Add(cnn);
            float r = this.mouseArea.clickSize * 2f / 4f;
            using (SolidBrush brush = new SolidBrush(this.mouseArea.AnchorColor))
            {
                using (Pen pen = new Pen(brush))
                {
                    PointF[] ps = cnn.ConnectionPath.PathPoints.Clone() as PointF[];
                    this.mouseArea.CoordTransform.TransformPoints(ps);
                    
                    for (int i = 1; i < cnn.ConnectionPath.PointCount; i++)
                    {
                        float x = (ps[i].X + ps[i - 1].X) / 2;
                        float y = (ps[i].Y + ps[i - 1].Y) / 2;
                        PointF p1 = ps[i];
                        brush.Color = this.mouseArea.AnchorColor;
                        if(cnn.OwnerConnection != null && cnn.OwnerConnection.Type == ConnectionType.Dynamic)
                            g.FillEllipse(brush, x - r, y - r, 2 * r, 2 * r);
                        if (i < ps.Length)
                        {
                            if(i == ps.Length - 1 && cnn.HasChildBranch)
                            {
                                brush.Color = this.mouseArea.HighlightAnchor;
                                g.FillRectangle(brush, new RectangleF( p1.X - 2, p1.Y - 2, 4, 4));
                            }
                            else
                                g.FillEllipse(brush, p1.X - 1.2f * r, p1.Y - 1.2f * r, 2.4f * r, 2.4f * r);
                        }
                    }

                    r = this.mouseArea.clickSize * 2f / 3f;

                    if (cnn.ChildElements.Count == 0)
                    {
                        if (cnn.EndElement != null)
                        {
                            PointF p1 = ps.Last();
                            brush.Color = this.mouseArea.HighlightAnchor;
                            r = 1.8f * r;
                            RectangleF rect = new RectangleF(p1.X - r, p1.Y - r, 2 * r, 2 * r);
                            g.FillEllipse(Brushes.White, rect);
                            brush.Color = ControlPaint.Dark(brush.Color);
                            pen.Brush = brush;
                            g.DrawEllipse(pen, rect);
                            r = this.mouseArea.clickSize * 2f / 4f;
                            brush.Color = this.mouseArea.HighlightAnchor;
                            rect = new RectangleF(p1.X - r, p1.Y - r, 2 * r, 2 * r);
                            g.FillEllipse(brush, rect);
                            pen.Brush = brush;
                            g.DrawEllipse(pen, rect);
                        }
                        else
                        {
                            PointF p1 = ps.Last();
                            brush.Color = ControlPaint.DarkDark(brush.Color);
                            pen.Brush = brush;
                            g.DrawRectangle(pen, p1.X - r, p1.Y - r, 2 * r, 2 * r);
                            brush.Color = this.mouseArea.NormalColor;
                            g.FillRectangle(brush, p1.X - r, p1.Y - r, 2 * r, 2 * r);
                            
                        }
                    }

                    if (cnn is SVG.BasicShapes.SVGConnectionElement)
                    {
                        SVG.BasicShapes.SVGConnectionElement connect = cnn as SVG.BasicShapes.SVGConnectionElement;
                        if (connect.StartElement != null)
                        {
                            brush.Color = ControlPaint.Dark(brush.Color);
                            pen.Brush = brush;
                            g.DrawEllipse(pen, ps[0].X - r, ps[0].Y - r, 2 * r, 2 * r);
                            brush.Color = this.mouseArea.HighlightAnchor;
                            g.FillEllipse(brush, ps[0].X - r, ps[0].Y - r, 2 * r, 2 * r);
                        }
                        else
                        {  
                            brush.Color = ControlPaint.DarkDark(brush.Color);
                            pen.Brush = brush;
                            g.DrawRectangle(pen, ps[0].X - r, ps[0].Y - r, 2 * r, 2 * r);
                            brush.Color = this.mouseArea.NormalColor;
                            g.FillRectangle(Brushes.White, ps[0].X - r, ps[0].Y - r, 2 * r, 2 * r);
                        }
                    }
                }
            }

            foreach (SVG.SVGElement cnn1 in cnn.ChildElements)
                if(cnn1 is SVG.BasicShapes.SVGBranchElement)
                    this.PaintBranch(g, cnn1 as SVG.BasicShapes.SVGBranchElement);
        }
        #endregion

        #region ..PaintEndAnchor
        void PaintEndAnchor(SVG.BasicShapes.SVGBranchElement cnn, Graphics g)
        {
             float r = this.mouseArea.clickSize * 2f / 4f;
             using (SolidBrush brush = new SolidBrush(this.mouseArea.AnchorColor))
             {
                 using (Pen pen = new Pen(brush))
                 {
                     using (Matrix matrix = this.mouseArea.CoordTransform)
                     {
                         if ((cnn as SVG.Interface.ISVGPathable).GPath == null || (cnn as SVG.Interface.ISVGPathable).GPath.PointCount < 2)
                             return;
                         PointF[] ps2 = new PointF[] { (cnn as SVG.Interface.ISVGPathable).GPath.PathPoints.Last() };
                         matrix.TransformPoints(ps2);
                         PointF p1 = ps2[0];
                         r = this.mouseArea.clickSize * 2f / 3f;
                         if (cnn.EndElement != null)
                         {
                             brush.Color = this.mouseArea.HighlightAnchor;
                             r = 1.8f * r;
                             RectangleF rect = new RectangleF(p1.X - r, p1.Y - r, 2 * r, 2 * r);
                             g.FillEllipse(Brushes.White, rect);
                             brush.Color = ControlPaint.Dark(brush.Color);
                             pen.Brush = brush;
                             g.DrawEllipse(pen, rect);
                             r = this.mouseArea.clickSize * 2f / 4f;
                             brush.Color = this.mouseArea.HighlightAnchor;
                             rect = new RectangleF(p1.X - r, p1.Y - r, 2 * r, 2 * r);
                             g.FillEllipse(brush, rect);
                             pen.Brush = brush;
                             g.DrawEllipse(pen, rect);
                         }
                         else
                         {
                             brush.Color = this.mouseArea.NormalColor;
                             g.FillRectangle(brush, p1.X - r, p1.Y - r, 2 * r, 2 * r);
                             brush.Color = ControlPaint.DarkDark(brush.Color);
                             pen.Brush = brush;
                             g.DrawRectangle(pen, p1.X - r, p1.Y - r, 2 * r, 2 * r);
                         }

                         foreach (SVG.SVGElement cnn1 in cnn.ChildElements)
                             if(cnn1 is SVG.BasicShapes.SVGBranchElement)
                             this.PaintEndAnchor(cnn1 as SVG.BasicShapes.SVGBranchElement, g);
                     }
                 }
             }
        }
        #endregion

        #region ..mouseArea_MouseWheel
        private void mouseArea_MouseWheel(object sender, MouseEventArgs e)
		{
			if(this.reverseConnetorIndicatorPath.PointCount > 1)
				this.mouseArea.Invalidate();
			this.reverseConnetorIndicatorPath.Reset();
			this.reversePath.Reset();
		}
		#endregion

        #region ..GetConnection
        /// <summary>
		/// Get the connector at the mouse point
		/// </summary>
		/// <param name="viewPoint">the point in the view coordinate</param>
		/// <returns></returns>
		internal SVG.BasicShapes.SVGConnectionElement GetConnectionAtClientPoint(Point clientPoint)
		{
            SVGElementCollection list = new SVGElementCollection();
            if (this.mouseArea.CurrentScene is SVG.Interface.ISVGContainer)
                list = (this.mouseArea.CurrentScene as SVG.Interface.ISVGContainer).ChildElements;
            else
                list.Add(this.mouseArea.CurrentScene);
            SVG.SVGElement element = this.mouseArea.GetElementAtViewPoint(this.mouseArea.PointClientToView(clientPoint), list);
			return element as SVG.BasicShapes.SVGConnectionElement;
		}
		#endregion

        #region ..GetConnectionBranchAtViewPoint
        /// <summary>
        /// 判断连接线是否有分支在指定的坐标上，如果有，返回分支
        /// </summary>
        /// <param name="rootConnection"></param>
        /// <param name="viewPoint"></param>
        SVG.BasicShapes.SVGBranchElement GetConnectionBranchAtViewPoint(SVG.BasicShapes.SVGBranchElement rootConnection, PointF viewPoint, SVG.BasicShapes.SVGBranchElement connectionElement)
        {
            if (rootConnection == null)
                return null;
            SVG.BasicShapes.SVGBranchElement result = null;
            if (rootConnection.ConnectionPath.IsOutlineVisible(viewPoint, Canvas.InPathPen))
                result = rootConnection;
            foreach (SVG.SVGElement branch in rootConnection.ChildElements)
            {
                if (result != null)
                    break;
                if (branch is SVG.BasicShapes.SVGBranchElement)
                {
                    result = GetConnectionBranchAtViewPoint(branch as SVG.BasicShapes.SVGBranchElement, viewPoint, connectionElement);
                }
            }
            if (result != null && this.mouseArea.TrytoConnectElement(result.OwnerConnection, -1, 0, ConnectionTargetType.Branch, connectionElement))
                return result;
            return null;
        }
        #endregion

        #region ..InvalidateAnchor
        /// <summary>
		/// Invalidate the anchor
		/// </summary>
		/// <param name="cnn"></param>
		void InvalidateAnchor(SVG.BasicShapes.SVGConnectionElement cnn)
		{
			if(cnn == null)
				return;
			PointF[] ps = cnn.GetAnchors().Clone() as PointF[];
            if (ps.Length < 1)
                return;
			this.mouseArea.GetTotalTransformForElement(cnn).TransformPoints(ps);
			float r = this.mouseArea.clickSize * 2f /3f;
			for(int i = 0;i<ps.Length;i++)//(PointF p1 in ps)
			{
				PointF p1 = ps[i];
				RectangleF rect = new RectangleF(p1.X -r,p1.Y -r,2 * r,2 * r);
				Rectangle rect1 = Rectangle.Ceiling(rect);
				this.mouseArea.Invalidate(new Rectangle(rect1.X - 2,rect1.Y - 2,rect1.Width + 4,rect1.Height + 4));
			}
		}
		#endregion

        #region ..GetCrossPoint
        /// <summary>
        /// 获取分支路径上最接近指定坐标的点
        /// </summary>
        /// <param name="branch"></param>
        /// <param name="viewPoint"></param>
        /// <returns></returns>
        PointF GetCrossPoint(SVG.BasicShapes.SVGBranchElement branch, PointF viewPoint, ref int anchorIndex)
        {
            if (branch != null)
            {
                GraphicsPath path = (branch as SVG.Interface.ISVGPathable).GPath;
                //判断端点
                var query = from t in path.PathPoints where SVG.PathHelper.Distance(t,viewPoint) < Canvas.PenWidth select t;
                if (query.Count() > 0)
                {
                    anchorIndex = Array.IndexOf(path.PathPoints, query.First());
                    return query.First();
                }
                using (GraphicsPath path1 = new GraphicsPath())
                {
                    for (int i = 1; i < path.PointCount; i++)
                    {
                        path1.Reset();
                        PointF p1 = path.PathPoints[i - 1];
                        PointF p2 = path.PathPoints[i];
                        path1.AddLine(p1, p2);
                        if (path1.IsOutlineVisible(viewPoint, Canvas.InPathPen))
                        {
                            PointF? resultPoint = null;
                            PointF? resultPoint1 = null;
                            if((viewPoint.X - p1.X ) * (viewPoint.X - p2.X) < 0)
                                resultPoint = new PointF(viewPoint.X, p1.Y + (viewPoint.X - p1.X) / (p2.X - p1.X) * (p2.Y - p1.Y));

                            if ((viewPoint.Y - p1.Y) * (viewPoint.Y - p2.Y) < 0)
                                resultPoint1 = new PointF(p1.X + (viewPoint.Y - p1.Y) / (p2.Y - p1.Y) * (p2.X - p1.X), viewPoint.Y);

                            anchorIndex = i - 1;
                            if (resultPoint.HasValue != resultPoint1.HasValue)
                                return resultPoint1.HasValue ? resultPoint1.Value : resultPoint.Value;
                            else if (resultPoint.HasValue)
                                return PathHelper.Distance(resultPoint.Value, p1) + PathHelper.Distance(resultPoint.Value, p2) < PathHelper.Distance(resultPoint1.Value, p1) + PathHelper.Distance(resultPoint1.Value, p2) ? resultPoint.Value : resultPoint1.Value;
                            else
                                return p2;
                        }
                    }
                }
            }
            return viewPoint;
        }
        #endregion

        #region ..NormalizeBranches
        void NormalizeBranches(SVG.BasicShapes.SVGBranchElement root)
        {
            if (root == null || root.ConnectionPath == null)
                return;
            if (!this.branches.Contains(root))
                this.branches.Add(root);

            foreach (SVG.SVGElement child in root.ChildElements)
                if(child is SVG.BasicShapes.SVGBranchElement)
                this.NormalizeBranches(child as SVG.BasicShapes.SVGBranchElement);
        }
        #endregion

        #region ..AlignToElement
        bool moved = false;
        PointF AlignToElement(SVG.BasicShapes.SVGBranchElement branch, PointF point, int anchorIndex, GraphicsPath indicatorPath)
        {
            if (branch != null && branch.ConnectionPath != null && branch.ConnectionPath.PointCount > 1)
            {
                if (indicatorPath != null)
                    indicatorPath.Reset();
                float? x = null, y = null;
                int index = anchorIndex;
                GraphicsPath path = branch.ConnectionPath.Clone() as GraphicsPath;
                path.Transform(this.mouseArea.CoordTransform);
                bool hori = true, vert = true;
                if (index >= path.PointCount)
                {
                    index = index - path.PointCount;
                    if (index >= 0 && index < branch.ConnectionPath.PointCount - 1)
                    {
                        PointF p = branch.ConnectionPath.PathPoints[index];
                        PointF p1 = branch.ConnectionPath.PathPoints[index + 1];
                        if (Math.Abs(p.X - p1.X) < 0.01f)
                            vert = false;
                        else if (Math.Abs(p.Y - p1.Y) < 0.01f)
                            hori = false;
                    }
                }
                PointF current = path.PathPoints[index];
                PointF pre = index - 1 >= 0 ? path.PathPoints[index - 1] : current;
                PointF next = index + 1 < path.PointCount ? path.PathPoints[index + 1] : current;
                float minx = Math.Min(current.X, Math.Min(pre.X, next.X));
                float maxx = Math.Max(current.X, Math.Max(pre.X, next.X));
                float miny = Math.Min(current.Y, Math.Min(pre.Y, next.Y));
                float maxy = Math.Max(current.Y, Math.Max(pre.Y, next.Y));
                float? vertexX = null, vertexY = null;
                for (int i = index; i >= 0; i--)
                {
                    PointF p = path.PathPoints[i];
                    
                    //hori
                    if (hori &&Math.Abs(p.X - point.X) < Canvas.SnapMargin)
                    {
                        if (!x.HasValue && i - 1 >= 0 && Math.Abs(p.X - path.PathPoints[i-1].X) < 1)
                        {
                            x = p.X;
                            if (indicatorPath != null && moved)
                            {
                                indicatorPath.StartFigure();
                                indicatorPath.AddLine(p.X, (float)Math.Min(p.Y, miny) - SnapIndicatorExtensionLength, p.X, (float)Math.Max(p.Y, maxy) + SnapIndicatorExtensionLength);
                            }
                        }
                        else if (!vertexX.HasValue)
                            vertexX = p.X;
                    }
                    if (vert && Math.Abs(p.Y - point.Y) < Canvas.SnapMargin)
                    {
                        if (!y.HasValue && i - 1 >= 0 && Math.Abs(p.Y - path.PathPoints[i - 1].Y) < 1)
                        {
                            y = p.Y;
                            if (indicatorPath != null && moved)
                            {
                                indicatorPath.StartFigure();
                                indicatorPath.AddLine((float)Math.Min(p.X, minx) - SnapIndicatorExtensionLength, p.Y, (float)Math.Max(p.X, maxx) + SnapIndicatorExtensionLength, p.Y);
                            }
                        }
                        else if (!vertexY.HasValue)
                            vertexY = p.Y;
                    }

                    if (x.HasValue && y.HasValue)
                        break;
                }

                for (int i = index + 1; i < path.PointCount; i++)
                {
                    if (x.HasValue && y.HasValue)
                        break;
                    PointF p = path.PathPoints[i];
                    PointF p1 = path.PathPoints[i - 1];
                    //hori
                    if (hori && Math.Abs(p.X - point.X) < Canvas.SnapMargin)
                    {
                        if (!x.HasValue && i - 1 >= 0 && Math.Abs(p.X - path.PathPoints[i - 1].X) < 1)
                        {
                            x = p.X;
                            if (indicatorPath != null && moved)
                            {
                                indicatorPath.StartFigure();
                                indicatorPath.AddLine(p.X, (float)Math.Min(p.Y, miny) - SnapIndicatorExtensionLength, p.X, (float)Math.Max(p.Y, maxy) + SnapIndicatorExtensionLength);
                            }
                        }
                        else if (!vertexX.HasValue)
                            vertexX = p.X;
                    }
                    if (vert && Math.Abs(p.Y - point.Y) < Canvas.SnapMargin)
                    {
                        if (!y.HasValue && i - 1 >= 0 && Math.Abs(p.Y - path.PathPoints[i - 1].Y) < 1)
                        {
                            y = p.Y;
                            if (indicatorPath != null && moved)
                            {
                                indicatorPath.StartFigure();
                                indicatorPath.AddLine((float)Math.Min(p.X, minx) - SnapIndicatorExtensionLength, p.Y, (float)Math.Max(p.X, maxx) + SnapIndicatorExtensionLength, p.Y);
                            }
                        }
                        else if (!vertexY.HasValue)
                            vertexY = p.Y;
                    }
                }

                if (!moved && indicatorPath != null)
                    indicatorPath.Reset();
                if (!moved && !x.HasValue && !y.HasValue)
                    moved = true;
                return new PointF(x.HasValue ? x.Value : (vertexX.HasValue ? vertexX.Value : point.X), y.HasValue ? y.Value : (vertexY.HasValue ? vertexY.Value : point.Y));
            }
            return point;
        }
        #endregion
    }
}
