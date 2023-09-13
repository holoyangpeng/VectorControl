using System;
using YP.SVG.Text;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// InfoPos ��ժҪ˵����
	/// </summary>
	internal struct InfoPos
	{
		#region ..���켰����
		public InfoPos(TextContentInfo info ,int offset)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.Info = info;
			this.Offset = offset;
		}
		#endregion

		#region ..˽�б���
		public TextContentInfo Info;
		public int Offset;
		#endregion

		public static bool operator == (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return true;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return false;
			return pos1.Info == pos2.Info && pos1.Offset == pos2.Offset;
		}

		public static bool operator != (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return false;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return true;
			return pos1.Info != pos2.Info || pos1.Offset != pos2.Offset;
		}

		public static bool operator< (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return false;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return false;
			int index = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			int index1 = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			if(index < index1)
				return true;
			if(index1 == index)
				return pos1.Offset < pos2.Offset;
			return false;
		}

		public static bool operator > (InfoPos pos1,InfoPos pos2)
		{
			if(pos1.Equals(null) || pos2.Equals(null))
				return false;
			if((pos1.Equals(null)) != (pos2.Equals(null)))
				return false;
			int index = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			int index1 = pos1.Info.OwnerTextContentElement.OwnerTextElement.TextContentInfos.IndexOf(pos1.Info);
			if(index < index1)
				return false;
			if(index1 == index)
				return pos1.Offset > pos2.Offset;
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if(obj is InfoPos)
				return this == (InfoPos)obj;
			return base.Equals (obj);
		}


	}
}
