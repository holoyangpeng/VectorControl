using System;
using System.Windows.Forms;
using System.Drawing;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 提供一个列表，选择默认的线条粗细
	/// </summary>
	public class ListStrokeWidth:System.Windows.Forms.ListBox
	{
		#region ..构造及消除
		public ListStrokeWidth()
		{
			this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 20;
			base.Items.Add(1f);
			base.Items.Add(2f);
			//base.Items.Add(2.5f);
			base.Items.Add(3f);
			//base.Items.Add(3.5f);
			base.Items.Add(4f);
			//base.Items.Add(4.5f);
			base.Items.Add(5f);
			base.Items.Add(6f);
            base.Items.Add(7f);
            base.Items.Add(8f);
            base.Items.Add(9f);
            base.Items.Add(10f);
            base.Items.Add(11f);
			this.BorderStyle = BorderStyle.None;
            this.Height = this.Items.Count * ItemHeight;
		}
		#endregion

		#region ..常量
		private const int RECTCOLOR_LEFT = 4;
		private const int RECTCOLOR_TOP = 2;
		private const int RECTCOLOR_Width = 20;
		private const int RECTCOLOR_Margin = 4;
		#endregion

        #region ..OnDrawItem
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            int index = e.Index;
            if (e.Index == -1)
                index = (this.SelectedIndex >= 0 ? this.SelectedIndex : 0);
            if (index >= 0 || index < this.Items.Count)
            {
                if (e.State == DrawItemState.Selected || e.State == DrawItemState.None)
                    e.DrawBackground();
                int left = RECTCOLOR_LEFT;
                int top = RECTCOLOR_TOP;
                int width = RECTCOLOR_Width;
                int margin = RECTCOLOR_Margin;
                Rectangle rect = new Rectangle(left, e.Bounds.Top + top, e.Bounds.Width - 2 * left, e.Bounds.Height - 2 * top);
                Color fore = e.ForeColor;
                if ((int)e.State == 4113)
                    fore = Color.Black;
                using (System.Drawing.StringFormat sf = new StringFormat(System.Drawing.StringFormat.GenericTypographic))
                {
                    sf.LineAlignment = StringAlignment.Center;
                    if (this.Items[index] is float && (float)this.Items[index] > 0)
                    {
                        float a = (float)this.Items[index];
                        e.Graphics.DrawString(a.ToString(), e.Font, new SolidBrush(fore), new Rectangle(left, e.Bounds.Top + top, width, e.Bounds.Height - 2 * top), sf);
                        float top1 = (e.Bounds.Height - top - a) / 2 + 1;
                        e.Graphics.FillRectangle(new SolidBrush(fore), left + width + margin, e.Bounds.Top + top1, e.Bounds.Width - width - left - 2 * margin, a);
                    }
                }
            }
            base.OnDrawItem(e);
        }
        #endregion
	}
}
