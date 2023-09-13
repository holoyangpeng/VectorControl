using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	#region ..ColorCollecton
	/// <summary>
	/// 定义了颜色的集合操作类型
	/// </summary>
	public sealed class ColorCollecton:System.Collections.CollectionBase
	{
		#region ..属性
		/// <summary>
		/// 获取或设置索引处的颜色
		/// </summary>
		internal Color this[int index]
		{
			set
			{
				this.List[index] = value;
			}
			get
			{
				return (Color)this.List[index];
			}
		}
		#endregion

		#region ..集合操作
		/// <summary>
		/// 添加颜色
		/// </summary>
		/// <param name="color"></param>
		internal void Add(Color color)
		{
			if(!this.List.Contains(color))
				this.List.Add(color);
		}

		/// <summary>
		/// 在指定索引处插入颜色
		/// </summary>
		/// <param name="index"></param>
		/// <param name="color"></param>
		internal void Insert(int index,Color color)
		{
			index = (int)Math.Max(0,index);
			if(!this.List.Contains(color))
			{
				if(index >= this.List.Count)
					this.List.Add(color);
				else
					this.List.Insert(index,color);
			}
		}

		/// <summary>
		/// 丛集合中删除项
		/// </summary>
		/// <param name="color"></param>
		internal void Remove(Color color)
		{
			if(this.List.Contains(color))
				this.List.Remove(color);
		}
		#endregion

		#region ..Contains
		/// <summary>
		/// 判断集合是否包含颜色
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public bool Contains(Color color)
		{
			return this.List.Contains(color);
		}

		/// <summary>
		/// 获取颜色在集合中的索引
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public int IndexOf(Color color)
		{
			return this.List.IndexOf(color);
		}
		#endregion

		#region ..插入文本
		internal void AddText(string text)
		{
			if(!this.List.Contains(text))
				this.List.Add(text);
		}
		#endregion
	}
	#endregion

	#region ..ColorSelectorHelper
	/// <summary>
	/// ColorSelectorHelper 的摘要说明。
	/// </summary>
	internal class ColorSelectorHelper
	{
		internal static int ColorIndex = 28;
		internal static int ColorCount = 140;
		internal readonly static string NoneColorStr = "无";
		internal readonly static string CustomColorStr = "自定义...";
		internal readonly static int ItemHeight = 18;

		#region ..构造及消除
		public ColorSelectorHelper(IColorSelector listcontrol)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.lstControl = listcontrol;
			this.lstControl.ItemHeight = ItemHeight;
		}
		#endregion

		#region ..私有变量
		IColorSelector lstControl = null;
		#endregion

		#region ..常量
		private const int RECTCOLOR_LEFT = 4;
		private const int RECTCOLOR_TOP = 2;
		private const int RECTCOLOR_WIDTH = 30;
		private const int RECTTEXT_MARGIN = 10;
		private const int RECTTEXT_LEFT = RECTCOLOR_LEFT + RECTCOLOR_WIDTH + RECTTEXT_MARGIN;
		#endregion

		#region ..DrawItem
		internal void DrawItem(object sender,DrawItemEventArgs e)
		{
			Graphics Grphcs = e.Graphics;
			Color BlockColor = Color.Empty;
			int left = RECTCOLOR_LEFT;
			if(e.State == DrawItemState.Selected || e.State == DrawItemState.None) 
				e.DrawBackground();
			if(e.Index == -1) 
				BlockColor = lstControl.SelectedIndex < 0 ? (Color)this.lstControl.Items[0] : SystemColors.Highlight;
			else if(e.Index < lstControl.Items.Count)
				BlockColor = (Color)lstControl.Items[e.Index];
			Grphcs.FillRectangle(new SolidBrush(BlockColor),left,e.Bounds.Top+RECTCOLOR_TOP,RECTCOLOR_WIDTH,e.Bounds.Height - 2 * RECTCOLOR_TOP);
			using(System.Drawing.StringFormat sf = new StringFormat(System.Drawing.StringFormat.GenericTypographic))
			{
				sf.LineAlignment = StringAlignment.Far;
				using(Brush brush = new SolidBrush(e.ForeColor))
				{
					if(BlockColor.IsEmpty)
					{
						sf.Alignment = StringAlignment.Center;
						Rectangle rect = new Rectangle(left,e.Bounds.Top+1,e.Bounds.Width-2 * left,e.Bounds.Height - 2);
						Grphcs.DrawRectangle(Pens.Black,rect);
						Grphcs.DrawString(NoneColorStr,e.Font,brush,rect,sf);
					}
					else
					{
						Grphcs.DrawRectangle(Pens.Black,left,e.Bounds.Top+RECTCOLOR_TOP,RECTCOLOR_WIDTH,e.Bounds.Height - 2 * RECTCOLOR_TOP);
						Grphcs.DrawString(e.Index == lstControl.Items.Count - 1?CustomColorStr:BlockColor.Name,e.Font,brush,new Rectangle(RECTTEXT_LEFT,e.Bounds.Top,e.Bounds.Width-RECTTEXT_LEFT,e.Bounds.Height),sf);
					}
				}
			}
		}
		#endregion
	}
	#endregion
}
