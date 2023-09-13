using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;

using YP.SymbolDesigner.Document;
using YP.CommonControl;
using YP.CommonControl.Dock;
using YP.VectorControl.Forms;

namespace YP.SymbolDesigner
{
    public partial class Form1 : Form
    {
        #region ..Constructor
        public Form1()
        {
            InitializeComponent();

            //构造ToolBar
            string configFile = "Resources/toolbar.xml";
            if (System.IO.File.Exists(configFile))
            {
                toolBar = new Tool.ToolBar(configFile);
                this.Controls.Add(toolBar.ToolStrip);
                menuStrip1.SendToBack();
            }
            
            //构造工具栏
            string path = @"Resources/symbol.xml";// new System.IO.FileInfo().FullName;
            //创建图元选择器
            symbolSelector = new SymbolSelector(path);
            //设置图元选择器外观
            symbolSelector.TitleColor = SystemColors.Menu;
            symbolSelector.ContentColor = ControlPaint.LightLight(SystemColors.Menu);
            symbolSelector.GradientTitle = false;
            symbolSelector.Height = 300;
            Panel panel = new Panel();
            Button btn = new Button();
            btn.Text = "管理图元库 >>";
            btn.Click += new EventHandler(ShowSymbolMenu);
            btn.Dock = DockStyle.Top;
            
            symbolSelector.Dock = DockStyle.Fill;
            symbolSelector.ContextMenuStrip = this.symbolContextMenuStrip;
            symbolSelector.ElementDoubleClicked += new EventHandler(symbolSelector_ElementDoubleClicked);
            panel.Controls.Add(symbolSelector);
            panel.Controls.Add(btn);
            panel.Height = 300;
            Content c = new Content("图元", panel);
            this.layoutManager1.AddContentToLayout(c, LayoutStyle.Left, new Size(200, 250));
            toolContent = c;

            propertygrid = new PropertyGrid();
            Content c1 = new Content("属性框", propertygrid);
            this.layoutManager1.InsertContentAfter(c1, new Size(200, 200), c);
            propertyContent = c1;

            Content c2 = new Content("导航", this.navigator);
            this.layoutManager1.InsertContentAfter(c2, new Size(200, 100), c1);
            navigatorContent = c2;

            this.layoutManager1.AddControlIntoMainArea(this.tabControl);
            this.tabControl.Visible = false;
            this.tabControl.IDEBorder = true;
            this.tabControl.BackColor = SystemColors.Menu;
            this.tabControl.SelectedIndexChanged += new EventHandler(tabControl_SelectedIndexChanged);
            //处理事件，点击关闭按钮时关闭文档
            this.tabControl.ButtonClick += new CommonControl.TabControl.TabControl.ButtonClickEventHandler(tabControl_ButtonClick);

            this.filedialog.Filter = DocumentHelper.GetAllSupportFileFilter();

            this.filedialog.FilterIndex = DocumentHelper.DocumentTypes.Length+1;
            this.filedialog.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, "Sample");

            //this.layoutManager1.HideContent(navigatorContent);
            //创建新建文档子菜单
            foreach (string doc in DocumentHelper.DocumentTypes)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(doc);
                newToolStripMenuItem.DropDownItems.Add(item);
            }
            if (newToolStripMenuItem.DropDownItems.Count > 0)
            {
                (newToolStripMenuItem.DropDownItems[0] as ToolStripMenuItem).ShowShortcutKeys = true;
                (newToolStripMenuItem.DropDownItems[0] as ToolStripMenuItem).ShortcutKeys = Keys.Control | Keys.N;
            }

            DirectoryInfo dir = new System.IO.DirectoryInfo(System.IO.Path.Combine(Application.StartupPath, "sample"));
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                this.OpenFile(file.FullName);
            }

            if (this.tabControl.TabPages.Count > 0)
                this.tabControl.SelectedIndex = 0;
        }
        #endregion

        #region ..private fields
        OpenFileDialog filedialog = new OpenFileDialog();
        SymbolSelector symbolSelector = null;
        YP.CommonControl.TabControl.TabControl tabControl = new YP.CommonControl.TabControl.TabControl();
        Content toolContent = null;
        Content propertyContent = null;
        Content navigatorContent = null;
        Tool.ToolBar toolBar;
        PropertyGrid propertygrid;
        DocumentControl currentControl = null;
        Controls.Navigator navigator = new Controls.Navigator();
        MenuOperator menuOperator = new MenuOperator();
        Hashtable fileManager = new Hashtable();
        #endregion

        #region ..properties
        /// <summary>
        /// 当前活动的文档窗口
        /// </summary>
        DocumentControl CurrentDocumentControl
        {
            set
            {
                if (this.currentControl != value)
                {
                    if (this.currentControl != null && this.currentControl.Canvas != null)
                        this.currentControl.Canvas.PropertyGrid = null;
                    this.currentControl = value;
                    this.menuOperator.DocumentConrol = value;
                    //同步状态
                    if (this.currentControl != null)
                    {
                        this.toolBar.Canvas = currentControl.Canvas;
                        currentControl.Canvas.PropertyGrid = this.propertygrid;
                        this.navigator.VectorControl = currentControl.Canvas;
                        var symbol = currentControl.SymbolFile;
                        if(symbol == null)
                            symbol = DocumentHelper.GetDefaultSymbolFile(this.currentControl.DocumentType);
                        Uri uri = new Uri(Path.Combine(Application.StartupPath, "Symbol/"));
                        uri = new Uri(uri, symbol);
                        symbolSelector.LoadConfiguration(uri.AbsoluteUri);
                        //symbolSelector.Load(System.IO.Path.Combine("Symbol", currentControl.SymbolFile == null ? DocumentHelper.GetDefaultSymbolFile(this.currentControl.DocumentType): this.currentControl.SymbolFile));
                    }
                    else
                    {
                        this.toolBar.Canvas = null;
                        this.navigator.VectorControl = null;
                    }
                }
            }
        }
        #endregion

        #region ..ExecuteMenuItem
        private void ExecuteMenuItem(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item == null || item.Text == null)
                return;
            string text = MenuOperator.NormalizeItemText(item);
            switch (text)
            {
                case "打开":
                    try
                    {
                        if (this.filedialog.ShowDialog(this) == DialogResult.OK)
                        {
                            OpenFile(this.filedialog.FileName);
                        }
                    }
                    catch (System.Exception e1)
                    {
                        MessageBox.Show(e1.Message);
                    }
                    break;
                case "退出":
                    this.Close();
                    break;
                case "关闭":
                    this.tabControl_ButtonClick(this.tabControl, CommonControl.TabControl.TabControl.ButtonStyle.Close);
                    break;
                case "工具条":
                    this.toolBar.ToolStrip.Visible = !this.toolBar.ToolStrip.Visible;
                    break;
                case "图元":
                    if (this.layoutManager1.IsContentVisible(toolContent))
                        this.layoutManager1.HideContent(this.toolContent);
                    else
                        this.layoutManager1.ShowContent(this.toolContent);
                    break;
                case "导航":
                    if (this.layoutManager1.IsContentVisible(navigatorContent))
                        this.layoutManager1.HideContent(this.navigatorContent);
                    else
                        this.layoutManager1.ShowContent(this.navigatorContent);
                    break;
                case "属性":
                    if (this.layoutManager1.IsContentVisible(this.propertyContent))
                        this.layoutManager1.HideContent(this.propertyContent);
                    else
                        this.layoutManager1.ShowContent(this.propertyContent);
                    break;
                case "关于":
                    MessageBox.Show("VectorControl，让图形应用开发更高效");
                    break;
                case "newgroup":
                    {
                        Dialog.PromptDialog dlg = new Dialog.PromptDialog();
                        while (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            try
                            {
                                this.symbolSelector.NewGroup(dlg.Value);
                                break;
                            }
                            catch (System.Exception e1)
                            {
                                MessageBox.Show(e1.Message);
                            }
                        }
                    }
                    break;
                case "opennewsymbol":
                    {
                        SaveFileDialog symDlg = new SaveFileDialog();
                        symDlg.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, "Symbol");
                        symDlg.Filter = "图元库(*.symbol)|*.symbol|所有文件(*.*)|*.*";
                        if (symDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            string filePath = symDlg.FileName;
                            var stream = System.IO.File.CreateText(filePath);
                            stream.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><symbols/>");
                            stream.Close();
                            this.symbolSelector.LoadConfiguration(filePath);
                            //更新当前文档图元库
                            if (this.currentControl != null)
                                this.currentControl.SymbolFile = filePath;

                            filePath = System.IO.Path.GetTempFileName();
                            stream = System.IO.File.CreateText(filePath);
                            stream.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><symbols/>");
                            stream.Close();
                            SymbolSelector tmp = new SymbolSelector("");
                            tmp.LoadConfiguration(filePath);

                            tmp.NewGroup("test");
                            var temp = tmp.AddSymbol(this.currentControl.Canvas.Selection, "test1");
                            if (temp != null)
                            {
                            }
                        }
                    }
                    break;
                case "opensymbol":
                    {
                        OpenFileDialog symDlg = new OpenFileDialog();
                        symDlg.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, "Symbol");
                        symDlg.Filter = "图元库(*.symbol)|*.symbol|所有文件(*.*)|*.*";
                        if (symDlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            this.symbolSelector.LoadConfiguration(symDlg.FileName);
                            
                            //更新当前文档图元库
                            if (this.currentControl != null)
                                this.currentControl.SymbolFile = symDlg.FileName;
                        }
                    }
                    break;
                case "deletegroup":
                    {
                        if (MessageBox.Show("删除之后不能恢复，确认删除当前组？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.symbolSelector.DeleteCurrentGroup();
                        }
                    }
                    break;
                case "renamegroup":
                    {
                        Dialog.PromptDialog dlg = new Dialog.PromptDialog();
                        while (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            try
                            {
                                this.symbolSelector.RenameCurrentGroup(dlg.Value);
                                break;
                            }
                            catch (System.Exception e1)
                            {
                                MessageBox.Show(e1.Message);
                            }
                        }
                    }
                    break;
                case "deletesymbol":
                    {
                        if (MessageBox.Show("删除之后不能恢复，确认删除当前图元？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.symbolSelector.DeleteCurrentSymbol();
                        }
                    }
                    break;
                case "addselection":
                    {
                        if (this.currentControl != null
                            && this.currentControl.Canvas != null)
                        {
                            string text1 = "<root><symbol id='test'><rect x='0' width='100' height='200'/></symbol><symbol id='test2'><ellipse cx='0' cy='50' rx='100' ry='100'/><rect x='0' y='0' width='100' height='100'/></symbol></root>";
                            SVG.Document.SVGDocument doc = new SVG.Document.SVGDocument();
                            doc.LoadXml(text1);

                            foreach (System.Xml.XmlNode node in doc.DocumentElement.ChildNodes)
                            {
                                if (node is SVG.DocumentStructure.SVGSymbolElement)
                                {
                                    this.symbolSelector.AddSymbol((node as SVG.DocumentStructure.SVGSymbolElement).ChildElements, (node as SVG.SVGElement).ID);
                                }
                            }
                        }

                        Console.WriteLine(this.symbolSelector.SelectedGroup.GetAttribute("id"));
                    }
                    break;
                case "symbolproperty":
                    {
                        this.UpdateSymbolProperty();
                    }
                    break;
                case "dropsymbol":
                    {
                        var symbol = this.symbolSelector.SelectedObject as SVG.DocumentStructure.SVGSymbolElement;
                        if (this.currentControl != null && symbol != null)
                            this.currentControl.Canvas.DropSymbol(symbol, new PointF(50, 50));
                    }
                    break;
                default:
                    menuOperator.ExecuteCommand(item);
                    break;
            }
        }
        #endregion

        #region ..AddFile
        void AddFile(string title, DocumentControl dc)
        {
            string name = title;
            name = System.IO.Path.GetFileName(title);
            YP.CommonControl.TabControl.TabPage page = new YP.CommonControl.TabControl.TabPage(name, dc);
            this.tabControl.TabPages.Add(page);
            //将文件添加到列表
            if (dc.FilePath.Length > 0)
                fileManager.Add(dc.FilePath.ToLower(), dc);
            dc.Tag = page;
            dc.TextChanged += new EventHandler(dc_TextChanged);
            dc.ShowToolTip += new DocumentControl.ToolTipEventHandler(dc_ShowToolTip);
        }
        #endregion

        #region ..OpenFile
        void OpenFile(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                DocumentControl dc = null;
                filename = System.IO.Path.GetFullPath(filename);
                //检测一下该文件名是否已经打开
                if (!this.fileManager.ContainsKey(filename.ToLower()))
                {
                    dc = DocumentHelper.CreateDocument(DocumentHelper.GetDocumentType(System.IO.Path.GetExtension(filename)), filename.ToLower());
                    this.AddFile(filename, dc);
                }
                //如果存在，则打开相应的窗体
                else
                {
                    dc = this.fileManager[filename.ToLower()] as DocumentControl;
                    if (dc != null && dc.Tag is YP.CommonControl.TabControl.TabPage)
                        this.tabControl.SelectedTab = dc.Tag as YP.CommonControl.TabControl.TabPage;
                }
            }
        }
        #endregion

        #region ..tabControl_SelectedIndexChanged
        void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            DocumentControl dc = null;
            if (this.tabControl.SelectedTab != null)
                dc = this.tabControl.SelectedTab.Control as DocumentControl;
            this.CurrentDocumentControl = dc;
            
            this.tabControl.Visible = this.tabControl.TabPages.Count > 0;
            this.toolBar.ToolStrip.Enabled = this.tabControl.Visible;

            if (this.tabControl.SelectedTab != null)
                this.Text = "VectorControl图元设计器 - " + this.tabControl.SelectedTab.Text;
            else
                this.Text = "VectorControl图元设计器";

            foreach(ToolStripItem item in this.menuStrip1.Items)
            {
                if(item.Tag is string && (item.Tag as string).ToLower() == "needdoc")
                    item.Visible = dc != null;
            }
        }
        #endregion

        #region ..tabControl_ButtonClick
        void tabControl_ButtonClick(object sender, CommonControl.TabControl.TabControl.ButtonStyle style)
        {
            if (style == YP.CommonControl.TabControl.TabControl.ButtonStyle.Close)
            {
                DocumentControl dc = this.tabControl.SelectedTab.Control as DocumentControl;
                this.CloseFile(dc);
            }
        }
        #endregion

        #region ..CloseFile
        bool CloseFile(DocumentControl dc)
        {
            if (dc != null)
            {
                dc.ShowToolTip -= new DocumentControl.ToolTipEventHandler(dc_ShowToolTip);
                dc.TextChanged -= new EventHandler(dc_TextChanged);
                //关闭之前，如果文档发生修改，询问用户是否保存
                if (this.currentControl.Changed)//frm.Canvas.Changed)
                {
                    DialogResult result = MessageBox.Show(this, "文档\"" + this.tabControl.SelectedTab.Text + "\"已经发生修改，是否保存?", "保存", MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        if (!dc.SaveFile(false))
                            return false;
                    }
                    else if (result == DialogResult.Cancel)
                        return false;
                }
                dc.Dispose();
                this.tabControl.TabPages.Remove(dc.Tag as YP.CommonControl.TabControl.TabPage);
                tabControl_SelectedIndexChanged(this.tabControl, EventArgs.Empty);
                //从列表中删除文件
                if (fileManager.ContainsKey(dc.FilePath.ToLower()))
                    fileManager.Remove(dc.FilePath.ToLower());
            }
            return true;
        }
        #endregion

        #region ..Form1_Load, Load时，初始化布局
        private void Form1_Load(object sender, EventArgs e)
        {
            //when the form load, initialize the tool box to make all of them expand
            this.toolContent.ParentGroup.MakeGroupFill();
            this.navigatorContent.ParentGroup.MakeGroupFill();
            this.propertyContent.ParentGroup.MakeGroupFill();
            tabControl_SelectedIndexChanged(this.tabControl, EventArgs.Empty);
        }
        #endregion

        #region ..DropDownOpenning
        private void DropDownOpenning(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripItemCollection list = null;
            if (sender is ToolStripMenuItem)
                list = (sender as ToolStripMenuItem).DropDownItems;
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                ToolStripMenuItem item1 = list[i] as ToolStripMenuItem;
                switch (MenuOperator.NormalizeItemText(item1))
                {
                    case "工具条":
                        item1.Checked = this.toolBar.ToolStrip.Visible;
                        break;
                    case "属性":
                        item1.Checked = this.layoutManager1.IsContentVisible(this.propertyContent);
                        break;
                    case "图元":
                        item1.Checked = this.layoutManager1.IsContentVisible(this.toolContent);
                        break;
                    case "导航":
                        item1.Checked = this.layoutManager1.IsContentVisible(this.navigatorContent);
                        break;
                    default:
                        if (list[i].Tag is string && list[i].Tag.ToString().ToLower() == "needdoc")
                            list[i].Visible = this.currentControl != null;
                        break;
                }
            }

            if(sender is ToolStripItem && (sender as ToolStripItem).Tag is string && (sender as ToolStripItem).Tag.ToString().ToLower() == "needdoc")
                this.menuOperator.UpdateCommandStatus(sender);
        }
        #endregion

        #region ..dc_ShowToolTip
        void dc_ShowToolTip(object sender, string tooltip, DocumentControl.ToolTipType type)
        {
            switch (type)
            {
                case DocumentControl.ToolTipType.Element:
                    this.lbElement.Text = tooltip;
                    break;
                case DocumentControl.ToolTipType.Connection:
                    this.lbConnection.Text = tooltip;
                    break;
                case DocumentControl.ToolTipType.Location:
                    this.lbMouse.Text = tooltip;
                    break;
            }
        }
        #endregion

        #region ..ShowSymbolMenu
        void ShowSymbolMenu(object sender, EventArgs e)
        {
            Control c = sender as Control;
            this.symbolContextMenuStrip.Show(c, new Point(c.Width, 0), ToolStripDropDownDirection.Right);
        }
        #endregion

        #region ..symbolContextMenuStrip_Opened
        private void symbolContextMenuStrip_Opened(object sender, EventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            foreach (ToolStripItem item in menu.Items)
            {
                string text = MenuOperator.NormalizeItemText(item);
                switch (text)
                {
                    case "newgroup":
                        item.Enabled = this.symbolSelector.HasValidSymbolFile();
                        break;
                    case "deletegroup":
                        item.Enabled = this.symbolSelector.HasSelectedGroup();
                        break;
                    case "renamegroup":
                        item.Enabled = this.symbolSelector.HasSelectedGroup();
                        break;
                    case "deletesymbol":
                        item.Enabled = this.symbolSelector.SelectedObject != null;
                        break;
                    case "addselection":
                        item.Enabled = this.symbolSelector.HasSelectedGroup() && this.currentControl != null && this.currentControl.Canvas.Selection.Count > 0;
                        break;
                    case "symbolproperty":
                        item.Enabled = this.symbolSelector.SelectedObject != null;
                        break;
                }
            }
        }
        #endregion

        #region ..UpdateSymbolProperty
        void UpdateSymbolProperty()
        {
            if (this.symbolSelector.SelectedObject is SVG.DocumentStructure.SVGSymbolElement)
            {
                Dialog.SymbolPropertyDialog dlg = new Dialog.SymbolPropertyDialog();
                dlg.SymbolElement = this.symbolSelector.SelectedObject as SVG.DocumentStructure.SVGSymbolElement;
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    this.symbolSelector.RefreshCurrentSymbol();
            }
        }
        #endregion

        #region ..symbolSelector_ElementDoubleClicked
        void symbolSelector_ElementDoubleClicked(object sender, EventArgs e)
        {
            this.UpdateSymbolProperty();
        }
        #endregion

        #region ..newToolStripMenuItem_DropDownItemClicked
        private void newToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            string text = item.Text;
            var dc = DocumentHelper.CreateDocument(text, null);
            var name = string.Format("未命名-{0}", fileManager.Count + 1);
            this.AddFile(name, dc);
            dc.FilePath = name;
        }
        #endregion

        #region ..dc_TextChanged
        void dc_TextChanged(object sender, EventArgs e)
        {
            DocumentControl dc = sender as DocumentControl;
            if (dc != null && dc.Tag is YP.CommonControl.TabControl.TabPage)
                (dc.Tag as YP.CommonControl.TabControl.TabPage).Text = dc.Text;
        }
        #endregion

        #region ..OnClosing
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            for (int i = 0; i < this.tabControl.TabPages.Count; i++)
            {
                if (!this.CloseFile(this.tabControl.SelectedTab.Control as DocumentControl))
                {
                    e.Cancel = true;
                    return;
                }
                i--;
            }
        }
        #endregion
    }
}
