using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define the menu item so that it can support the theme and execute some commands
    /// </summary>
    public class MenuCommand : ToolStripMenuItem, IItemsContainer, IUpdateable
    {
        #region ..static fields
        static ImageList _menuImages;
        static MenuCommand()
        {
            _menuImages = Common.ResourceHelper.LoadBitmapStrip(Type.GetType("YP.CommonControl.Menu.PopupMenu"),
                    "YP.CommonControl.Resources.ImagesPopupMenu.bmp", new Size(16, 16), new Point(0, 0));
        }
        #endregion

        #region ..Constructor
        /// <summary>
        /// create an instance
        /// </summary>
        public MenuCommand():base()
        {
            this.AttachDropDownEvent();
            this.DropDown.DropShadowEnabled = false;
        }
        #endregion

        #region ..DropDownEvent
        void AttachDropDownEvent()
        {
            this.DropDown.Paint += new PaintEventHandler(DropDown_Paint);
            this.DropDown.Renderer.RenderToolStripBorder += new ToolStripRenderEventHandler(Renderer_RenderToolStripBorder);
        }

        void Renderer_RenderToolStripBorder(object sender, ToolStripRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(e.ConnectedArea.Location, new Size(leftMargin, e.ConnectedArea.Height));

            //draw the connect area
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, this.BackColor)))
            {
                e.Graphics.FillRectangle(Brushes.White, e.ConnectedArea);
                e.Graphics.FillRectangle(brush, e.ConnectedArea);
            }
        }

        void DropDown_Paint(object sender, PaintEventArgs e)
        {
            Color color = this.BackColor;
            Rectangle rect = new Rectangle(e.ClipRectangle.Location, new Size(leftMargin, e.ClipRectangle.Height));
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, this.BackColor)))
            {
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, color, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(brush, rect);
        }
        #endregion

        #region ..const fields
        internal const int leftMargin = 26;
        internal const int rightMargin = 20;
        internal const int arrowLength = 5;
        internal const int checkWidth = 19;
        #endregion

        #region ..public properties
        /// <summary>
        /// gets the sub items collection
        /// </summary>
        public ToolStripItemCollection Items
        {
            get
            {
                return this.DropDownItems;
            }
        }

        /// <summary>
        /// gets the parent items which store this
        /// </summary>
        public ToolStripItemCollection ParentItems
        {
            get
            {
                if (this.OwnerItem is ToolStripMenuItem)
                    return (this.OwnerItem as ToolStripMenuItem).DropDownItems;
                if (this.Parent != null)
                    return Parent.Items;
                return null;
            }
        }
        #endregion

        #region ..events
        /// <summary>
        /// occurs when the menu item show
        /// </summary>
        public event EventHandler Update;
        #endregion

        #region ..OnUpdate
        protected virtual void OnUpdate()
        {
            if (this.Update != null)
                this.Update(this, EventArgs.Empty);
        }
        #endregion

        #region ..OnDropDownShow
        protected override void OnDropDownShow(EventArgs e)
        {
            base.OnDropDownShow(e);
            foreach (ToolStripItem item in this.Items)
            {
                if (item is IUpdateable)
                    (item as IUpdateable).InvokeUpdate();
            }
        }
        #endregion

        #region ..InvokeUpdate
        /// <summary>
        /// invoke the update method
        /// </summary>
        public void InvokeUpdate()
        {
            this.OnUpdate();
        }
        #endregion

        #region ..OnPaint
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                base.OnPaint(e);
                return;
            }
            this.DropDown.BackColor = this.BackColor;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
            {
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.DisplayFormatControl;
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;

                Rectangle rect = e.ClipRectangle;

                Brush textBrush = SystemBrushes.MenuText;
                if (!this.Enabled)
                    textBrush = SystemBrushes.InactiveCaption;

                #region ..Draw the border when highlight or press
                bool hot = this.Selected;
                if (this.OwnerItem != null || !(this.Parent is MenuStrip))
                    rect = new Rectangle(rect.X + 2, rect.Y, rect.Width - 3, rect.Height);
                else
                    hot = hot || (this.DropDown != null && this.DropDown.Visible);
                hot = hot && this.Enabled;
                if (hot)
                {
                    Color color = this.BackColor;
                    Color color1 = color;
                    bool drawhighlight = this.OwnerItem != null || this.DropDown == null || !this.DropDown.Visible || (this.DropDown.Visible && this.Parent is PopupMenu);
                    if (!this.Pressed || drawhighlight)
                        color1 = Color.FromArgb(120, SystemColors.Highlight);

                    //paint the back ground
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, color1, LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(Brushes.White, rect);
                        e.Graphics.FillRectangle(brush, rect);
                    }

                    color = Color.FromArgb(color.R, color.G, color.B);
                    using (Pen pen = new Pen(ControlPaint.Dark(color)))
                    {
                        if (drawhighlight)
                            e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                        else
                        {
                            e.Graphics.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom);
                            e.Graphics.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                            e.Graphics.DrawLine(pen, rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom);
                        }
                    }
                }
                #endregion

                #region ..Draw menu content
                //if the item is the top item
                if (this.OwnerItem == null && this.Parent is MenuStrip)
                {
                    if (this.DropDown != null)
                        this.DropDown.BackColor = this.BackColor;
                    sf.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(this.Text, this.Font, SystemBrushes.MenuText, this.ContentRectangle, sf);
                }
                else
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    rect = new Rectangle(e.ClipRectangle.Location, new Size(leftMargin, e.ClipRectangle.Height));
                    //if checked 
                    Image img = this.Image;
                    if (this.Checked)
                    {
                        Rectangle rect1 = new Rectangle((rect.Width - checkWidth) / 2, (rect.Height - checkWidth) / 2, checkWidth, checkWidth);
                        using (Brush brush = new LinearGradientBrush(rect1,Color.White,Color.FromArgb(60, SystemColors.Highlight), LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(Brushes.White, rect1);
                            e.Graphics.FillRectangle(brush, rect1);
                        }
                        e.Graphics.DrawRectangle(SystemPens.Highlight, rect1);
                        img = _menuImages.Images[0];
                    }
                    //draw the image
                    if (img != null)
                    {
                        rect = new Rectangle((rect.Width - img.Width) / 2, (rect.Height - img.Height) / 2, img.Width, img.Height);
                        if (this.Enabled)
                            e.Graphics.DrawImageUnscaled(img, rect);
                        else
                            ControlPaint.DrawImageDisabled(e.Graphics, img, rect.X, rect.Y, Color.Transparent);
                    }

                    //draw menu text
                    GraphicsContainer c = e.Graphics.BeginContainer();
                    e.Graphics.TranslateTransform(leftMargin + 8, 0);
                    e.Graphics.DrawString(this.Text, this.Font, textBrush, e.ClipRectangle, sf);
                    e.Graphics.EndContainer(c);

                    //draw short cut
                    if (this.ShowShortcutKeys && this.ShortcutKeys != Keys.None)
                    {
                        string text = ShortCutHelper.GetShortCutText(this.ShortcutKeys);
                        if (text != null && text.Length > 0)
                        {
                            sf.Alignment = StringAlignment.Far;
                            rect = new Rectangle(e.ClipRectangle.Location, new Size(e.ClipRectangle.Width - rightMargin, e.ClipRectangle.Height));
                            e.Graphics.DrawString(text, this.Font, textBrush, rect, sf);
                        }
                    }

                    //draw the arrow
                    if (this.DropDownItems.Count > 0)
                    {
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            int center = e.ClipRectangle.Right - rightMargin / 2;
                            int middle = e.ClipRectangle.Y + e.ClipRectangle.Height / 2;
                            float width = (float)(arrowLength * Math.Sin(Math.PI / 3));
                            float height = (float)(arrowLength * Math.Cos(Math.PI / 3));
                            PointF[] ps = new PointF[] { new PointF(center - width / 2f, middle - height - 1), new PointF(center - width / 2f, middle + height + 1), new PointF(center + width / 2f, middle) };
                            e.Graphics.FillPolygon(Brushes.Black, ps);
                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region ..OnBackColorChanged
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            foreach (ToolStripItem item in this.DropDownItems)
                item.BackColor = this.BackColor;
        }
        #endregion
    }
}