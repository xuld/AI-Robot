/*************************************************************************
 *
 * Copyright (c) 2009-2012 Xuld. All rights reserved.
 * 
 * Project Url: http://work.xuld.net/coreplus
 * 
 * This source code is part of the Project CorePlus for .Net.
 * 
 * This code is licensed under CorePlus License.
 * See the file License.html for the license details.
 * 
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * 
 *************************************************************************/


using System;
using System.Runtime.Serialization;

namespace CorePlus.RunTime {

	/// <summary>
	/// 语法错误产生的异常。继承此类实现更完整的异常（如包含位置，所操作的操作符）。
	/// </summary>
	[Serializable]
	public class SyntaxException : Exception {

		/// <summary>
		/// 表示错误类型。
		/// </summary>
		SyntaxErrorType _type = SyntaxErrorType.Other;

		/// <summary>
		/// 发生错误的行。
		/// </summary>
		int _line = 0;

		/// <summary>
		/// 发生错误的列。
		/// </summary>
		int _linePosition = 0;

		/// <summary>
		/// 获取错误类型。
		/// </summary>
		public SyntaxErrorType Type {
			get { return _type; }
			protected set {
				_type = value;
			}
		}

		/// <summary>
		/// 初始化 CorePlus.SyntaxException 的新实例。
		/// </summary>
		public SyntaxException()
			: base() {
            

		}

		/// <summary>
		/// 获取指示错误发生位置的行号。
		/// </summary>
		/// <returns>
		/// 指示错误发生位置的行号。
		/// </returns>
		public int LineNumber {
			get {
				return _line;
			}
			protected set {
				_line = value;
			}
		}

		/// <summary>
		/// 获取指示错误发生位置的行位置。
		/// </summary>
		/// <returns>
		/// 指示错误发生位置的行位置。
		/// </returns>
		public int LinePosition {
			get {
				return _linePosition;
			}
			protected set {
				_linePosition = value;
			}
		}

		/// <summary>
		/// 初始化 CorePlus.SyntaxException 的新实例。
		/// </summary>
		/// <param name="type">错误种类。</param>
		protected SyntaxException(SyntaxErrorType type) {

			_type = type;
		}

		/// <summary>
		/// 初始化 CorePlus.SyntaxException 的新实例。
		/// </summary>
		/// <param name="type">错误种类。</param>
		/// <param name="message">信息。</param>
		public SyntaxException( string message, SyntaxErrorType type)
			: base(message) {
			_type = type;
		}

		/// <summary>
		/// 初始化 CorePlus.SyntaxException 的新实例。
		/// </summary>
		/// <param name="type">错误种类。</param>
		/// <param name="message">信息。</param>
		/// <param name="line">发生错误的行。</param>
		/// <param name="column">发生错误的列。</param>
        public SyntaxException(string message, SyntaxErrorType type, int line, int column)
			: base(message) {
			_type = type;
			_line = line;
			_linePosition = column;
		}

		/// <summary>
		/// 初始化 CorePlus.SyntaxException 的新实例。
		/// </summary>
		/// <param name="type">错误种类。</param>
		/// <param name="message">信息。</param>
		/// <param name="line">发生错误的行。</param>
		/// <param name="column">发生错误的列。</param>
		/// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。</param>
        public SyntaxException(string message, SyntaxErrorType type, Exception innerException, int line, int column)
			: base(message, innerException) {
			_type = type;
			_line = line;
			_linePosition = column;
		}

		/// <summary>
		/// 初始化 <see cref="SyntaxException"/> 的新实例。
		/// </summary>
		/// <param name="message">信息。</param>
		public SyntaxException(string message)
			: base(message) {

		}

		/// <summary>
		/// 使用 <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 和 <see cref="T:System.Runtime.Serialization.StreamingContext" /> 对象中的信息初始化 <see cref="SyntaxException"/> 类的新实例。
		/// </summary>
		/// <param name="info">SerializationInfo 对象，包含 <see cref="SyntaxException"/> 的所有属性。
		/// </param>
		/// <param name="context">StreamingContext 对象，包含上下文信息。
		/// </param>
		protected SyntaxException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			_type = (SyntaxErrorType)info.GetInt32("Type");
			_line = info.GetInt32("Line");
			_linePosition = info.GetInt32("Column");
		}


		/// <summary>
		/// 创建并返回当前异常的字符串表示形式。
		/// </summary>
		/// <returns>当前异常的字符串表示形式。</returns>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
		/// </PermissionSet>
		public override string ToString() {
			return _line > 0 ? String.Concat("[", Type, "] 信息:", Message, "位置：行:", _line, "  列:", _linePosition)
				:
				String.Concat("[", Type, "] 信息:", Message);
		}

		/// <summary>
		/// 对于给定的 <see cref="T:System.Runtime.Serialization.StreamingContext" />，将所有的 HtmlException 属性流式写入 <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 类。
		/// </summary>
		/// <param name="info">SerializationInfo 对象。
		/// </param>
		/// <param name="context">StreamingContext 对象。
		/// </param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("lineNumber", _line);
			info.AddValue("linePosition", _linePosition);
		}
	}

}