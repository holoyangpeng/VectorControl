using System;
using System.Collections;
using YP.SVG.Text;
using System.Collections.Generic;

namespace YP.VectorControl.Operation.Text
{
	#region ..Delete¼ü
	/// <summary>
	/// Delete¼ü
	/// </summary>
	internal class Delete:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				if(!editor.Selection.IsEmpty)
				{
					editor.ClearSelects();
					editor.InvokeUndo();
				}
				else
					Delete.DeleteChar(false,editor);
			}
		}

		internal static void DeleteChar(bool pre,Text.TextEditor editor)
		{
			if(editor != null && editor.OwnerTextElement != null)
			{
				bool old = editor.OwnerTextElement.OwnerDocument.AcceptNodeChanged;
				editor.OwnerTextElement.OwnerDocument.AcceptNodeChanged = true;
				List<TextContentInfo> list = editor.OwnerTextElement.TextContentInfos;
				TextContentInfo info = editor.Caret.Info;
				int offset = editor.Caret.Offset;
				if(pre)
				{
					if(offset > 0)
					{
						editor.InvalidateRegion(info,offset-1,1);
						info.RemoveString(offset - 1,1);
						editor.InvokeUndo();
						editor.Caret.AdaptCaret(info,offset-1);
						InfoPos pos = new InfoPos(info,offset-1);
						editor.Selection.AdaptSelection(pos,pos);
						
					}
					else
					{
						int index = list.IndexOf(info);
						if(index - 1 >= 0)
						{
							TextContentInfo info1 = list[index - 1] as TextContentInfo;
							InfoPos pos = new InfoPos(info1,info1.TextContent.Length);
							TextContentInfo info2 = list[list.Count - 1] as TextContentInfo;
							editor.InvalidateRegion(pos,new InfoPos(info2,info2.TextContent.Length));
							
							info1.AppendString(info.TextContent);
							if(info.OwnerTextNode != null && info.OwnerTextNode.ParentNode is SVG.SVGElement)
								(info.OwnerTextNode.ParentNode as SVG.SVGElement).InternalRemoveChild(info.OwnerTextNode);
							if(info.OwnerTextContentElement != null && info.OwnerTextContentElement.ParentNode is SVG.SVGElement)
								(info.OwnerTextContentElement.ParentNode as SVG.SVGElement).InternalRemoveChild(info.OwnerTextContentElement);
							editor.OwnerTextElement.RemoveInfo(info);
							editor.InvokeUndo();
							editor.Caret.AdaptCaret(info1,info1.TextContent.Length);
							pos = new InfoPos(info1,info1.TextContent.Length);
							editor.Selection.AdaptSelection(pos,pos);
							
						}
					}
				}
				else
				{
					if(offset < info.TextContent.Length)
					{
						editor.InvalidateRegion(info,offset,1);
						info.RemoveString(offset,1);
						
					}
					else
					{
						int index = list.IndexOf(info);
						if(index + 1<list.Count)
						{
							InfoPos pos = new InfoPos(info,offset);
							TextContentInfo info2 = list[list.Count - 1] as TextContentInfo;
							editor.InvalidateRegion(pos,new InfoPos(info2,info2.TextContent.Length));

							TextContentInfo info1 = list[index + 1] as TextContentInfo;
							info.AppendString(info1.TextContent);
							(info1.OwnerTextNode.ParentNode as SVG.SVGElement).InternalRemoveChild(info1.OwnerTextNode);
							(info1.OwnerTextContentElement.ParentNode as SVG.SVGElement).InternalRemoveChild(info1.OwnerTextContentElement);
							editor.OwnerTextElement.RemoveInfo(info1);
						}
					}
					editor.InvokeUndo();
				}
			}
			editor.OwnerTextElement.OwnerDocument.AcceptNodeChanged = true;
			
		}
	}
	#endregion

	#region ..BackSpace¼ü
	/// <summary>
	/// BackSpace¼ü
	/// </summary>
	internal class BackSpace:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				if(!editor.Selection.IsEmpty)
				{
					editor.ClearSelects();
					editor.InvokeUndo();
				}
				else
					Delete.DeleteChar(true,editor);
			}
		}
	}
	#endregion

	#region ..Tab¼ü
	/// <summary>
	/// BackSpace¼ü
	/// </summary>
	internal class Tab:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				editor.Insert("\t");				
			}
		}
	}
	#endregion

	#region ..Control + C
	internal class Copy:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				editor.Copy();
			}
		}
	}
	#endregion

	#region ..Control + X
	internal class Cut:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				editor.Copy();
				editor.ClearSelects();
			}
		}
	}
	#endregion

	#region ..Clear
	internal class Clear:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				editor.Clear();
			}
		}
	}
	#endregion

	#region ..Control+£Ö
	internal class Paste:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor != null)
			{
				editor.Paste();
			}
		}
	}
	#endregion

	#region ..Enter
	internal class Enter:Action
	{
		internal override void Execute(Text.TextEditor editor)
		{
			if(editor.Selection.IsEmpty)
				editor.SplitInOffset(new InfoPos(editor.Caret.Info,editor.Caret.Offset));
			else
				editor.SplitInOffset(editor.Selection.StartPos);
		}
	}
	#endregion
}
