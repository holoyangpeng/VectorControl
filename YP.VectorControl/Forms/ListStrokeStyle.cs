using System;
using System.Windows.Forms;
using System.Drawing;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// �ṩһ���б�ѡ��VectorControlĬ�ϵ�������ʽ
	/// </summary>
	public class ListStrokeStyle:ListBox
	{
		#region ..DashArray
		struct DashArray
		{
			float[] array;
			public DashArray(float[] a)
			{
				this.array = a;
			}

			public override string ToString()
			{
				if(this.array == null || this.array.Length == 0)
					return "none";
				else
				{
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					for(int i = 0;i<this.array.Length;i++)
					{
						sb.Append(this.array[i].ToString());
						sb.Append(" ");
					}
					return sb.ToString(0,sb.Length - 1);
				}
			}

			public float[] Array
			{
				get
				{
					return this.array;
				}
			}
		}
		#endregion

		#region ..���켰����
		public ListStrokeStyle()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 20;
			base.Items.Add(new DashArray(null));
			base.Items.Add(new DashArray(new float[]{1,1}));
			base.Items.Add(new DashArray(new float[]{2,2}));
			base.Items.Add(new DashArray(new float[]{3,3}));
			base.Items.Add(new DashArray(new float[]{4,4}));
			base.Items.Add(new DashArray(new float[]{2,1,1,1}));
			base.Items.Add(new DashArray(new float[]{3,1,1,1}));
			base.Items.Add(new DashArray(new float[]{4,1,1,1}));
			base.Items.Add(new DashArray(new float[]{2,1,1,1,1,1}));
			base.Items.Add(new DashArray(new float[]{3,1,1,1,1,1}));
			base.Items.Add(new DashArray(new float[]{4,1,1,1,1,1}));
			this.BorderStyle = BorderStyle.None;
            this.Height = this.Items.Count * ItemHeight;
		}
		#endregion

        #region ..SelectedStyle
        /// <summary>
        /// ��ȡ��ǰѡ�е���ʽ
        /// </summary>
        public float[] SelectedStyle
        {
            get
            {
                if (this.SelectedItem is DashArray)
                    return ((DashArray)this.SelectedItem).Array;
                return null;
            }
            set
            {
                if (value == null)
                    this.SelectedIndex = 0;
                else
                {
                    for (int i = 1; i < this.Items.Count; i++)
                        if (this.Items[i] is DashArray && ((DashArray)this.Items[i]).Array.ToString() == value.ToString())
                        {
                            this.SelectedIndex = i;
                            break;
                        }
                }
            }
        }
        #endregion

        #region ..����
        //��߾�
		private const int RECTCOLOR_LEFT = 4;
		//�ϱ߾�
		private const int RECTCOLOR_TOP = 2;
		private const int RECTCOLOR_Width = 20;
		private const int RECTCOLOR_Margin = 4;
		#endregion

		#region ..OnDrawItem
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			int index = e.Index;
			if(e.Index == -1)
				index = (this.SelectedIndex >= 0 ? this.SelectedIndex : 0);
			if(index >= 0 || index < this.Items.Count)
			{
				if(e.State == DrawItemState.Selected || e.State == DrawItemState.None) 
					e.DrawBackground();
				int left = RECTCOLOR_LEFT;
				int top = RECTCOLOR_TOP;
				Rectangle rect = new Rectangle(left,e.Bounds.Top + top,e.Bounds.Width - 2 * left,e.Bounds.Height - 2 * top);
				Color fore = e.ForeColor;
				if((int)e.State == 4113)
					fore = Color.Black;
				using(Pen pen = new Pen(fore,2))
				{
					if(this.Items[index] is DashArray)
					{
						float[] a = ((DashArray)this.Items[index]).Array;
						if(a == null)
						{
							e.Graphics.DrawLine(pen,e.Bounds.Left + left,e.Bounds.Top + e.Bounds.Height / 2 - 1,e.Bounds.Left + e.Bounds .Width - 2 * left,e.Bounds .Top + e.Bounds.Height / 2 - 1);
						}
						else
						{
							pen.DashPattern = a;
							e.Graphics.DrawLine(pen,e.Bounds.Left + left,e.Bounds.Top + e.Bounds.Height / 2 - 1,e.Bounds.Left + e.Bounds .Width - 2 * left,e.Bounds .Top + e.Bounds.Height / 2 - 1);
						}
					}						
				}
			}
			base.OnDrawItem (e);
		}
		#endregion
	}
}
