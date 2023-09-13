using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PerfectSVG.VectorControl;
using PerfectSVG.SVGDom.DataType;

namespace PerfectSVG.SymbolDesigner
{
    public partial class DocumentControl : UserControl
    {
        #region ..Constructor
        public DocumentControl()
        {
            InitializeComponent();
            vectorControl1 = new PerfectSVG.VectorControl.VectorControl(new SVGLength(100, SVGDom.Enum.LengthType.SVG_LENGTHTYPE_PERCENTAGE), new SVGLength(100, SVGDom.Enum.LengthType.SVG_LENGTHTYPE_PERCENTAGE));
            this.vectorControl1.Dock = DockStyle.Fill;
            this.Controls.Add(this.vectorControl1);
            this.FilePath = string.Empty;
            this.InitializeVectorControl();
        }

        public DocumentControl(string filepath)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			this.vectorControl1 = new PerfectSVG.VectorControl.VectorControl(filepath);
			this.vectorControl1.Dock = DockStyle.Fill;
			this.Controls.Add(this.vectorControl1);
			this.FilePath = System.IO.Path.GetFullPath(filepath);
			this.InitializeVectorControl();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}
        #endregion

        #region ..private fields
        MenuOperator menuOperator;
        System.Windows.Forms.SaveFileDialog savedlg = new SaveFileDialog();
        VectorControl.VectorControl vectorControl1;
        #endregion

        #region ..properties
        public bool AllowToConnectToAnchor { set; get; }

        public bool AllowToConnectToShape { set; get; }

        /// <summary>
        /// 向外部公开VectorControl控件
        /// </summary>
        public PerfectSVG.VectorControl.VectorControl VectorControl
        {
            get
            {
                return this.vectorControl1;
            }
        }

        public string FilePath { set; get; }
        #endregion

        #region ..InitializeVectorControl
        void InitializeVectorControl()
		{
			if(this.vectorControl1 != null)
			{
				this.vectorControl1.Document.AddCustomPropertyForAllElements (new PerfectSVG.VectorControl.Property.CustomPropertyItem("Name",typeof(string),"常规","name"));
				this.vectorControl1.CurrentOperator = PerfectSVG.VectorControl.Enum.Operator.Transform;
                this.vectorControl1.Loaded += new EventHandler(vectorControl1_Loaded);
                //this.vectorControl1.DocumentChanged += new EventHandler(vectorControl1_DocumentChanged);
                //this.vectorControl1.SelectionChanged += new EventHandler(Document_SelectionChanged);
                //this.vectorControl1.Document.ElementInserted += new VectorElementChangedEventHandler(Document_ElementInserted);
                //this.vectorControl1.Document.ElementRemoved += new VectorElementChangedEventHandler(Document_ElementRemoved);
                //this.vectorControl1.Document.ConnectChanged += new ConnectChangedEventHandler(Document_ConnectChanged);
                //this.vectorControl1.ElementDropped +=new ElementDroppedEventHandler(vectorControl1_ElementDropped); 
                //this.vectorControl1.Document.ElementChanged += new AttributeChangedEventHandler(Document_ElementChanged);
				this.vectorControl1.DrawConnectPoint = false;
                this.vectorControl1.ElementConnecting += new ElementConnectEventHandler(vectorControl1_ElementConnecting);
                this.vectorControl1.ContextMenuStrip = contextMenuStrip1;
                this.vectorControl1.ZoomWhenMouseWheel = true;
                this.vectorControl1.Grid = new VectorControl.Struct.Grid(false);
                this.vectorControl1.Rule = new VectorControl.Struct.Rule(false, PerfectSVG.VectorControl.Enum.UnitType.Pixel);
                this.vectorControl1.Document.ElementInserted += new SVGDom.VectorElementChangedEventHandler(Document_ElementInserted);
                this.vectorControl1.CanvasColor = Color.Black;
                this.vectorControl1.Stroke = new VectorControl.Struct.Stroke(Color.FromArgb(255, 255, 127));
                this.vectorControl1.TextBlockStyle = new VectorControl.Struct.TextBlockStyle(Color.FromArgb(255, 255, 127), SVGDom.Enum.Alignment.Center, SVGDom.Enum.VerticalAlignment.Middle);
                //this.vectorControl1.ElementClick += new ElementClickEventHandler(vectorControl1_ElementClick);
                this.vectorControl1.ConnectType = PerfectSVG.VectorControl.Enum.ConnectType.Dynamic;
                //this.vectorControl1.ElementConnecting += new ElementConnectEventHandler(vectorControl1_ElementConnecting);
                //this.vectorControl1.PaintConnectablePoint += new PaintConnectablePointEventHandler(vectorControl1_PaintConnectablePoint);
                this.vectorControl1.SnapToElement = true;
                //this.vectorControl1.TransformType = PerfectSVG.VectorControl.Enum.TransformType.Translate
                    //| PerfectSVG.VectorControl.Enum.TransformType.Scale | PerfectSVG.VectorControl.Enum.TransformType.Select;
			}
            this.menuOperator = new MenuOperator();
            this.menuOperator.DocumentConrol = this;
            this.AllowToConnectToAnchor = this.AllowToConnectToShape = true;
		}
        #endregion

        #region ..vectorControl1_Loaded
        void vectorControl1_Loaded(object sender, EventArgs e)
        {
            //第一次导入，滚动画布到正中
            this.vectorControl1.ScrollToCenter();
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
            if (this.vectorControl1 == null)
                return true;
            string filename = string.Empty;

            try
            {
                Uri uri = new Uri(this.FilePath);
                if (!uri.IsFile)
                    showdlg = true;
                else
                {
                    this.FilePath = uri.AbsolutePath;
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
                writer.Write(this.vectorControl1.XmlCode);
                writer.Close();
                stream.Close();
                //保存之后记得将控件的修改状态重置
                //this.vectorControl1.Changed = false;
                this.FilePath = filename;
                this.Text = this.FilePath;
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
        void Document_ElementInserted(object sender, SVGDom.VectorElementChangedEventArgs e)
        {
            if (!(e.Element is SVGDom.DocumentStructure.SVGUseElement) && e.Element is SVGDom.SVGTransformableElement)
                (e.Element as SVGDom.SVGTransformableElement).CreateDefaultConnectPoint = false;

            //文本块
            if (e.Element is SVGDom.Text.SVGTextBlockElement)
            {
                //use对象的文本块不折行
                if (e.Element.ParentElement is SVGDom.DocumentStructure.SVGUseElement)
                    e.Element.SetAttribute("wrap", "nowrap");
            }
        }
        #endregion

        #region ..vectorControl1_ElementConnecting
        bool vectorControl1_ElementConnecting(object sender, ElementConnectEventArgs e)
        {
            if (e.AnchorIndex == -1)
                return !(e.TargetElement is SVGDom.DocumentStructure.SVGUseElement);
            return true;
        }
        #endregion
    }
}
