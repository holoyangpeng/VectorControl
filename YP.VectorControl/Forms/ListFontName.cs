using System;
using System.Windows.Forms;
using System.Drawing;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// �ṩһ���б�ѡ��ǰϵͳ��װ����������
	/// </summary>
	public class ListFontName:ListBox
	{
		#region ..���켰����
		public ListFontName()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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
