using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// ʵ�ֱ༭������
	/// </summary>
	internal abstract class Action
	{
		#region ..���켰����
		internal Action()
		{
			//
			// TODO: �ڴ˴����ӹ��캯���߼�
			//
		}
		#endregion

		/// <summary>
		/// ִ�ж���
		/// </summary>
		/// <param name="eidtor"></param>
		internal abstract void Execute(LabelTextOperation eidtor);
	}
}