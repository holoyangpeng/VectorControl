using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// Selection ��ժҪ˵����
	/// </summary>
	internal class Selection
	{
		#region ..���켰����
		public Selection()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		int offset = 0;
		int length = 0;
		int oldoffset,oldlength = 0;
		#endregion

		#region ..�¼�
		public event EventHandler SelectionChanged;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡѡ���Ŀ�ʼ����
		/// </summary>
		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		/// <summary>
		/// ��ȡѡ���ĳ���
		/// </summary>
		public int Length
		{
			get
			{
				return this.length;
			}
		}

		/// <summary>
		/// �ж�ѡ���Ƿ�Ϊ��
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.length == 0;
			}
		}

		public int OldOffset
		{
			get
			{
				return this.oldoffset;
			}
		}

		public int OldLength
		{
			get
			{
				return this.oldlength;
			}
		}
		#endregion

		#region ..AdaptSelection
		public void AdaptSelection(int offset,int length)
		{
			if(this.offset != offset || this.length != length)
			{
				int old = this.length;
				this.offset = offset;
				this.length = length;
				if(old != 0 || length != 0)
					this.OnSelectionChanged();
			}
		}
		#endregion

		#region ..OnSelectionChanged
		protected virtual void OnSelectionChanged()
		{
			if(this.SelectionChanged != null)
				this.SelectionChanged(this,EventArgs.Empty);
			this.oldoffset = this.offset;
			this.oldlength = this.length;
		}
		#endregion
	}
}
