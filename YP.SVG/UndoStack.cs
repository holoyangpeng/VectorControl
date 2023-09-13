using System;
using System.Collections;
using System.Collections.Specialized;

using YP.SVG.Interface;

namespace YP.SVG
{
    /// <summary>
    /// define the stack to store the undo operation
    /// </summary>
	public class UndoStack
	{
		#region ..˽�б���
		Stack undostack = new Stack();
		Stack redostack = new Stack();
		bool acceptChanges = true;
		Document.SVGDocument doc = null;
		#endregion

		#region ..�����ֶ�
		/// <summary>
		/// �����Ƿ��¼��������
		/// </summary>
		public bool AcceptChanges
		{
			get
			{
				return acceptChanges;
			}
		}	
		#endregion

		#region ..���켰����
		public UndoStack(Document.SVGDocument doc)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.doc = doc;
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ȷ����ǰ�Ƿ����ִ�г�������
		/// </summary>
		public bool CanUndo
		{
			get
			{
				return this.undostack.Count > 0;
			}
		}

		/// <summary>
		/// ȷ����ǰ�Ƿ����ִ����������
		/// </summary>
		public bool CanRedo
		{
			get
			{
				return this.redostack.Count > 0;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ����ջ���ƽ�һ��������ִ�д˲���֮�����е�������¼�������
		/// </summary>
		/// <param name="operation">����</param>
		public virtual void Push(Interface.IUndoOperation[] operations) 
		{
			if (operations == null) 
			{
				throw new ArgumentNullException("����Ĳ���");
			}
		
			if (AcceptChanges) 
			{
				this.acceptChanges = false;
				UndoCollection undo = new UndoCollection(operations); 
				undostack.Push(undo);
				ClearRedoStack();
				this.acceptChanges = true;
			}
		}

		/// <summary>
		/// ���������¼
		/// </summary>
		void ClearRedoStack()
		{
			redostack.Clear();
		}
		
		/// <summary>
		/// �����ջ��t���м�¼
		/// </summary>
		public void ClearAll()
		{
			undostack.Clear();
			redostack.Clear();
		}

		/// <summary>
		/// �����ϴμ�¼
		/// </summary>
		/// <returns>�ڳ��������з����ı�Ķ���</returns>
		public virtual Interface.ISVGElement[] Undo()
		{
			if(this.CanUndo)
			{
				bool old = this.AcceptChanges;
				this.acceptChanges = false;
				UndoCollection operations = (UndoCollection)this.undostack.Pop();
				if(operations == null)
					return null;
				DateTime time1 = operations.OperateTime;
				ArrayList list = new ArrayList();
				for(int i = 0;i<operations.Count;i++)//each(IUndoOperation undo in operations)
				{
					Interface.IUndoOperation undo = operations[i] as Interface.IUndoOperation;
					if(undo != null)
					{
						if(undo is YP.SVG.Undo.NodeUndoOperation)
						{
							if(!list.Contains((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement)&& (undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement != null)
								list.Add((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement);
						}
						undo.Undo();
					}
				}
				if(list.Count > 0)
				{
					YP.SVG.SVGElement[] l1 = new SVGElement[list.Count];
					list.CopyTo(l1);
					this.doc.InvokeSelectionChanged(new CollectionChangedEventArgs(l1,CollectionChangeAction.Change));
				}
				this.redostack.Push(operations);
				this.acceptChanges = old;
			}
			return null;			
		}

		/// <summary>
		/// �ظ��ϴμ�¼
		/// </summary>
		/// <returns>�����������з����ı�Ķ���</returns>
		public virtual Interface.ISVGElement[] Redo()
		{
			if(this.CanRedo)
			{
				bool old = this.AcceptChanges;
				this.acceptChanges = false;
				UndoCollection operations = (UndoCollection)this.redostack.Pop();
				if(operations == null)
					return null;
				DateTime time1 = operations.OperateTime;
				ArrayList list = new ArrayList();
				for(int i = operations.Count - 1;i>=0;i--)
				{
					Interface.IUndoOperation undo = operations[i] as Interface.IUndoOperation;
					if(undo != null)
					{
						undo.Redo();
						if(undo is YP.SVG.Undo.NodeUndoOperation)
						{
							if(!list.Contains((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement) && (undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement != null)
								list.Add((undo as YP.SVG.Undo.NodeUndoOperation).ChangedElement);
						}
					}
				}
				if(list.Count > 0)
				{
					YP.SVG.SVGElement[] l1 = new SVGElement[list.Count];
					list.CopyTo(l1);
					this.doc.InvokeSelectionChanged(new CollectionChangedEventArgs(l1,CollectionChangeAction.Change));
				}
				this.undostack.Push(operations);
				this.acceptChanges = old;
			}
			return null;			
		}
		#endregion

		#region ..���¶���
		public void Update()
		{
			if(this.CanUndo)
			{
				UndoCollection undo = this.undostack.Peek() as UndoCollection;
			}
		}
		#endregion
	}
}
