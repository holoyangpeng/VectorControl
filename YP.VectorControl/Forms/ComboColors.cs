using System;
using System.Windows.Forms;
using System.Drawing;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 提供一个下拉框，选取颜色
	/// </summary>
	internal class ComboColors:System.Windows.Forms.ComboBox,IColorSelector
	{
		#region ..私有变量
		ColorCollecton items = new ColorCollecton();
		ColorSelectorHelper helper = null;
		Color customColor = Color.White;
		bool createevent = true;
        ColorDialog color = new ColorDialog();
		#endregion

		#region ..构造及消除
		public ComboColors()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.helper = new ColorSelectorHelper(this);
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			for(int i = ColorSelectorHelper.ColorIndex;i<ColorSelectorHelper.ColorCount;i++)
			{
				this.Items.Add(Color.FromKnownColor((KnownColor)i));
				base.Items.Add(Color.FromKnownColor((KnownColor)i));
			}
			this.items.Add(Color.White);
			base.Items.Add(Color.White);
			this.DrawItem += new DrawItemEventHandler(helper.DrawItem);
		}
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
		/// <summary>
		/// 获取或设置选定的颜色
		/// </summary>
		public Color SelectedColor
		{
			set
			{
				int index = this.items.IndexOf(value);
				if(index>=0 && index < this.items.Count - 1)
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

		#region ..OnDropDownStyleChanged
		protected override void OnDropDownStyleChanged(EventArgs e)
		{
			if(this.DropDownStyle != ComboBoxStyle.DropDownList)
				this.DropDownStyle = ComboBoxStyle.DropDownList;
		}
		#endregion

		#region ..OnDropDown
		int oldindex = 0;
		protected override void OnDropDown(EventArgs e)
		{
			this.oldindex = this.SelectedIndex;
			base.OnDropDown (e);
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
				}
				else
				{
					this.SelectedIndex = this.oldindex;
				}
			}
			else
				base.OnSelectedIndexChanged (e);
		}
		#endregion
	}
}
