using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 列表形式的颜色选择器
	/// </summary>
	[ToolboxBitmap(typeof(ListBox))]
	[ToolboxItem(false)]
	public class ListColors:System.Windows.Forms.ListBox,IColorSelector
	{
		#region ..构造及消除
		public ListColors()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            this.ItemHeight = 20;
			this.helper = new ColorSelectorHelper(this);
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.items.Add(Color.Empty);
			base.Items.Add(Color.Empty);
			this.items.Add(Color.White);
			base.Items.Add(Color.White);
			for(int i = ColorSelectorHelper.ColorIndex;i<ColorSelectorHelper.ColorCount;i++)
			{
				this.Items.Add(Color.FromKnownColor((KnownColor)i));
				base.Items.Add(Color.FromKnownColor((KnownColor)i));
			}
			this.items.Add(Color.Transparent);
			base.Items.Add(Color.Transparent);
			this.DrawItem += new DrawItemEventHandler(helper.DrawItem);
			this.BorderStyle = BorderStyle.None;
		}
		#endregion

		#region ..私有变量
		ColorCollecton items = new ColorCollecton();
		ColorSelectorHelper helper = null;
        static ColorDialog color = new ColorDialog();
		Color customColor = Color.White;
		bool createevent = true;
		int oldindex = -1;
		#endregion

		#region ..DrawMode
		public new DrawMode DrawMode
		{
			set
			{
				base.DrawMode = DrawMode.OwnerDrawFixed;
			}
			get
			{
				return DrawMode.OwnerDrawFixed;
			}
		}
		#endregion

		#region ..Items
		public new ColorCollecton Items
		{
			get
			{
				return this.items;
			}
		}
		#endregion

		#region ..SelectedColor
		public override int SelectedIndex
		{
			get
			{
				return base.SelectedIndex;
			}
			set
			{
				if(value == this.items.Count - 1)
				{
					if(createevent)
						base.SelectedIndex = -1;
					else
						base.SelectedIndex = value;
				}
				else
					base.SelectedIndex = value;
			}
		}

		/// <summary>
		/// 获取或设置选定的颜色
		/// </summary>
		public Color SelectedColor
		{
			set
			{
				int index = this.items.IndexOf(value);
				if(index>=0)
					this.SelectedIndex = this.items.IndexOf(value);
				else
				{
					createevent = false;
					this.items[this.items.Count - 1] = value;
					this.SelectedIndex = this.items.Count - 1;
					createevent=true;
				}
			}
			get
			{
				if(this.SelectedIndex >= 0 )
					return this.items[this.SelectedIndex];
				return Color.Empty;
			}
		}
		#endregion

		#region ..OnSelectedIndexChanged
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			if(this.SelectedIndex == this.items.Count - 1 && createevent)
			{
				
				if(color.ShowDialog(this) == DialogResult.OK)
				{
					this.items[this.items.Count - 1] = color.Color;
					base.OnSelectedIndexChanged (e);
					this.Invalidate();
				}
				else
					this.SelectedIndex = this.oldindex;
			}
			else
				base.OnSelectedIndexChanged (e);
			this.oldindex = this.SelectedIndex;
		}
		#endregion
	}
}
