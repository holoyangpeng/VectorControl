using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using YP.SymbolDesigner.Document;
using YP.VectorControl;
using YP.SVG;
using YP.VectorControl.Forms;

namespace YP.SymbolDesigner
{
    /// <summary>
    /// deal the menu command
    /// </summary>
    internal class MenuOperator
    {
        #region ..Constructor
        public MenuOperator()
        {
            this.exportdlg.Filter = "SVG文件(*.svg)|*.svg";
        }
        #endregion

        #region ..private fields
        DocumentControl documentControl = null;
        System.Windows.Forms.SaveFileDialog savedlg = new SaveFileDialog();
        System.Windows.Forms.SaveFileDialog exportdlg = new SaveFileDialog();
        #endregion

        #region ..public properties
        public DocumentControl DocumentConrol
        {
            set
            {
                this.documentControl  = value;
            }
            get
            {
                return this.documentControl;
            }
        }

        public VectorControl.Canvas VectorControl
        {
            get
            {
                if(this.documentControl != null)
                    return this.documentControl.Canvas;
                return null;
            }
        }
        #endregion

        #region ..ExecuteCommand
        public void ExecuteCommand(ToolStripItem item)
        {
            if (this.VectorControl == null || item ==null)
                return;
            Grid grid = this.VectorControl.Grid;
            Guide guide = this.VectorControl.Guide;
            SVGElementCollection selection = this.VectorControl.Selection;
            Form owner = this.VectorControl.FindForm();

            string text = NormalizeItemText(item);
            switch (text)
            {
                case "撤消":
                    this.VectorControl.Undo();
                    break;
                case "重做":
                    this.VectorControl.Redo();
                    break;
                case "顶部对齐":
                    this.VectorControl.Align(AlignElementsType.Top);
                    break;
                case "中心对齐":
                    this.VectorControl.Align(AlignElementsType.VerticalCenter);
                    break;
                case "底部对齐":
                    this.VectorControl.Align(AlignElementsType.Bottom);
                    break;
                case "左对齐":
                    this.VectorControl.Align(AlignElementsType.Left);
                    break;
                case "居中对齐":
                    this.VectorControl.Align(AlignElementsType.HorizontalCenter);
                    break;
                case "右对齐":
                    this.VectorControl.Align(AlignElementsType.Right);
                    break;
                case "顶部分布":
                    this.VectorControl.Distribute(DistributeType.Top);
                    break;
                case "垂直居中分布":
                    this.VectorControl.Distribute(DistributeType.VerticalCenter);
                    break;
                case "底部分布":
                    this.VectorControl.Distribute(DistributeType.Bottom);
                    break;
                case "左侧分布":
                    this.VectorControl.Distribute(DistributeType.Left);
                    break;
                case "右侧分布":
                    this.VectorControl.Distribute(DistributeType.Right);
                    break;
                case "水平居中分布":
                    this.VectorControl.Distribute(DistributeType.HorizontalCenter);
                    break;
                case "置于顶层":
                    this.VectorControl.UpdateElementsLayer(ElementLayer.Top);
                    break;
                case "置于底层":
                    this.VectorControl.UpdateElementsLayer(ElementLayer.Bottom);
                    break;
                case "上移一层":
                    this.VectorControl.UpdateElementsLayer(ElementLayer.Up);
                    break;
                case "下移一层":
                    this.VectorControl.UpdateElementsLayer(ElementLayer.Down);
                    break;
                case "删除":
                    this.VectorControl.Delete();
                    break;
                case "复制":
                    this.VectorControl.Copy();
                    break;
                case "粘贴":
                    this.VectorControl.Paste();
                    break;
                case "剪切":
                    this.VectorControl.Cut();
                    break;
                case "全选":
                    this.VectorControl.SelectAll();
                    break;
                case "取消选择":
                    this.VectorControl.SelectNone();
                    break;
                case "组合图元":
                    this.VectorControl.Group();
                    break;
                case "拆分图元":
                    this.VectorControl.UnGroup();
                    break;
                case "显示网格":
                    grid.Visible = !grid.Visible;
                    this.VectorControl.Grid = grid;
                    break;
                case "吸附到网格":
                    if ((this.VectorControl.VisualAlignment & VisualAlignment.Grid) == VisualAlignment.Grid)
                        this.VectorControl.VisualAlignment = (this.VectorControl.VisualAlignment ^ VisualAlignment.Grid);
                    else
                        this.VectorControl.VisualAlignment = (this.VectorControl.VisualAlignment | VisualAlignment.Grid);
                    break;
                case "吸附到对象":
                    if ((this.VectorControl.VisualAlignment & VisualAlignment.Element) == VisualAlignment.Element)
                        this.VectorControl.VisualAlignment = (this.VectorControl.VisualAlignment ^ VisualAlignment.Element);
                    else
                        this.VectorControl.VisualAlignment = (this.VectorControl.VisualAlignment | VisualAlignment.Element);
                    break;
                case "设置网格":
                    GridSetupDialog gridsetup = new GridSetupDialog();
                    gridsetup.Grid = this.VectorControl.Grid;
                    if (gridsetup.ShowDialog(owner) == DialogResult.OK)
                        this.VectorControl.Grid = gridsetup.Grid;
                    gridsetup = null;
                    break;
                case "显示标尺":
                    this.VectorControl.ShowRule = !this.VectorControl.ShowRule;
                    break;
                case "显示参考线":
                    guide.Visible = !guide.Visible;
                    this.VectorControl.Guide = guide;
                    break;
                case "吸附到参考线":
                    if ((this.VectorControl.VisualAlignment & VisualAlignment.Guide) == VisualAlignment.Guide)
                        this.VectorControl.VisualAlignment = (this.VectorControl.VisualAlignment ^ VisualAlignment.Guide);
                    else
                        this.VectorControl.VisualAlignment = (this.VectorControl.VisualAlignment | VisualAlignment.Guide);
                    break;
                case "锁定参考线":
                    guide.Lock = !guide.Lock;
                    this.VectorControl.Guide = guide;
                    break;
                case "设置参考线":
                    GuideSetupDialog guideset = new GuideSetupDialog();
                    guideset.Guide = this.VectorControl.Guide;
                    if (guideset.ShowDialog(owner) == DialogResult.OK)
                        this.VectorControl.Guide = guideset.Guide;
                    guideset = null;
                    break;
                case "清除参考线":
                    this.VectorControl.ClearGuides();
                    break;
                case "高度相同":
                    this.VectorControl.AdjustElementsSize(ElementSizeAdjustment.Height);
                    break;
                case "宽度相同":
                    this.VectorControl.AdjustElementsSize(ElementSizeAdjustment.Width);
                    break;
                case "尺寸相同":
                    this.VectorControl.AdjustElementsSize(ElementSizeAdjustment.All);
                    break;
                case "向左旋转":
                    using (System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix())
                    {
                        matrix.Rotate(-90);
                        this.VectorControl.TransformSelection(matrix);
                    }
                    break;
                case "向右旋转":
                    using (System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix())
                    {
                        matrix.Rotate(90);
                        this.VectorControl.TransformSelection(matrix);
                    }
                    break;
                case "水平镜像旋转":
                    using (System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix(-1, 0, 0, 1, 0, 0))
                    {
                        this.VectorControl.TransformSelection(matrix);
                    }
                    break;
                case "垂直镜像旋转":
                    using (System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, 0))
                    {
                        this.VectorControl.TransformSelection(matrix);
                    }
                    break;
                case "高质量显示":
                    this.VectorControl.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    break;
                case "高速显示":
                    this.VectorControl.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    break;
                case "高质量显示文本":
                    if (this.VectorControl.TextRenderingHint == System.Drawing.Text.TextRenderingHint.AntiAlias)
                        this.VectorControl.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                    else
                        this.VectorControl.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    break;
                case "显示连接点":
                    if (this.VectorControl != null)
                        this.VectorControl.ShowConnectablePoint = !this.VectorControl.ShowConnectablePoint;
                    break;
                case "修改文档尺寸":
                    //if (this.Canvas != null)
                    //{
                    //    Size size = this.Canvas.DocumentSize;
                    //    Dialog.SizeDialog dlg = new YP.Canvas.Demo.Dialog.SizeDialog();
                    //    dlg.DocumentSize = size;
                    //    if (dlg.ShowDialog(owner) == DialogResult.OK)
                    //        this.Canvas.DocumentSize = dlg.DocumentSize;
                    //}
                    break;
                case "保存":
                    this.documentControl.SaveFile(false);
                    break;
                case "另存为":
                    this.documentControl.SaveFile(true);
                    break;
                case "导出为图片":
                    this.VectorControl.ExportImage();
                    break;
                case "打印":
                    this.VectorControl.Print();
                    break;
                case "导出标准svg":
                    if (this.exportdlg.ShowDialog(owner) == DialogResult.OK)
                    {
                        System.IO.Stream stream = null;
                        try
                        {
                            string filename = this.exportdlg.FileName;
                            stream = System.IO.File.Open(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                            System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8);
                            writer.Write(this.VectorControl.ExportSVG(true));
                            writer.Close();
                            stream.Close();
                        }
                        catch (System.Exception e1)
                        {
                            MessageBox.Show(e1.Message);
                        }
                        finally
                        {
                            //记住关闭流
                            if (stream != null)
                                stream.Close();
                        }
                    }
                    break;
                case "showhighlightoutline":
                    this.VectorControl.ShowSelectionHighlightOutline = !this.VectorControl.ShowSelectionHighlightOutline;
                    break;
                case "显示轮廓线":
                    this.VectorControl.OutLine = !this.VectorControl.OutLine;
                    break;
                //设置编辑环境允许进行的二维变换操作
                case "保护":
                    Dialog.ProtectDialog pdlg = new Dialog.ProtectDialog();
                    pdlg.TransformBehavior = this.VectorControl.TransformBehavior;
                    if (pdlg.ShowDialog(owner) == DialogResult.OK)
                        this.VectorControl.TransformBehavior = pdlg.TransformBehavior;
                    break;
                case "显示操作手柄":
                    this.VectorControl.ShowResizeGrap = !this.VectorControl.ShowResizeGrap;
                    break;
                case "显示旋转中心点":
                    this.VectorControl.ShowCenterPointGrap = !this.VectorControl.ShowCenterPointGrap;
                    break;
                case "显示选择边框":
                    this.VectorControl.ShowSelectedBounds = !this.VectorControl.ShowSelectedBounds;
                    break;
                case "禁止删除":
                    if ((this.VectorControl.ProtectType & ProtectType.Delete) == ProtectType.Delete)
                        this.VectorControl.ProtectType = ProtectType.None;
                    else
                        this.VectorControl.ProtectType = ProtectType.Delete;
                    break;
                case "锁住图元":
                case "解锁图元":
                    bool isLock = this.VectorControl.HasLockedElementsInSelection;
                    foreach (SVGElement element in selection)
                    {
                        if (element is SVG.SVGTransformableElement)
                            (element as SVG.SVGTransformableElement).IsLock = !isLock;
                    }
                    break;
                case "缩放到选区":
                    this.VectorControl.BringElementsIntoView(selection.ToArray());
                    break;
                case "使画布适应窗口":
                    if (this.VectorControl != null)
                        this.VectorControl.FitCanvasToWindow();
                    break;
                case "滚动到画布正中":
                    if (this.VectorControl != null)
                        this.VectorControl.ScrollToCenter();
                    break;
                case "自动连接到连接点":
                    documentControl.AllowToConnectToAnchor = !documentControl.AllowToConnectToAnchor;
                    break;
                case "自动连接到形状":
                    documentControl.AllowToConnectToShape = !documentControl.AllowToConnectToShape;
                    break;
                case "property":
                    {
                        var selection1 = this.VectorControl.Selection;
                        if (selection1.Count == 1 && selection1[0] is SVGTransformableElement)
                        {
                            SymbolDesigner.Dialog.ConnectPointPropertyDialog dlg = new Dialog.ConnectPointPropertyDialog();
                            dlg.Element = selection1[0] as SVGTransformableElement;
                            dlg.ShowDialog(this.VectorControl);
                        }
                    }
                    break;
                case "up":
                    this.VectorControl.CurrentScene = SubSystemDocumentHelper.FindParentSystem(SubSystemDocumentHelper.FindSubSystemViewElement(this.VectorControl.CurrentScene));
                    break;
                case "help":
                    //{
                    //    var symbols = DocumentControl.GetAllSymbolReference(this.Canvas.Document);
                    //    foreach (SVGTransformableElement elm in symbols)
                    //    {
                    //        if (!DocumentControl.IsSymbolConnected(elm))
                    //            elm.Selected = true;
                    //    }
                    //}
                    //System.Diagnostics.Process.Start("vectorcontrolsdk.chm");
                    break;
                case "高亮非连通对象":
                    {
                        var symbols = DocumentControl.GetAllSymbolReference(this.VectorControl.Document);
                        foreach (SVGTransformableElement elm in symbols)
                        {
                            if (!DocumentControl.IsSymbolConnected(elm))
                                elm.Selected = true;
                        }
                    }
                    break;
            }
        }
        #endregion

        #region ..UpdateCommandStatus
        public void UpdateCommandStatus(object menu)
        {
            //在显示菜单之前，调用控件的ExecuteBehaviorPresent方法确定菜单所对应的操作当前是否可用，从而更新菜单有效状态
            System.Windows.Forms.ToolStripItemCollection list = null;
            if (menu is ToolStripMenuItem)
                list = (menu as ToolStripMenuItem).DropDownItems;
            else if (menu is ContextMenuStrip)
                list = (menu as ContextMenuStrip).Items;
            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                Behavior be = Behavior.None;
                ToolStripMenuItem item1 = list[i] as ToolStripMenuItem;
                //if (this.documentControl == null && list[i].Tag != null)
                //{
                //    list[i].Visible = false;
                //    continue;
                //}
                //else
                //    list[i].Visible = true;
                if (item1 == null)
                    continue;
                try
                {
                    item1.Enabled = item1.DropDown != null;
                    switch (NormalizeItemText(item1))
                    {
                        case "撤消":
                            be = Behavior.Undo;
                            break;
                        case "重做":
                            be = Behavior.Redo;
                            break;
                        case "复制":
                            be = Behavior.Copy;
                            break;
                        case "粘贴":
                            be = Behavior.Paste;
                            break;
                        case "剪切":
                            be = Behavior.Cut;
                            break;
                        case "删除":
                            be = Behavior.Delete;
                            break;
                        case "全选":
                            be = Behavior.SelectAll;
                            break;
                        case "取消选择":
                            be = Behavior.SelectNone;
                            break;
                        case "组合图元":
                            be = Behavior.Group;
                            break;
                        case "拆分图元":
                            be = Behavior.UnGroup;
                            break;
                        case "分布":
                            be = Behavior.Distriute;
                            break;
                        case "对齐选中对象":
                            be = Behavior.AlignElements;
                            break;
                        case "尺寸":
                            be = Behavior.AdjustSize;
                            break;
                        case "位置":
                            be = Behavior.AdjustLayer;
                            break;
                        case "旋转":
                            be = Behavior.Transform;
                            break;
                        case "显示标尺":
                            item1.Checked = this.VectorControl.ShowRule;
                            break;
                        case "高质量显示":
                            item1.Checked = this.VectorControl.SmoothingMode == System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            break;
                        case "高速显示":
                            item1.Checked = this.VectorControl.SmoothingMode != System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            break;
                        case "高质量显示文本":
                            item1.Checked = this.VectorControl.TextRenderingHint == System.Drawing.Text.TextRenderingHint.AntiAlias;
                            break;
                        case "显示连接点":
                            item1.Checked = this.VectorControl.ShowConnectablePoint;
                            break;
                        case "显示参考线":
                            item1.Checked = this.VectorControl.Guide.Visible;
                            break;
                        case "吸附到参考线":
                            item1.Checked = (this.VectorControl.VisualAlignment & VisualAlignment.Guide) == VisualAlignment.Guide;
                            break;
                        case "锁定参考线":
                            item1.Checked = this.VectorControl.Guide.Lock;
                            break;
                        case "吸附到对象":
                            item1.Checked = (this.VectorControl.VisualAlignment & VisualAlignment.Element) == VisualAlignment.Element;
                            break;
                        case "显示网格":
                            item1.Checked = this.VectorControl.Grid.Visible;
                            break;
                        case "吸附到网格":
                            item1.Checked = (this.VectorControl.VisualAlignment & VisualAlignment.Grid) == VisualAlignment.Grid;
                            break;
                        case "显示轮廓线":
                            item1.Checked = this.VectorControl.OutLine;
                            break;
                        case "showhighlightoutline":
                            item1.Checked = this.VectorControl.ShowSelectionHighlightOutline;
                            break;
                        case "显示操作手柄":
                            item1.Checked = this.VectorControl.ShowResizeGrap;
                            break;
                        case "显示旋转中心点":
                            item1.Checked = this.VectorControl.ShowCenterPointGrap;
                            break;
                        case "显示选择边框":
                            item1.Checked = this.VectorControl.ShowSelectedBounds;
                            break;
                        case "禁止删除":
                            item1.Checked = (this.VectorControl.ProtectType & ProtectType.Delete) == ProtectType.Delete;
                            break;
                        case "自动连接到连接点":
                            item1.Checked = documentControl.AllowToConnectToAnchor;
                            break;
                        case "自动连接到形状":
                            item1.Checked = documentControl.AllowToConnectToShape;
                            break;
                        case "property":
                            item1.Visible = documentControl != null;
                            item1.Enabled = documentControl.Canvas.Selection.Count == 1 && documentControl.Canvas.Selection[0] is SVGTransformableElement;
                            break;
                        case "up":
                            item1.Enabled = documentControl.Canvas.CurrentScene != documentControl.Canvas.Document.RootElement;
                            break;
                        case "锁住图元":
                        case "解锁图元":
                            if (this.VectorControl.HasLockedElementsInSelection)
                                item1.Text = "解锁图元";
                            else
                                item1.Text = "锁住图元";
                            break;
                    }
                    item1.Visible = true;
                    if (be != Behavior.None && this.VectorControl != null)
                        item1.Enabled = this.VectorControl.ExecuteBehaviorPresent(be);
                    if (be != Behavior.None && this.VectorControl == null)
                        item1.Visible = false;
                }
                catch
                {
                    item1.Visible =false;
                }
            }
        }
        #endregion

        #region ..NormalizeItemText
        /// <summary>
        /// 获取ToolStripItem的文本，去掉热键
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static string NormalizeItemText(ToolStripItem item)
        {
            if (item == null)
                return null;
            string text = item.Tag as string;
            if(text == null || text.ToLower() == "needdoc")
                text = item.Text;
            int index = text.IndexOf("(");
            text = index > 0 ? text.Substring(0, index) : text;
            return text.ToLower();
        }
        #endregion
    }
}
