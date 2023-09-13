using System;
using System.Drawing;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// Caret 的摘要说明。
	/// </summary>
	internal class Caret
	{
		#region ..构造及消除
		public Caret()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			
		}
		#endregion

		#region ..私有变量
		int offset =0;
		bool visible = true;
		int oldOffset = 0;
		float oldLength = 0; 
		int oriOffset = -1;
		#endregion

		#region ..事件
		public event EventHandler OffsetChanged;
		#endregion

		#region ..公共属性
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
