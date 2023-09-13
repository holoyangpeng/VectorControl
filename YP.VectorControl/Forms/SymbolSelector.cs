using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using YP.SVG;
using YP.SVG.DocumentStructure;
using YP.SVG.BasicShapes;
using YP.SVG.Interface;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>图元选择器。</para>
	/// <para>通过图元选择器,可以为用户提供当前环境提供的<see cref="Symbol">图元</see>和<see cref="VectorControl">连接线</see>。</para>
	/// <para>用户可以直接拖曳图元或连接线到编辑环境中以创建实例。</para>
	/// </summary>
	[ToolboxItem(false)]
	public class SymbolSelector:OutlookBar
	{
		#region ..构造及消除
		/// <summary>
		/// 传入图元文件路径,创建一个SymbolSelector选择器
		/// </summary>
		/// <param name="configpath">图元文件路径</param>
        public SymbolSelector(string configpath)
            : base()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.IconHeight = 60;
            this.LoadConfiguration(configpath);
            this.showtip = true;
            this.dodragdrap = true;
            this.AllowDrop = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
            //			}
        }
		#endregion

        #region ..private fields
        YP.SVG.Document.SVGDocument doc = null;
        string filePath = string.Empty;
        #endregion

        #region ..常量
        const int BorderMargin = 5;
		#endregion

		#region ..分析ShapeDocument
		void ParseShapes(YP.SVG.Document.SVGDocument doc)
		{
			if(doc == null)
				return;
			System.Xml.XmlNodeList list = doc.GetElementsByTagName("group");
			for(int i = 0;i<list.Count;i++)
			{
				System.Xml.XmlElement element = list[i] as System.Xml.XmlElement;
				string enabled = element.GetAttribute("enabled").Trim().ToLower();
				if(element == null||string.Compare(enabled,"false") ==0)
					continue;
				ShapeGroup group = new ShapeGroup();
                group.Tag = element;
				group.ID = element.GetAttribute("id");
				this.items.Add(group);
				System.Xml.XmlNodeList list1 = element.ChildNodes;//GetElementsByTagName("symbol")
				for(int j = 0;j<list1.Count;j++)
				{
					SVGSymbolElement symbol = list1[j] as SVGSymbolElement;
					if(symbol != null)
						group.Add(symbol);
					SVGConnectionElement connect = list1[j] as SVGConnectionElement;
					if(connect != null)
						group.Add(connect);
				}
			}
		}
		#endregion

		#region ..绘制IconPanel
		internal override void PaintIconArea(object sender, PaintEventArgs e)
		{
            try
            {
                if (this.doc != null)
                    this.doc.drawDemoString = false;
                int index = this.SelectedIndex;
                if (index >= 0)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    ShapeGroup group = this.items[index] as ShapeGroup;
                    Point pos = this.iconLabel.AutoScrollPosition;
                    index = (int)Math.Max(0, -pos.Y / IconHeight);
                    int count = this.iconLabel.Height / IconHeight;
                    int right = (int)Math.Min(group.Count, index + count + 2);
                    e.Graphics.TranslateTransform(pos.X, pos.Y);
                    using (System.Drawing.Drawing2D.Matrix matrix = new Matrix())
                    {
                        for (int i = index; i < right; i++)
                        {
                            IOutlookBarPath shape = group[i] as IOutlookBarPath;
                            if (shape == null)
                                continue;
                            Color backcolor = iconColor;
                            Color foreColor = Color.Black;
                            bool selected = false;
                            if (shape == this.SelectedObject as IOutlookBarPath)
                            {
                                backcolor = SystemColors.Highlight;
                                foreColor = SystemColors.HighlightText;
                                selected = true;
                            }

                            Rectangle bounds = this.GetShapeRectangle(i);
                            int b = BorderMargin;
                            Rectangle srect = new Rectangle(bounds.X + 2 * b, bounds.Y + 2 * b, bounds.Height - 4 * b, bounds.Height - 4 * b);
                            using (Pen pen = new Pen(SystemColors.Highlight))
                            {
                                using (Brush brush = new SolidBrush(backcolor))
                                {
                                    GraphicsPath path1 = null;
                                    if (shape is IOutlookBarPath)
                                        path1 = (shape as IOutlookBarPath).GPath;
                                    if (path1 == null || path1.PointCount <= 1)
                                        continue;
                                    matrix.Reset();

                                    using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                                    {
                                        sf.LineAlignment = StringAlignment.Far;
                                        using (GraphicsPath path = path1.Clone() as GraphicsPath)
                                        {
                                            path.Flatten();
                                            RectangleF rect = path.GetBounds();
                                            if (rect.Width == 0)
                                                rect.Width = srect.Width;
                                            if (rect.Height == 0)
                                                rect.Height = srect.Height;
                                            if (!rect.IsEmpty)
                                            {
                                                float cx = rect.X + rect.Width / 2f;
                                                float cy = rect.Y + rect.Height / 2f;
                                                using (Brush brush1 = new SolidBrush(foreColor))
                                                {
                                                    using (Pen pen1 = new Pen(foreColor))
                                                    {
                                                        {
                                                            if (selected)
                                                            {
                                                                Rectangle temp = new Rectangle(bounds.X, bounds.Y, bounds.Height, bounds.Height);
                                                                e.Graphics.DrawRectangle(pen, temp);
                                                                path.Reset();
                                                                path.AddRectangle(temp);
                                                                path.AddRectangle(new Rectangle(bounds.X + b, bounds.Y + b, bounds.Height - 2 * b, bounds.Height - 2 * b));
                                                                path.FillMode = FillMode.Alternate;
                                                                path.Transform(matrix);
                                                                e.Graphics.FillPath(brush1, path);

                                                            }
                                                            matrix.Translate(srect.X + srect.Width / 2f - cx, srect.Y + srect.Height / 2f - cy);
                                                            matrix.Translate(cx, cy);
                                                            float scale = (float)Math.Min((srect.Height) / rect.Width, (srect.Height) / rect.Height);
                                                            matrix.Scale(scale, scale);
                                                            matrix.Translate(-cx, -cy);

                                                            if (shape is SVGSymbolElement)
                                                                (shape as SVGSymbolElement).Render(e.Graphics, matrix);
                                                            else if (shape is SVGConnectionElement)
                                                            {
                                                                using (GraphicsPath path2 = path1.Clone() as GraphicsPath)
                                                                {
                                                                    path2.Transform(matrix);
                                                                    e.Graphics.DrawPath(pen1, path2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    float width = (float)Math.Max(1, bounds.Width - bounds.Height - ContentMargin - 2 * LeftMargin);
                                                    RectangleF rect1 = new RectangleF(bounds.X + bounds.Height + LeftMargin + ContentMargin, bounds.Bottom - 20, width, 20);
                                                    e.Graphics.FillRectangle(brush, rect1);
                                                    if (selected)
                                                    {
                                                        pen.Color = foreColor;
                                                        pen.DashStyle = DashStyle.Dash;
                                                        e.Graphics.DrawRectangle(pen, rect1.X, rect1.Y, rect1.Width, rect1.Height);
                                                    }
                                                    if (shape is IOutlookBarPath)
                                                    {
                                                        string title = (shape as IOutlookBarPath).Title;
                                                        if(title.Trim().Length > 0)
                                                            e.Graphics.DrawString(title, SystemInformation.MenuFont, brush1, rect1, sf);
                                                        else
                                                            e.Graphics.DrawString((shape as IOutlookBarPath).ID, SystemInformation.MenuFont, brush1, rect1, sf);
                                                    }
                                                    //else if (shape is Connect)
                                                    //{
                                                    //    string title = (shape as IPath).Title;
                                                    //    e.Graphics.DrawString((shape as IPath).ID, SystemInformation.MenuFont, brush1, rect1, sf);
                                                    //}
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (this.doc != null)
                    this.doc.drawDemoString = true;
            }
		}
		#endregion

        #region ..Load
        public override void LoadConfiguration(string filePath)
        {
            this.Controls.Clear();
            items.Clear();
            try
            {
                this.filePath = filePath;
                doc = SVG.Document.SvgDocumentFactory.CreateSimpleDocumentFromFile(filePath);
                this.ParseShapes(doc);
                if (Uri.IsWellFormedUriString(filePath, UriKind.RelativeOrAbsolute))
                {
                    Uri uri = new Uri(filePath);
                    this.filePath = uri.AbsolutePath;
                }
            }
            catch
            {

            }
            this.AddIconArea();
            this.Invalidate(true);
        }
        #endregion

        #region ..NewGroup
        /// <summary>
        /// 新建组
        /// </summary>
        /// <param name="groupName"></param>
        public void NewGroup(string groupName)
        {
            if (doc != null && System.IO.File.Exists(this.filePath))
            {
                var gElm = doc.DocumentElement.SelectSingleNode(string.Format("//*[local-name()='group' and @id='{0}']", groupName));
                if (gElm != null)
                    throw new Exception(string.Format("{0}已经存在", groupName));
                else
                {
                    var elm = doc.CreateElement("group");
                    if (doc.DocumentElement != null)
                    {
                        elm = doc.DocumentElement.AppendChild(elm) as System.Xml.XmlElement;
                        elm.SetAttribute("id", groupName);
                        ShapeGroup group = new ShapeGroup();
                        group.ID = groupName;
                        group.Tag = elm;
                        this.items.Add(group);
                        CalculateIconArea();
                        CurrentGroup = group;
                        this.Invalidate(true);
                        Save();
                    }
                }
            }
        }
        #endregion

        #region ..DeleteCurrentGroup
        /// <summary>
        /// 删除当前组
        /// </summary>
        public void DeleteCurrentGroup()
        {
            var group = this.CurrentGroup as ShapeGroup;
            if (group != null )
            {
                var element = group.Tag as System.Xml.XmlElement;
                if (element != null && element.ParentNode != null)
                    element.ParentNode.RemoveChild(element);
                if (this.items.Contains(group))
                {
                    int index = this.items.IndexOf(group);
                    this.items.Remove(group);
                    if (index < this.items.Count)
                        this.CurrentGroup = this.items[index] as IGroup;
                    else if (index - 1 >= 0 && this.items.Count > 0)
                        this.CurrentGroup = this.items[index - 1] as IGroup;
                }
                this.CalculateIconArea();
                Save();
                this.Invalidate(true);
            }
        }
        #endregion

        #region ..RenameGroup
        /// <summary>
        /// 重命名组
        /// </summary>
        /// <param name="groupName"></param>
        public void RenameCurrentGroup(string groupName)
        {
            var group = this.CurrentGroup as ShapeGroup;
            if (group != null && group.ID != groupName)
            {
                  var gElm = doc.DocumentElement.SelectSingleNode(string.Format("//*[local-name()='group' and @id='{0}']", groupName));
                  if (gElm != null)
                      throw new Exception(string.Format("{0}已经存在", groupName));
                  else if(group.Tag is System.Xml.XmlElement)
                  {
                      group.ID = groupName;
                      (group.Tag as System.Xml.XmlElement).SetAttribute("id", groupName);
                      Save();
                      this.Invalidate(true);
                  }
            }
        }
        #endregion

        #region ..DeleteCurrentSymbol
        /// <summary>
        /// 删除当前图元
        /// </summary>
        public void DeleteCurrentSymbol()
        {
            var group = this.CurrentGroup as ShapeGroup;
            if (group != null)
            {
                var path = this.SelectedObject as IOutlookBarPath;
                if (path is SVGElement && group.IndexOf(path) >= 0)
                {
                    this.SelectedPathIndex = -1;
                    int index = group.IndexOf(path);
                    (path as SVGElement).ParentNode.RemoveChild(path as SVGElement);
                    group.RemoveAt(group.IndexOf(path));
                    this.CalculateIconArea();
                    if (index < group.Count)
                        this.SelectedPathIndex = index;
                    else if (index - 1 >= 0 && group.Count > 0)
                        this.SelectedPathIndex = index - 1;
                    this.Invalidate(true);
                    this.Save();
                }
            }
        }
        #endregion

        #region ..AddSymbol
        /// <summary>
        /// 往图元库中加入新图元
        /// </summary>
        /// <param name="elements">要添加的图元对象列表</param>
        /// <param name="name">symbol的name</param>
        /// <returns>返回最后生成的svgelement副本</returns>
        public SVGElement AddSymbol(SVGElementCollection elements, string name)
        {
            if (elements == null || elements.Count == 0)
                throw new Exception("不能创建内容为空的图元");
            var group = this.CurrentGroup as ShapeGroup;
            if (group != null && group.Tag is SVGElement)
            {
                var parent = group.Tag as SVGElement;
                var doc = parent.OwnerDocument;
                var symbol = doc.DocumentElement.SelectSingleNode(string.Format("//*[@id='{0}']", name)) as SVGElement;
                if (symbol != null)
                {
                    throw new Exception(string.Format("图元{0}已经存在", name));
                }
                else
                {
                    symbol = doc.CreateElement("symbol") as SVGElement;
                    symbol.ID = name;
                    foreach (SVGElement elm in elements)
                    {
                        if (elm is SVGBranchElement)
                        {
                            elm.SetAttribute("groupfirst", "true");
                            //SVGElement temp = (elm as SVG.Interface.BasicShapes.ISVGBasicShape).ConvertToPath() as YP.SVG.SVGElement;
                            //temp.SetAttribute("fill", "none");
                            symbol.AppendChild(doc.ImportNode(elm, true));
                        }
                        else if (elm is SVGUseElement)
                        {
                            SVGElement elm1 = (elm as SVGUseElement).RefElement;
                            if (elm1 != null)
                            {
                                var g = elm.OwnerDocument.CreateElement("g");
                                g.SetAttribute("transform", elm.GetAttribute("transform"));
                                for (int i = 0; i < elm1.ChildNodes.Count; i++)
                                    g.AppendChild(elm1.ChildNodes[i].CloneNode(true));
                                symbol.AppendChild(doc.ImportNode(g, true));
                            }
                        }
                        else
                            symbol.AppendChild(doc.ImportNode(elm, true));
                    }
                    symbol = parent.AppendChild(symbol) as SVGElement;
                    group.Add(symbol as IOutlookBarPath);
                    this.SelectedPathIndex = group.IndexOf(symbol as IOutlookBarPath);
                    this.CalculateIconArea();
                    CalculateIconViewSize();
                    this.Invalidate(true);
                    this.Save();
                    return symbol.Clone() as SVGElement;
                }
            }
            return null;
        }
        #endregion

        #region ..Save
        void Save()
        {
            if (doc != null && System.IO.File.Exists(this.filePath))
            {
                try
                {
                    doc.Save(this.filePath);
                }
                catch { }
            }
        }
        #endregion

        #region ..RefreshCurrentSymbol
        /// <summary>
        /// 刷新当前图元
        /// </summary>
        public void RefreshCurrentSymbol()
        {
            if (this.selectIconIndex >= 0)
                this.InvalidateShape(this.selectIconIndex);
            this.Save();
        }
        #endregion

        #region ..HasValidSymbolFile
        /// <summary>
        /// 判断当前是否有合法的Symbol定义文件
        /// </summary>
        /// <returns></returns>
        public bool HasValidSymbolFile()
        {
            return this.doc != null && this.doc.DocumentElement != null;
        }
        #endregion

        #region ..HasSelectedGroup
        /// <summary>
        /// 判断当前是否有选中的组
        /// </summary>
        /// <returns></returns>
        public bool HasSelectedGroup()
        {
            return this.CurrentGroup != null;
        }
        #endregion
    }
}
