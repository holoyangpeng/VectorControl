using System;

namespace YP.SVG
{
	/// <summary>
	/// 实现SVGException
	/// </summary>
	public class SVGException:System.Exception
	{
		#region ..构造及消除
		public SVGException(string message,SVGExceptionType exceptionType,System.Exception innerException):base(message,innerException)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.svgExceptionType = exceptionType;
		}
		#endregion

		#region ..私有变量
		SVGExceptionType svgExceptionType;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取例外的类别
		/// </summary>
		public SVGExceptionType SVGExceptionType
		{
			get
			{
				return this.svgExceptionType;
			}
		}
		#endregion
	}
}
