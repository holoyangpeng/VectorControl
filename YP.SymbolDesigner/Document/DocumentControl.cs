using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using YP.SVG.DataType;
using YP.SVG;
using YP.VectorControl;
using System.Xml;
using YP.VectorControl.Forms;

namespace YP.SymbolDesigner.Document
{
    #region ..Position
    /// <summary>
    /// 定义图元的文本块相对于图元的位置
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// 图元上边缘
        /// </summary>
        Top,
        /// <summary>
        /// 图元下边缘
        /// </summary>
        Bottom,
        /// <summary>
        /// 图元左边缘
        /// </summary>
        Left,
        /// <summary>
        /// 图元右边缘
        /// </summary>
        Right
    }
    #endregion

    public partial class DocumentControl : UserControl
    {
        #region ..Constructor
        public DocumentControl()
        {
            InitializeComponent();
            this.CreateCanvas(null);
            this.canvas.Dock = DockStyle.Fill;
            this.Controls.Add(this.canvas);
            this.FilePath = string.Empty;
            this.AllowToConnectToShape = false;
            Initialize();
            this.InitializeCanvas();
        }

        public DocumentControl(string filepath)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
            this.CreateCanvas(filepath);
			this.canvas.Dock = DockStyle.Fill;
			this.Controls.Add(this.canvas);
			this.FilePath = System.IO.Path.GetFullPath(filepath);
            Initialize();
			this.InitializeCanvas();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}
        #endregion

        #region ..private fields
        MenuOperator menuOperator;
        System.Windows.Forms.SaveFileDialog savedlg = new SaveFileDialog();
        protected VectorControl.Canvas canvas;
        string symbolFile = null;
        string content = string.Empty;
        string filePath = string.Empty;
        #endregion

        #region ..properties
        public bool AllowToConnectToAnchor { set; get; }

        public bool AllowToConnectToShape { set; get; }
        
        /// <summary>
        /// 获取一个值，指示相对于最后一次保存时，文档是否发生修改
        /// </summary>
        public bool Changed
        {
            get
            {
                if (this.canvas == null)
                    return false;
                return this.canvas.Document.OuterXml != this.content;
            }
        }

        /// <summary>
        /// 向外部公开VectorControl控件
        /// </summary>
        public YP.VectorControl.Canvas Canvas
        {
            get
            {
                return this.canvas;
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string DocumentType { set; get; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    if (System.IO.File.Exists(this.FilePath))
                        this.Text = System.IO.Path.GetFileName(this.filePath);
                    else
                        this.Text = this.filePath;
                }

                if (this.canvas != null)
                    this.content = this.canvas.Document.OuterXml;
                this.SynText();
            }
            get
            {
                return this.filePath;
            }
        }

        /// <summary>
        /// 获取或设置当前文档对应的图元库
        /// </summary>
        public string SymbolFile
        {
            set
            {
                symbolFile = value;
            }
            get
            {
                return symbolFile;
            }
        }
        #endregion

        #region ..Dispose
        public new virtual void Dispose()
        {
            if (this.canvas != null)
            {
                //this.canvas.Loaded -= new EventHandler(canvas_Loaded);
                this.canvas.Document.ConnectionChanged -= new ConnectionChangedEventHandler(ConnectionChanged);
                this.canvas.ElementConnecting -= new ElementConnectEventHandler(ElementConnecting);
                
                this.canvas.Document.ElementInserted -= new SVG.SVGElementChangedEventHandler(ElementInsertRemoved);
                this.canvas.Document.ElementRemoved -= new SVG.SVGElementChangedEventHandler(ElementInsertRemoved);
                this.canvas.Document.ElementChanged -= new AttributeChangedEventHandler(ElementChanged);
                this.canvas.MouseMove -= new MouseEventHandler(canvas_MouseMove);
            }
            base.Dispose();
        }
        #endregion

        #region ..Events
        public enum ToolTipType
        {
            Element,
            Connection,
            Location
        }
        public delegate void ToolTipEventHandler(object sender, string tooltip, ToolTipType type);

        public event ToolTipEventHandler ShowToolTip;
        #endregion

        #region ..CreateControl
        protected virtual void CreateCanvas(string filePath)
        {
            if (filePath == null)
                this.canvas = new VectorControl.Canvas(new SVGLength(100, SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE), new SVGLength(100, SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE));
            else
                this.canvas = new VectorControl.Canvas(filePath);

            CreateProperties();
        }
        #endregion
        
        #region ..CreatProperties
        protected virtual void CreateProperties()
        {
            this.canvas.Document.PropertyGenerator = new PropertyGenerator();
        }
        #endregion

        #region ..InitializeVectorControl
        protected virtual void InitializeCanvas()
		{
			if(this.canvas != null)
			{
				this.canvas.CurrentOperator = Operator.Transform;
                this.canvas.ZoomWhenMouseWheel = true;
                this.canvas.ShowConnectablePoint = false;
                this.canvas.BranchSupport = true;
                this.canvas.AutoFitWindowWhenFirstLoading = true;
                this.canvas.ContextMenuStrip = contextMenuStrip1;
                this.canvas.ConnectionType = YP.SVG.ConnectionType.Dynamic;
                this.canvas.Document.ConnectionChanged += new ConnectionChangedEventHandler(ConnectionChanged);
                this.canvas.ElementConnecting += new ElementConnectEventHandler(ElementConnecting);
                this.canvas.ElementClick += new ElementClickEventHandler(canvas_ElementClick);
                this.canvas.Document.ElementInserted += new SVG.SVGElementChangedEventHandler(ElementInsertRemoved);
                this.canvas.Document.ElementRemoved += new SVG.SVGElementChangedEventHandler(ElementInsertRemoved);
                this.canvas.Document.ElementChanged += new AttributeChangedEventHandler(ElementChanged);
                this.canvas.ElementDropped += new ElementDroppedEventHandler(canvas_ElementDropped);
                this.canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
                this.content = this.canvas.Document.OuterXml;
                this.canvas.TransformBehavior = TransformBehavior.Translate | TransformBehavior.Scale;
			}
		}

        
        #endregion

        #region ..Initialize
        void Initialize()
        {
            this.AllowToConnectToAnchor = this.AllowToConnectToShape = true;
            this.menuOperator = new MenuOperator();
            this.menuOperator.DocumentConrol = this;
            this.savedlg.Filter = DocumentHelper.GetFileFilter(this.DocumentType);
        }
        #endregion

        #region ..SaveFile
        /// <summary>
        /// 保存文件，在保存完毕之后，请记得将文档的修改状态重置
        /// </summary>
        /// <param name="showdlg"></param>
        /// <returns></returns>
        public bool SaveFile(bool showdlg)
        {
            if (this.canvas == null)
                return true;
            string filename = string.Empty;

            try
            {
                this.savedlg.Filter = DocumentHelper.GetFileFilter(this.DocumentType);
                string filePath = this.FilePath;
                if (!System.IO.File.Exists(FilePath))
                    showdlg = true;
                else
                {
                    this.FilePath = filePath;
                    this.savedlg.InitialDirectory = System.IO.Path.GetDirectoryName(this.FilePath);
                    this.savedlg.FileName = System.IO.Path.GetFileName(this.FilePath);
                }
            }
            catch { }

            if (System.IO.File.Exists(this.FilePath) && !showdlg)
                filename = this.FilePath;
            else
            {
                if (this.savedlg.ShowDialog(this) == DialogResult.OK)
                    filename = this.savedlg.FileName;
                else
                    return false;
            }

            try
            {
                System.IO.Stream stream = System.IO.File.Open(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8);
                writer.Write(this.canvas.Document.OuterXml);
                writer.Close();
                stream.Close();
                this.FilePath = filename;
            }
            catch (System.Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            return true;
        }
        #endregion

        #region ..contextMenuStrip1_Opening
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.menuOperator.UpdateCommandStatus(sender);
        }
        #endregion

        #region ..ClickMenuItem
        private void ClickMenuItem(object sender, EventArgs e)
        {
            this.menuOperator.ExecuteCommand(sender as ToolStripItem);
        }
        #endregion

        #region ..Document_ElementInserted
        void ElementChanged(object sender, AttributeChangedEventArgs e)
        {
            this.OnShowToolTip(string.Format("{0}对象,{1}属性改变", e.ChangedElement.Name, e.AttributeName), ToolTipType.Element);
            SynText();
        }

        protected virtual void ElementInsertRemoved(object sender, SVG.SVGElementChangedEventArgs e)
        {
            if (e.Element != null && e.NewParent == this.canvas.CurrentScene)
                e.Element.SetAttribute("isSymbol", "true");
            switch (e.Action)
            {
                case SVGElementChangedAction.Insert:
                    this.OnShowToolTip("插入:" + e.Element.Name, ToolTipType.Element);
                    break;
                case SVGElementChangedAction.Remove:
                    this.OnShowToolTip("删除:" + e.Element.Name, ToolTipType.Element);
                    break;
            }
            SynText();
        }
        #endregion

        #region ..Connetion
        void ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            this.OnShowToolTip(string.Format("连接线{0}改变:{1}", e.ChangedElement.ID, e.Type), ToolTipType.Connection);
        }

        protected virtual bool ElementConnecting(object sender, ElementConnectEventArgs e)
        {
            if (e.Type != ConnectionTargetType.Branch)
                this.OnShowToolTip(string.Format("试图建立连接到{0}图元", e.TargetElement != null ? e.TargetElement.Name : ""), ToolTipType.Connection);
            else if(e.ConnectionElement != null)
            {
                SVG.BasicShapes.SVGConnectionElement branch = e.TargetElement as SVG.BasicShapes.SVGConnectionElement;
                this.OnShowToolTip(string.Format("试图建立分支到，分支开始图元为:{0}", branch.StartElement!= null ? branch.StartElement.ID : "空" ), ToolTipType.Connection);
            }
            if (e.AnchorIndex == -1)
                return this.AllowToConnectToShape;
            if (e.AnchorIndex >= 0)
                return this.AllowToConnectToAnchor;

            return true;
        }
        #endregion

        #region ..OnShowToolTip
        protected virtual void OnShowToolTip(string toolTip, ToolTipType type)
        {
            if (this.ShowToolTip != null)
                this.ShowToolTip(this, toolTip, type);
        }
        #endregion

        #region ..canvas_MouseMove
        void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            PointF p = this.canvas.PointClientToView(e.Location);
            this.OnShowToolTip(string.Format("{0} {1}", p.X, p.Y), ToolTipType.Location);
        }
        #endregion

        #region ..canvas_ElementClick
        void canvas_ElementClick(object sender, ElementClickEventArgs e)
        {
            //双击，如果是子系统，阻止双击同时跳转到子系统
            if (e.ClickType == YP.VectorControl.MouseClickType.DoubleClick && SubSystemDocumentHelper.IsSubSystemElement(e.Element))
            {
                e.Bubble = false;
                var content = SubSystemDocumentHelper.FindSubSystemContentElement(e.Element);
                if (content == null)
                    content = SubSystemDocumentHelper.CreateSubSystemContent(e.Element);

                this.canvas.CurrentScene = content;
            }
        }
        #endregion

        #region ..SynText
        void SynText()
        {
            if (System.IO.File.Exists(this.FilePath))
                this.Text = System.IO.Path.GetFileName(this.filePath);
            else
                this.Text = this.filePath;
            if (this.Changed)
                this.Text += "*";
        }
        #endregion

        #region ..canvas_ElementDropped
        void canvas_ElementDropped(object sender, ElementDroppedEventArgs e)
        {
            //对于拖放进来的图元实例，设置一个标记为isSymbol
            if (e.DroppedInstance is SVG.DocumentStructure.SVGUseElement 
                || e.DroppedInstance is SVG.DocumentStructure.SVGGElement)
                e.DroppedInstance.SetAttribute("isSymbol", "true");
        }
        #endregion

        #region ..GetAllSymbolReference
        /// <summary>
        /// 获取所有的图元示例（包含use对象和直接复制添加的g对象）
        /// </summary>
        /// <param name="svgDoc"></param>
        /// <returns></returns>
        public static XmlNodeList GetAllSymbolReference(SVG.Document.SVGDocument svgDoc)
        {
            return svgDoc.SelectNodes("//*[@isSymbol='true' or local-name(.)='use']");
        }
        #endregion

        #region ..IsSymbolConnected
        /// <summary>
        /// 判断传入的图元是否有连接线连接到其上
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsSymbolConnected(SVGTransformableElement element)
        {
            //如果是组对象（g），则认为当其中任意子对象连通时，则视为连通
            if (element is SVG.DocumentStructure.SVGGElement)
            {
                foreach (SVGElement elm in element.ChildElements)
                {
                    if (!(elm is SVGTransformableElement))
                        continue;

                    if (IsSymbolConnected(elm as SVGTransformableElement))
                        return true;
                }

                return false;
            }

            //如果不是组对象，判断连接到图元的连接线
            var cnn = element.GetAllConnections();
            return cnn != null && cnn.Length > 0;
        }
        #endregion
    }
}
