using System;
using System.Drawing;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// Caret ��ժҪ˵����
	/// </summary>
	internal class Caret
	{
		#region ..���켰����
		public Caret()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			
		}
		#endregion

		#region ..˽�б���
		int offset =0;
		bool visible = true;
		int oldOffset = 0;
		float oldLength = 0; 
		int oriOffset = -1;
		#endregion

		#region ..�¼�
		public event EventHandler OffsetChanged;
		#endregion

		#region ..��������
		public int Offset
		{
			set
			{
				value = (int)Math.Max(0,value);
				if(offset != value)
				{
					this.oldOffset = this.offset;
					this.offset = value;
					this.OnOffsetChanged();
				}
			}
			get
			{
				return this.offset;
			}
		}

		public int OriOffset
		{
			set
			{
				this.oriOffset = value;
			}
			get
			{
				return this.oriOffset;
			}
		}

		public float OldLength
		{
			set
			{
				this.oldLength = value;
			}
			get
			{
				return this.oldLength;
			}
		}

		public int OldOffset
		{
			get
			{
				return this.oldOffset;
			}
		}

		public bool Visible
		{
			set
			{
				this.visible = value;
			}
			get
			{
				return this.visible;
			}
		}
		#endregion

		#region .. OnOffsetChanged
		protected virtual void OnOffsetChanged()
		{
			if(this.OffsetChanged != null)
				this.OffsetChanged(this,EventArgs.Empty);
		}
		#endregion
	}
}
