using System;
using System.Windows.Forms;
using System.Drawing;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 提供一个列表，选择当前系统安装的所有字体
	/// </summary>
	public class ListFontName:ListBox
	{
		#region ..构造及消除
		public ListFontName()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.ItemHeight = 20;
			this.BorderStyle = BorderStyle.None;
			FontFamily[] families = System.Drawing.FontFamily.Families ;
			for(int j = 0;j<families.Length;j++)
			{
				this.Items.Add(families[j].Name);
			}

		}
		#endregion
	}
}
