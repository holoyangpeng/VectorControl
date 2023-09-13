using System;

namespace YP.SVG.Undo
{
	/// <summary>
	/// InnerTextUndoOperation ��ժҪ˵����
	/// </summary>
	public class InnerTextUndoOperation:Interface.IUndoOperation	
	{
		#region ..Constructor
		public InnerTextUndoOperation(SVG.SVGElement changedElement,string oldValue,string newValue)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.ownerElement = changedElement;
		}
		#endregion

		#region ..private fields
		string oldValue = string.Empty;
		string newValue = string.Empty;
		SVG.SVGElement ownerElement = null;
		#endregion

		#region ..Redo
		/// <summary>
		/// �ظ���һ������
		/// </summary>
		public void Redo()
		{
			if(this.ownerElement != null)
				this.ownerElement.InnerText = newValue;
		}
		#endregion

		#region ..Undo
		/// <summary>
		/// ������һ������
		/// </summary>
		public void Undo()
		{
			if(this.ownerElement != null)
				this.ownerElement.InnerText = oldValue;
		}
		#endregion
	}
}
